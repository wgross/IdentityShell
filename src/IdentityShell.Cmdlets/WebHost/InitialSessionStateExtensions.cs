using System.Management.Automation.Runspaces;

namespace IdentityShell.Cmdlets.WebHost
{
    public static class InitialSessionStateExtensions
    {
        public static InitialSessionState AddWebHostCommands(this InitialSessionState sessionState)
        {
            sessionState.Commands.Add(new SessionStateCmdletEntry("Restart-IdentityServer", typeof(RestartIdentityServerCommand), string.Empty));

            return sessionState;
        }
    }
}