using PamelloV7.Framework.Shared.Entities;

namespace PamelloV7.Framework.Core.Scope.Attributes;

public class CustomScopeAttribute<TUserType> : Attribute
    where TUserType : IPamelloBasicUser;
