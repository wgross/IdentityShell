using IdentityModel.Client;
using IdentityShell.Commands.Endpoints;
using System.Management.Automation;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityShell.Commands.Endpoints
{
    [Cmdlet(VerbsLifecycle.Invoke, "IdentityUserEndpoint")]
    [OutputType(typeof(UserInfoResponse))]
    public sealed class InvokeIdentityUserInfoEndpointCommand : InvokeIdentityEndpointCommandBase
    {
        [Parameter(Mandatory = true)]
        public string Token { get; set; }

        protected override void ProcessRecord() => this.WriteObject(Await(InvokeUserInfo()));

        private Task<UserInfoResponse> InvokeUserInfo() => new HttpClient().GetUserInfoAsync(new UserInfoRequest
        {
            Address = this.EndpointUrl ?? this.DiscoveryDocument.UserInfoEndpoint,
            Token = this.Token
        });
    }
}