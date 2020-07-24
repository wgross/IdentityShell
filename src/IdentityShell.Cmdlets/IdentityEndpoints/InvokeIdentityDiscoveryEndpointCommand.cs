using IdentityModel.Client;
using System.Management.Automation;
using System.Net.Http;

namespace IdentityShell.Cmdlets.IdentityEndpoints
{
    [Cmdlet(VerbsLifecycle.Invoke, "IdentityDiscoveryEndpoint")]
    [OutputType(typeof(DiscoveryDocumentResponse))]
    public sealed class InvokeIdentityDiscoveryEndpointCommand : IdentityCommandBase
    {
        [Parameter()]
        public string EndpointUrl { get; set; } = "http://localhost:5000/.well-known/openid-configuration";

        protected override void ProcessRecord() => this.WriteObject(Await(new HttpClient().GetDiscoveryDocumentAsync(address: this.EndpointUrl)));
    }
}