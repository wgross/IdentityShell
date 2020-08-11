using Microsoft.PowerShell;
using System.Management.Automation.Runspaces;

namespace IdentityShell.Cmdlets.Operation
{
    public static class InitialSessionStateExtensions
    {
        public static InitialSessionState AddIdentityOperationCommands(this InitialSessionState sessionState)
        {
            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-IdentityPersistedGrant", typeof(GetIdentityPersistedGrantCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-IdentityPersistedGrant", typeof(RemoveIdentityPersistedGrantCommand), string.Empty));

            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-IdentityDeviceCode", typeof(GetIdentityDeviceCodeCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-IdentityDeviceCode", typeof(RemoveIdentityDeviceCodeCommand), string.Empty));

            return sessionState;
        }
    }
}