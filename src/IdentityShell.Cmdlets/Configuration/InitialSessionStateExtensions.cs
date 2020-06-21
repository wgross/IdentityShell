using Microsoft.PowerShell;
using System.Management.Automation.Runspaces;

namespace IdentityShell.Cmdlets.Configuration
{
    public static class InitialSessionStateExtensions
    {
        public static InitialSessionState AddIdentityConfigurationCommands(this InitialSessionState sessionState)
        {
            sessionState.ExecutionPolicy = ExecutionPolicy.Unrestricted;
            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-IdentityClient", typeof(GetIdentityClientCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-IdentityClient", typeof(SetIdentityClientCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-IdentityClient", typeof(RemoveIdentityClientCommand), string.Empty));

            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-IdentityResource", typeof(GetIdentityResourceCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-IdentityResource", typeof(SetIdentityResourceCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-IdentityResource", typeof(RemoveIdentityResourceCommand), string.Empty));

            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-IdentityApiResource", typeof(GetIdentityApiResourceCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-IdentityApiResource", typeof(SetIdentityApiResourceCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-IdentityApiResource", typeof(RemoveIdentityApiResourceCommand), string.Empty));

            sessionState.Commands.Add(new SessionStateCmdletEntry("New-IdentitySecret", typeof(NewIdentitySecretCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("New-IdentityClaim", typeof(NewIdentityClaimCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("New-IdentityScope", typeof(NewIdentityScopeCommand), string.Empty));
            return sessionState;
        }
    }
}