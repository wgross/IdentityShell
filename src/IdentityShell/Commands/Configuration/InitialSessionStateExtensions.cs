using System.Management.Automation.Runspaces;

namespace IdentityShell.Commands.Configuration
{
    public static class InitialSessionStateExtensions
    {
        public static InitialSessionState AddIdentityConfigurationCommands(this InitialSessionState sessionState)
        {
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-IdentityApiScope", typeof(SetIdentityApiScopeCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-IdentityApiScope", typeof(GetIdentityApiScopeCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-IdentityApiScope", typeof(RemoveIdentityApiScopeCommand), string.Empty));

            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-IdentityClient", typeof(GetIdentityClientCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-IdentityClient", typeof(SetIdentityClientCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-IdentityClient", typeof(RemoveIdentityClientCommand), string.Empty));

            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-IdentityResource", typeof(GetIdentityResourceCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-IdentityResource", typeof(SetIdentityResourceCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-IdentityResource", typeof(RemoveIdentityResourceCommand), string.Empty));

            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-IdentityApiResource", typeof(GetIdentityApiResourceCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-IdentityApiResource", typeof(SetIdentityApiResourceCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-IdentityApiResource", typeof(RemoveIdentityApiResourceCommand), string.Empty));

            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-TestUser", typeof(GetTestUserCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-TestUser", typeof(SetTestUserCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-TestUser", typeof(RemoveTestUserCommand), string.Empty));

            sessionState.Commands.Add(new SessionStateCmdletEntry("New-IdentitySecret", typeof(NewIdentitySecretCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("New-IdentityScope", typeof(NewIdentityScopeCommand), string.Empty));
            return sessionState;
        }
    }
}