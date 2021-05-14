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
        public string EndpointUrl { get; set; } = null;

        protected string DiscoveryEndpoint
        {
            get => this.discoveryEndpoint ??= this.GetDiscoveryEndpoint();
        }

        protected string GetDiscoveryEndpoint()
        {
            var config = this.LocalServiceProvider.GetRequiredService<IConfiguration>();

            var firstAddress = config["Urls"];

            if (firstAddress is null)
            {
                throw new PSInvalidOperationException("Discovery endpoint couldn't be determined");
            }

            firstAddress = firstAddress.Split(";").FirstOrDefault();

            if (string.IsNullOrEmpty(firstAddress))
            {
                throw new PSInvalidOperationException($"Discovery endpoint couldn't be determined from '{config["Urls"]}'");
            }

            return $"{firstAddress}/.well-known/openid-configuration";
        }

        protected DiscoveryDocumentResponse DiscoveryDocument
        {
            get => this.discoveryDocument ??= Await(new HttpClient().GetDiscoveryDocumentAsync(address: this.EndpointUrl ?? this.DiscoveryEndpoint));
        }
    }
}