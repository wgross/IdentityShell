using IdentityModel.Client;
using System.Management.Automation;
using System.Net.Http;

namespace IdentityShell.Commands.Endpoints
{
    [Cmdlet(VerbsLifecycle.Invoke, "IdentityTokenEndpoint")]
    [CmdletBinding(DefaultParameterSetName = "clientcredentials")]
    [OutputType(typeof(TokenResponse))]
    public sealed class InvokeIdentityTokenEndpointCommand : InvokeIdentityEndpointCommandBase
    {
        [Parameter(ParameterSetName = "clientcredentials", Mandatory = true)]
        [Parameter(ParameterSetName = "password")]
        public string ClientId { get; set; }

        [Parameter(ParameterSetName = "clientcredentials", Mandatory = true)]
        [Parameter(ParameterSetName = "password")]
        public string ClientSecret { get; set; }

        [Parameter(ParameterSetName = "clientcredentials", Mandatory = true)]
        [Parameter(ParameterSetName = "password", Mandatory = true)]
        public string[] Scopes { get; set; }

        #region Password Authorization

        [Parameter(ParameterSetName = "password", Mandatory = true)]
        public string UserName { get; set; }

        [Parameter(ParameterSetName = "password")]
        public string Password { get; set; }

        #endregion Password Authorization

        [Parameter()]
        public string TokenVariableName { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName.Equals("clientcredentials"))
            {
                var tokenRequest = new ClientCredentialsTokenRequest
                {
                    Address = this.EndpointUrl ?? this.DiscoveryDocument.TokenEndpoint,
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    Scope = string.Join(" ", this.Scopes)
                };

                this.ProcessTokenResponse(Await(new HttpClient().RequestClientCredentialsTokenAsync(tokenRequest)));
            }
            else if (this.ParameterSetName.Equals("password"))
            {
                var tokenRequest = new PasswordTokenRequest
                {
                    Address = this.EndpointUrl ?? this.DiscoveryDocument.TokenEndpoint,
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    UserName = this.UserName,
                    Password = this.Password,
                    Scope = string.Join(" ", this.Scopes)
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