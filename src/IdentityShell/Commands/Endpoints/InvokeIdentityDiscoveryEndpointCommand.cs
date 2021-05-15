using IdentityModel.Client;
using System.Management.Automation;
using System.Net.Http;

namespace IdentityShell.Commands.Endpoints
{
    [Cmdlet(VerbsLifecycle.Invoke, "IdentityDiscoveryEndpoint")]
    [OutputType(typeof(DiscoveryDocumentResponse))]
    public sealed class InvokeIdentityDiscoveryEndpointCommand : InvokeIdentityEndpointCommandBase
    {
        protected override void ProcessRecord() => this.WriteObject(Await(new HttpClient().GetDiscoveryDocumentAsync(address: this.AuthorityUri ?? this.DiscoveryEndpoint)));
    }
}