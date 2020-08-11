using System.Management.Automation.Runspaces;

namespace IdentityShell.Cmdlets.IdentityEndpoints
{
    public static class InitialSessionStateExtensions
    {
        public static InitialSessionState AddEndpointCommands(this InitialSessionState sessionState)
        {
            sessionState.Commands.Add(new SessionStateCmdletEntry("Invoke-IdentityDiscoveryEndpoint", typeof(InvokeIdentityDiscoveryEndpointCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Invoke-IdentityTokenEndpoint", typeof(InvokeIdentityTokenEndpointCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Invoke-IdentityUserInfoEndpoint", typeof(InvokeIdentityUserInfoEndpointCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("Invoke-IdentityTokenIntrospectionEndpoint", typeof(InvokeIdentityTokenIntrospectionEndpointCommand), string.Empty));

            return sessionState;
        }
    }
}