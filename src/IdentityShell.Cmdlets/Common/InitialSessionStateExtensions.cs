using Microsoft.PowerShell;
using System.Management.Automation.Runspaces;

namespace IdentityShell.Cmdlets.Common
{
    public static class InitialSessionStateExtensions
    {
        public static InitialSessionState AddCommonCommands(this InitialSessionState sessionState)
        {
            sessionState.ExecutionPolicy = ExecutionPolicy.Unrestricted;
            sessionState.Commands.Add(new SessionStateCmdletEntry("New-Claim", typeof(NewClaimCommand), string.Empty));

            return sessionState;
        }
    }
}