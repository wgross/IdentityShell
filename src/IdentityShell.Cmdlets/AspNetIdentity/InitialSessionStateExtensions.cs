using Microsoft.PowerShell;
using System.Management.Automation.Runspaces;

namespace IdentityShell.Cmdlets.AspNetIdentity
{
    public static class InitialSessionStateExtensions
    {
        public static InitialSessionState AddAspIdentityCommands(this InitialSessionState sessionState)
        {
            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-AspNetIdentityUser", typeof(GetAspNetIdentityUserCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-AspNetIdentityUser", typeof(SetAspNetIdentityUserCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-AspNetIdentityUser", typeof(RemoveAspNetIdentityUserCommand), string.Empty));

            sessionState.Commands.Add(new SessionStateCmdletEntry("Get-AspNetIdentityUserClaim", typeof(GetAspNetIdentityUserClaimCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Set-AspNetIdentityUserClaim", typeof(SetAspNetIdentityUserClaimCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Remove-AspNetIdentityUserClaim", typeof(RemoveAspNetIdentityUserClaimCommand), string.Empty));

            return sessionState;
        }
    }
}