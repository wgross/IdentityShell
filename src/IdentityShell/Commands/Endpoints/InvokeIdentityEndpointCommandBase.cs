using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;
using System.Net.Http;

namespace IdentityShell.Commands.Endpoints
{
    public abstract class InvokeIdentityEndpointCommandBase : IdentityCommandBase
    {
        private string discoveryEndpoint;
        private DiscoveryDocumentResponse discoveryDocument;

        [Parameter()]
        [ArgumentCompleter(typeof(AuthorityUrlCompleter))]
        public string AuthorityUri { get; set; } = null;

        protected string DiscoveryEndpoint
        {
            get => this.discoveryEndpoint ??= this.GetDiscoveryEndpoint();
        }

        protected string GetDiscoveryEndpoint()
        {
            var config = this.LocalServiceProvider.GetRequiredService<IConfiguration>();

            if (string.IsNullOrEmpty(this.AuthorityUri))
            {
                this.AuthorityUri = config["Urls"]?.Split(";")?.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(this.AuthorityUri))
            {
                throw new PSInvalidOperationException("Discovery endpoint is nether specified nor configured");
            }

            if (string.IsNullOrEmpty(this.AuthorityUri))
            {
                throw new PSInvalidOperationException($"Discovery endpoint couldn't be determined from '{config["Urls"]}'");
            }

            var metadataEndpoint = $"{this.AuthorityUri }/.well-known/openid-configuration";

            this.WriteVerbose($"Using metdata endpoint: '{metadataEndpoint}'");

            return metadataEndpoint;
        }

        protected DiscoveryDocumentResponse DiscoveryDocument
        {
            get => this.discoveryDocument ??= Await(new HttpClient().GetDiscoveryDocumentAsync(address: this.AuthorityUri ?? this.DiscoveryEndpoint));
        }
    }
}