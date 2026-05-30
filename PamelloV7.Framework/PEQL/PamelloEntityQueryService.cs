using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Actions;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Core.PEQL.Operators;
using PamelloV7.Framework.Core.PEQL.Range;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Core.Scope;
using PamelloV7.Framework.PEQL.Descriptors;
using PamelloV7.Framework.Repositories.Loaders;
using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Exceptions;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.PEQL;

public partial class PamelloEntityQueryService : IPamelloEntityQueryService
{
    private readonly IServiceProvider _services;
    
    private readonly PamelloRepositoriesLoader _repositoriesLoader;
    
    public readonly List<PamelloQueryProviderDescriptor> Providers = [];
    public readonly List<PamelloQueryOperatorDescriptor> Operators = [];
    
    public PamelloEntityQueryService(IServiceProvider services) {
        _services = services;
        
        _repositoriesLoader = services.GetRequiredService<PamelloRepositoriesLoader>();
    }

    public void Startup(IServiceProvider services) {
        PamelloOutput.Write("Adding providers");
        foreach (var repositoryDescriptor in _repositoriesLoader.RepositoriesDescriptors) {
            var repository = services.GetRequiredService(repositoryDescriptor.RepositoryType) as IPamelloRepository;
            if (repository is null) continue;

            PamelloOutput.Write($"| {repositoryDescriptor.Attribute.EntityType.Name} \"{repositoryDescriptor.Attribute.ProviderName}\"");
            
            var points = new List<PamelloQueryProviderPointDescriptor>();

            foreach (var method in repositoryDescriptor.RepositoryType.GetMethods()) {
                if (method.GetCustomAttribute<ProviderPointAttribute>() is not { } pointMethodAttribute) continue;

                PamelloOutput.Write($"|   {pointMethodAttribute}");
                
                points.Add(new PamelloQueryProviderPointDescriptor(
                    pointMethodAttribute,
                    method,
                    repository
                ));
            }
            
            Providers.Add(new PamelloQueryProviderDescriptor(
                repositoryDescriptor.Attribute.ProviderName,
                repositoryDescriptor.Attribute.EntityType,
                repository,
                points
            ));
        }

        PamelloOutput.Write("Adding operators");
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes())
            .Where(t => t.GetCustomAttributes().Any(a => a is PamelloQueryOperatorAttribute));
        
        foreach (var type in types) {
            var attribute = type.GetCustomAttribute<PamelloQueryOperatorAttribute>();
            if (attribute is null) continue;

            PamelloOutput.Write($"| {attribute.Operator} {attribute.Name}");
            if (attribute.Description is not null)
                PamelloOutput.Write($"|   {attribute.Description}");
            
            Operators.Add(new PamelloQueryOperatorDescriptor(
                attribute,
                type
            ));
        }
    }
    
    private PamelloQueryProviderDescriptor? GetProviderForEntityType<TEntityType>()
        where TEntityType : class, IPamelloBasicEntity
    {
        return Providers.FirstOrDefault(p => typeof(TEntityType).IsAssignableTo(p.EntityType));
    }
    private PamelloQueryProviderDescriptor? GetProviderByQuery(string name) {
        return Providers.FirstOrDefault(p => p.Name == name);
    }
    
    public TPamelloEntity? GetSingleById<TPamelloEntity>(int id)
        where TPamelloEntity : class, IPamelloBasicEntity
    {
        return GetProviderForEntityType<TPamelloEntity>()?.GetSingleById<TPamelloEntity>(id);
    }
    public IEnumerable<TPamelloEntity> GetByIds<TPamelloEntity>(params int[] ids)
        where TPamelloEntity : class, IPamelloBasicEntity
    {
        return GetProviderForEntityType<TPamelloEntity>()?.GetByIds<TPamelloEntity>(ids) ?? [];
    }
    
    public async IAsyncEnumerable<TPamelloEntity> GetAsync<TPamelloEntity>(string query) 
        where TPamelloEntity : class, IPamelloBasicEntity
    {
        //songs$all((1,2))#{Length>3:00}
        //|songs|$|all|((1,2))|#|{Length>3:00}|
        
        //songs$35,145,episodes$727
        //songs$35|,|145|,|episodes$727
        //
        //songs|$|35
        //145
        //episodes|$|727
        
        var subQueries = query
            .EnumerateStringBlocks([','])
            .ToSingleBlocksAround(block => block.Kind == QueryStringBlockKind.Operator)
            .ToList();

        if (subQueries.Count > 1) {
            string? previousProviderQuery = null;
            
            foreach (var subQuery in subQueries) {
                var (providerSubQuery, entitySubQuery) = SplitFullQuery(subQuery.ToOriginalString());
                
                providerSubQuery ??= previousProviderQuery;
                providerSubQuery ??= GetProviderForEntityType<TPamelloEntity>()?.Name;
                
                previousProviderQuery = providerSubQuery;
                
                await foreach (var entity in GetAsync<TPamelloEntity>($"{providerSubQuery}${entitySubQuery}")) {
                    yield return entity;
                }
            }
            yield break;
        }

        var (providerQuery, entityQuery) = SplitFullQuery(subQueries.First().ToOriginalString());
        
        var provider = providerQuery is not null
            ? GetProviderByQuery(providerQuery) ?? throw new PamelloException($"No provider found by provider query \"{providerQuery}\"")
            : GetProviderForEntityType<TPamelloEntity>() ?? throw new PamelloException($"No provider found for entity type {typeof(TPamelloEntity).Name}");
        
        //
        //id
        //

        var idRange = PamelloQueryRange.ParseOrDefault(entityQuery, true);

        if (idRange is { IsPurelyNumeric: true }) {
            if (idRange.StartNumber == idRange.EndNumber) {
                var entity = provider.GetSingleById<TPamelloEntity>(idRange.StartNumber);
                if (entity is not null) yield return entity;
            
                yield break;
            }
            
            foreach (var entity in provider.GetByIds<TPamelloEntity>(idRange.EnumerateNumericRange().ToArray())) {
                yield return entity;
            }
            
            yield break;
        }
        
        //
        //point
        //
        
        var (pointName, pointArguments) = SplitPointQuery(entityQuery);
        var point = provider.GetPointByName(pointName);

        if (point is not null) {
            await foreach (var entity in point.Execute<TPamelloEntity>(pointArguments ?? "", this)) {
                yield return entity;
            }
        }
    }

    private static (string? ProviderQuery, string EntityQuery) SplitFullQuery(string query) {
        var querySplit = query
            .EnumerateStringBlocks(['$'])
            .ToSingleBlocksAround(block => block.Kind == QueryStringBlockKind.Operator, 2)
            .Select(block => block.ToOriginalString())
            .ToList();
        
        if (querySplit.Count == 1) return (null, querySplit[0]);
        if (querySplit.Count != 2) throw new PamelloException($"Query must have exactly 2 parts, but found {querySplit.Count}");
        
        return (querySplit[0], querySplit[1]);
    }

    private static (string PointName, string? PointArguments) SplitPointQuery(string query) {
        var blocks = query.EnumerateStringBlocks().ToList();
        
        var argumentsBlock = blocks.LastOrDefault(b => b.Kind == QueryStringBlockKind.InParentheses);
        if (argumentsBlock is not null) blocks.Remove(argumentsBlock);

        return (
            blocks.ToSingleBlock()?.ToOriginalString() ?? "",
            argumentsBlock?.Text
        );
    }
    
    //
    //
    //
}
