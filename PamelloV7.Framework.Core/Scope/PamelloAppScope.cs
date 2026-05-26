using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Shared.Entities;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Core.Scope;

public static class PamelloAppScope
{
    private static readonly AsyncLocal<IPamelloBasicUser?> CurrentScopeUser = new();
    
    public static void RequireUser() {
        if (User is null) throw new PamelloNoScopeUserException();
    }
    
    public static IPamelloBasicUser RequiredUser => User ?? throw new PamelloNoScopeUserException();
    public static IPamelloBasicUser? User => CurrentScopeUser.Value;
    
    public static void SetUserIn(IPamelloBasicUser? user, Action action) {
        var previousUser = CurrentScopeUser.Value;
        CurrentScopeUser.Value = user;

        try {
            action();
        }
        finally {
            CurrentScopeUser.Value = previousUser;
        }
    }
    
    public static T SetUserIn<T>(IPamelloBasicUser? user, Func<T> action) {
        var previousUser = CurrentScopeUser.Value;
        CurrentScopeUser.Value = user;

        try {
            return action();
        }
        finally {
            CurrentScopeUser.Value = previousUser;
        }
    }
}
