using IdentityModel.Client;
using IdentityShell.Commands;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
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
            var serverAddressFeature = this.LocalServiceProvider.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
            var firstAddress = serverAddressFeature.Addresses.FirstOrDefault();
            if (firstAddress is null)
            {
                throw new PSInvalidOperationException("Discovery endpoint couldn't determined");
            }
            else
            {
                return $"{firstAddress}/.well-known/openid-configuration";
            }
        }

        protected DiscoveryDocumentResponse DiscoveryDocument
        {
            get => this.discoveryDocument ??= Await(new HttpClient().GetDiscoveryDocumentAsync(address: this.EndpointUrl ?? this.DiscoveryEndpoint));
        }

        
    }
}