using IdentityModel.Client;
using IdentityShell.Commands.Configuration.ArgumentCompleters;
using System.Management.Automation;
using System.Net.Http;

namespace IdentityShell.Commands.Endpoints
{
    [Cmdlet(VerbsLifecycle.Invoke, "IdentityTokenEndpoint")]
    [CmdletBinding(DefaultParameterSetName = "clientcredentials")]
    [OutputType(typeof(TokenResponse))]
    public sealed class InvokeIdentityTokenEndpointCommand : InvokeIdentityEndpointCommandBase
    {
        #region Client Credentials

        [Parameter(ParameterSetName = "clientcredentials", Mandatory = true)]
        [Parameter(ParameterSetName = "password")]
        [ArgumentCompleter(typeof(IdentityClientIdCompleter))]
        public string ClientId { get; set; }

        [Parameter(ParameterSetName = "clientcredentials", Mandatory = true)]
        [Parameter(ParameterSetName = "password")]
        public string ClientSecret { get; set; }

        #endregion Client Credentials

        [Parameter(
            ParameterSetName = "clientcredentials",
            Mandatory = true,
            HelpMessage = "Defines the scopes to access. Default is openid,profile")]
        [Parameter(
            ParameterSetName = "password",
            Mandatory = true,
            HelpMessage = "Defines the scopes to access. Default is openid,profile")]
        [ArgumentCompleter(typeof(IdentityApiScopeNameCompleter))]
        public string[] Scopes { get; set; } = new[] { "openid", "profile" };

        #region User Credentials

        [Parameter(ParameterSetName = "password", Mandatory = true)]
        [ArgumentCompleter(typeof(TestUserNameCompleter))]
        public string UserName { get; set; }

        [Parameter(ParameterSetName = "password")]
        public string Password { get; set; }

        #endregion User Credentials

        [Parameter(HelpMessage = "Optional variable name to store the aquired token in")]
        public string TokenVariableName { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName.Equals("clientcredentials"))
            {
                var tokenRequest = new ClientCredentialsTokenRequest
                {
                    Address = this.AuthorityUri ?? this.DiscoveryDocument.TokenEndpoint,
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
                    Address = this.AuthorityUri ?? this.DiscoveryDocument.TokenEndpoint,
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