using IdentityModel.Client;
using System.Management.Automation;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityShell.Cmdlets.IdentityEndpoints
{
    [Cmdlet(VerbsLifecycle.Invoke, "IdentityUserEndpoint")]
    public sealed class InvokeIdentityUserInfoEndpointCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true)]
        public string EndpointUrl { get; set; }

        [Parameter(Mandatory = true)]
        public string Token { get; set; }

        protected override void ProcessRecord() => this.WriteObject(Await(InvokeUserInfo()));

        private Task<UserInfoResponse> InvokeUserInfo() => new HttpClient().GetUserInfoAsync(new UserInfoRequest
        {
            Address = this.EndpointUrl,
            Token = this.Token
        });
    }
}