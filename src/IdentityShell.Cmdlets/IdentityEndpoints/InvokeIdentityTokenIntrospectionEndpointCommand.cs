using IdentityModel.Client;
using System.Management.Automation;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityShell.Cmdlets.IdentityEndpoints
{
    [Cmdlet(VerbsLifecycle.Invoke, "IdentityTokenIntrospectionEndpoint")]
    [OutputType(typeof(TokenIntrospectionResponse))]
    public sealed class InvokeIdentityTokenIntrospectionEndpointCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true)]
        public string EndpointUrl { get; set; }

        [Parameter(Mandatory = true)]
        public string Token { get; set; }

        [Parameter(Mandatory = true)]
        public string ApiResource { get; set; }

        [Parameter()]
        public string ApiSecret { get; set; }

        protected override void ProcessRecord() => this.WriteObject(Await(this.InvokeTokenIntrospection()));

        private async Task<TokenIntrospectionResponse> InvokeTokenIntrospection()
        {
            return await new HttpClient().IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = this.EndpointUrl,
                Token = this.Token,
                ClientId = this.ApiResource,
                ClientSecret = this.ApiSecret
            });
        }
    }
}