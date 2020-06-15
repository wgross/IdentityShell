using Microsoft.PowerShell;
using System.Management.Automation.Runspaces;

namespace IdentityShell.Commands
{
    public static class InitialSessionStateExtensions
    {
        public static InitialSessionState AddIdentityCommands(this InitialSessionState sessionState)
        {
            sessionState.ExecutionPolicy = ExecutionPolicy.Unrestricted;
            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-IdentityClient", typeof(GetIdentityClientCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-IdentityClient", typeof(SetIdentityClientCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-IdentityResource", typeof(GetIdentityResourceCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-IdentityResource", typeof(SetIdentityResourceCommand), string.Empty));
            return sessionState;
        }
    }
}