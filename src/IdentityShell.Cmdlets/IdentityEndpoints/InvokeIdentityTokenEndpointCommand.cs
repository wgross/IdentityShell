using IdentityModel.Client;
using System.Management.Automation;
using System.Net.Http;

namespace IdentityShell.Cmdlets.IdentityEndpoints
{
    [Cmdlet(VerbsLifecycle.Invoke, "IdentityTokenEndpoint")]
    [CmdletBinding(DefaultParameterSetName = "clientcredentials")]
    [OutputType(typeof(TokenResponse))]
    public sealed class InvokeIdentityTokenEndpointCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true)]
        public string EndpointUrl { get; set; }

        [Parameter(ParameterSetName = "clientcredentials", Mandatory = true)]
        [Parameter(ParameterSetName = "password")]
        public string ClientId { get; set; }

        [Parameter(ParameterSetName = "clientcredentials", Mandatory = true)]
        [Parameter(ParameterSetName = "password")]
        public string ClientSecret { get; set; }

        [Parameter(ParameterSetName = "clientcredentials", Mandatory = true)]
        [Parameter(ParameterSetName = "password", Mandatory = true)]
        public string Scope { get; set; }

        #region Password Authorizaion

        [Parameter(ParameterSetName = "password", Mandatory = true)]
        public string UserName { get; set; }

        [Parameter(ParameterSetName = "password")]
        public string Password { get; set; }

        #endregion Password Authorizaion

        [Parameter()]
        public string TokenVariableName { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName.Equals("clientcredentials"))
            {
                var tokenRequest = new ClientCredentialsTokenRequest
                {
                    Address = this.EndpointUrl,
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    Scope = this.Scope
                };

                this.ProcessTokenResponse(Await(new HttpClient().RequestClientCredentialsTokenAsync(tokenRequest)));
            }
            else if (this.ParameterSetName.Equals("password"))
            {
                var tokenRequest = new PasswordTokenRequest
                {
                    Address = this.EndpointUrl,
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    UserName = this.UserName,
                    Password = this.Password,
                    Scope = this.Scope,
                };

                this.ProcessTokenResponse(Await(new HttpClient().RequestPasswordTokenAsync(tokenRequest)));
            }
        }

        private void ProcessTokenResponse(TokenResponse tokenResponse)
        {
            if (tokenResponse.IsError)
            {
                this.WriteError(new ErrorRecord(tokenResponse.Exception ?? new PSInvalidOperationException("request was rejected"),
                    errorId: tokenResponse.Error,
                    errorCategory: ErrorCategory.AuthenticationError,
                    targetObject: tokenResponse));
            }
            else
            {
                if (!string.IsNullOrEmpty(this.TokenVariableName))
                {
                    this.SessionState.PSVariable.Set(this.TokenVariableName, tokenResponse.AccessToken);
                }
                this.WriteObject(tokenResponse);
            }
        }
    }
}