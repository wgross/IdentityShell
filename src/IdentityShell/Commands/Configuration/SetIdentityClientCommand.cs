using Duende.IdentityServer.Models;
using IdentityShell.Commands.Configuration.ArgumentCompleters;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Set, "IdentityClient")]
    [CmdletBinding()]
    [OutputType(typeof(Client))]
    public class SetIdentityClientCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ArgumentCompleter(typeof(IdentityClientIdCompleter))]
        public string ClientId { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public Client InputObject { get; set; }

        #region Parameter

        [Parameter]
        public bool AllowOfflineAccess { get; set; }

        [Parameter]
        public int IdentityTokenLifetime { get; set; }

        [Parameter]
        public int AccessTokenLifetime { get; set; }

        [Parameter]
        public int AuthorizationCodeLifetime { get; set; }

        [Parameter]
        public int AbsoluteRefreshTokenLifetime { get; set; }

        [Parameter]
        public int SlidingRefreshTokenLifetime { get; set; }

        [Parameter]
        public int? ConsentLifetime { get; set; }

        [Parameter]
        public TokenUsage RefreshTokenUsage { get; set; }

        [Parameter]
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        [Parameter]
        public TokenExpiration RefreshTokenExpiration { get; set; }

        [Parameter]
        public AccessTokenType AccessTokenType { get; set; }

        [Parameter]
        public bool EnableLocalLogin { get; set; }

        [Parameter]
        public object[] IdentityProviderRestrictions { get; set; }

        [Parameter]
        public bool IncludeJwtId { get; set; }

        [Parameter]
        public object[] Claims { get; set; }

        [Parameter]
        public bool AlwaysSendClientClaims { get; set; }

        [Parameter]
        public string ClientClaimsPrefix { get; set; }

        [Parameter]
        public string PairWiseSubjectSalt { get; set; }

        [Parameter]
        public int? UserSsoLifetime { get; set; }

        [Parameter]
        public string UserCodeType { get; set; }

        [Parameter]
        public int DeviceCodeLifetime { get; set; }

        [Parameter]
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(IdentityApiScopeNameCompleter))]
        public object[] AllowedScopes { get; set; }

        [Parameter]
        public Hashtable Properties { get; set; }

        [Parameter]
        public bool BackChannelLogoutSessionRequired { get; set; }

        [Parameter]
        public bool Enabled { get; set; }

        [Parameter]
        public string ProtocolType { get; set; }

        [Parameter]
        public object[] ClientSecrets { get; set; }

        [Parameter]
        public bool RequireClientSecret { get; set; }

        [Parameter]
        public string ClientName { get; set; }

        [Parameter]
        public string Description { get; set; }

        [Parameter]
        public string ClientUri { get; set; }

        [Parameter]
        public string LogoUri { get; set; }

        [Parameter]
        public object[] AllowedCorsOrigins { get; set; }

        [Parameter]
        public bool RequireConsent { get; set; }

        [Parameter]
        [ValidateSet(
            GrantType.AuthorizationCode,
            GrantType.ClientCredentials,
            GrantType.DeviceFlow,
            GrantType.Hybrid,
            GrantType.Implicit,
            GrantType.ResourceOwnerPassword
        )]
        public object[] AllowedGrantTypes { get; set; }

        [Parameter]
        public bool RequirePkce { get; set; }

        [Parameter]
        public bool AllowPlainTextPkce { get; set; }

        [Parameter]
        public bool AllowAccessTokensViaBrowser { get; set; }

        [Parameter]
        public object[] RedirectUris { get; set; }

        [Parameter]
        public object[] PostLogoutRedirectUris { get; set; }

        [Parameter]
        public string FrontChannelLogoutUri { get; set; }

        [Parameter]
        public bool FrontChannelLogoutSessionRequired { get; set; }

        [Parameter]
        public string BackChannelLogoutUri { get; set; }

        [Parameter]
        public bool AllowRememberConsent { get; set; }

        #endregion Parameter

        protected override void ProcessRecord()
        {
            var client = this.InputObject;
            var existingClient = this.LocalServiceProvider
                .GetRequiredService<IClientRepository>()
                .Query(c => c.ClientId == this.ClientId)
                .FirstOrDefault();

            if (client is null && existingClient is null)
            {
                client = this.SetBoundParameters(new());
                this.LocalServiceProvider
                    .GetRequiredService<IClientRepository>()
                    .Add(client);
            }
            else if (client is not null && existingClient is null)
            {
                this.LocalServiceProvider
                    .GetRequiredService<IClientRepository>()
                    .Add(this.SetBoundParameters(client));
            }
            else
            {
                client = this.SetBoundParameters(existingClient);
            }
            this.WriteObject(client);
        }

        private Client SetBoundParameters(Client client)
        {
            if (this.IsParameterBound(nameof(this.AllowOfflineAccess)))
            {
                client.AllowOfflineAccess = this.AllowOfflineAccess;
            }
            if (this.IsParameterBound(nameof(this.IdentityTokenLifetime)))
            {
                client.IdentityTokenLifetime = this.IdentityTokenLifetime;
            }
            if (this.IsParameterBound(nameof(this.AccessTokenLifetime)))
            {
                client.AccessTokenLifetime = this.AccessTokenLifetime;
            }
            if (this.IsParameterBound(nameof(this.AuthorizationCodeLifetime)))
            {
                client.AuthorizationCodeLifetime = this.AuthorizationCodeLifetime;
            }
            if (this.IsParameterBound(nameof(this.AbsoluteRefreshTokenLifetime)))
            {
                client.AbsoluteRefreshTokenLifetime = this.AbsoluteRefreshTokenLifetime;
            }
            if (this.IsParameterBound(nameof(this.SlidingRefreshTokenLifetime)))
            {
                client.SlidingRefreshTokenLifetime = this.SlidingRefreshTokenLifetime;
            }
            if (this.IsParameterBound(nameof(this.ConsentLifetime)))
            {
                client.ConsentLifetime = this.ConsentLifetime;
            }
            if (this.IsParameterBound(nameof(this.RefreshTokenUsage)))
            {
                client.RefreshTokenUsage = this.RefreshTokenUsage;
            }
            if (this.IsParameterBound(nameof(this.UpdateAccessTokenClaimsOnRefresh)))
            {
                client.UpdateAccessTokenClaimsOnRefresh = this.UpdateAccessTokenClaimsOnRefresh;
            }
            if (this.IsParameterBound(nameof(this.RefreshTokenExpiration)))
            {
                client.RefreshTokenExpiration = this.RefreshTokenExpiration;
            }
            if (this.IsParameterBound(nameof(this.AccessTokenType)))
            {
                client.AccessTokenType = this.AccessTokenType;
            }
            if (this.IsParameterBound(nameof(this.EnableLocalLogin)))
            {
                client.EnableLocalLogin = this.EnableLocalLogin;
            }
            if (this.IsParameterBound(nameof(this.IdentityProviderRestrictions)))
            {
                client.IdentityProviderRestrictions = Collection(this.IdentityProviderRestrictions);
            }
            if (this.IsParameterBound(nameof(this.IncludeJwtId)))
            {
                client.IncludeJwtId = this.IncludeJwtId;
            }
            if (this.IsParameterBound(nameof(this.Claims)))
            {
                client.Claims = this.Claims.Select(c => PSArgumentValue<ClientClaim>(c)).ToArray();
            }
            if (this.IsParameterBound(nameof(this.AlwaysSendClientClaims)))
            {
                client.AlwaysSendClientClaims = this.AlwaysSendClientClaims;
            }
            if (this.IsParameterBound(nameof(this.ClientClaimsPrefix)))
            {
                client.ClientClaimsPrefix = this.ClientClaimsPrefix;
            }
            if (this.IsParameterBound(nameof(this.PairWiseSubjectSalt)))
            {
                client.PairWiseSubjectSalt = this.PairWiseSubjectSalt;
            }
            if (this.IsParameterBound(nameof(this.UserSsoLifetime)))
            {
                client.UserSsoLifetime = this.UserSsoLifetime;
            }
            if (this.IsParameterBound(nameof(this.UserCodeType)))
            {
                client.UserCodeType = this.UserCodeType;
            }
            if (this.IsParameterBound(nameof(this.DeviceCodeLifetime)))
            {
                client.DeviceCodeLifetime = this.DeviceCodeLifetime;
            }
            if (this.IsParameterBound(nameof(this.AlwaysIncludeUserClaimsInIdToken)))
            {
                client.AlwaysIncludeUserClaimsInIdToken = this.AlwaysIncludeUserClaimsInIdToken;
            }
            if (this.IsParameterBound(nameof(this.AllowedScopes)))
            {
                client.AllowedScopes = Collection(this.AllowedScopes);
            }
            if (this.IsParameterBound(nameof(this.Properties)))
            {
                client.Properties = this.ToDictionary(this.Properties);
            }
            if (this.IsParameterBound(nameof(this.BackChannelLogoutSessionRequired)))
            {
                client.BackChannelLogoutSessionRequired = this.BackChannelLogoutSessionRequired;
            }
            if (this.IsParameterBound(nameof(this.Enabled)))
            {
                client.Enabled = this.Enabled;
            }
            if (this.IsParameterBound(nameof(this.ClientId)))
            {
                client.ClientId = this.ClientId;
            }
            if (this.IsParameterBound(nameof(this.ProtocolType)))
            {
                client.ProtocolType = this.ProtocolType;
            }
            if (this.IsParameterBound(nameof(this.ClientSecrets)))
            {
                client.ClientSecrets = this.ClientSecrets.Select(cs => PSArgumentValue<Secret>(cs)).ToArray();
            }
            if (this.IsParameterBound(nameof(this.RequireClientSecret)))
            {
                client.RequireClientSecret = this.RequireClientSecret;
            }
            if (this.IsParameterBound(nameof(this.ClientName)))
            {
                client.ClientName = this.ClientName;
            }
            if (this.IsParameterBound(nameof(this.Description)))
            {
                client.Description = this.Description;
            }
            if (this.IsParameterBound(nameof(this.ClientUri)))
            {
                client.ClientUri = this.ClientUri;
            }
            if (this.IsParameterBound(nameof(this.LogoUri)))
            {
                client.LogoUri = this.LogoUri;
            }
            if (this.IsParameterBound(nameof(this.AllowedCorsOrigins)))
            {
                client.AllowedCorsOrigins = Collection(this.AllowedCorsOrigins);
            }
            if (this.IsParameterBound(nameof(this.RequireConsent)))
            {
                client.RequireConsent = this.RequireConsent;
            }
            if (this.IsParameterBound(nameof(this.AllowedGrantTypes)))
            {
                client.AllowedGrantTypes = Collection(this.AllowedGrantTypes).Select(GrantTypeValue).ToList();
            }
            if (this.IsParameterBound(nameof(this.RequirePkce)))
            {
                client.RequirePkce = this.RequirePkce;
            }
            if (this.IsParameterBound(nameof(this.AllowPlainTextPkce)))
            {
                client.AllowPlainTextPkce = this.AllowPlainTextPkce;
            }
            if (this.IsParameterBound(nameof(this.AllowAccessTokensViaBrowser)))
            {
                client.AllowAccessTokensViaBrowser = this.AllowAccessTokensViaBrowser;
            }
            if (this.IsParameterBound(nameof(this.RedirectUris)))
            {
                client.RedirectUris = Collection(this.RedirectUris);
            }
            if (this.IsParameterBound(nameof(this.PostLogoutRedirectUris)))
            {
                client.PostLogoutRedirectUris = Collection(this.PostLogoutRedirectUris);
            }
            if (this.IsParameterBound(nameof(this.FrontChannelLogoutUri)))
            {
                client.FrontChannelLogoutUri = this.FrontChannelLogoutUri;
            }
            if (this.IsParameterBound(nameof(this.FrontChannelLogoutSessionRequired)))
            {
                client.FrontChannelLogoutSessionRequired = this.FrontChannelLogoutSessionRequired;
            }
            if (this.IsParameterBound(nameof(this.BackChannelLogoutUri)))
            {
                client.BackChannelLogoutUri = this.BackChannelLogoutUri;
            }
            if (this.IsParameterBound(nameof(this.AllowRememberConsent)))
            {
                client.AllowRememberConsent = this.AllowRememberConsent;
            }

            return client;
        }

        private string GrantTypeValue(string argumentValue)
        {
            // first the property with the value
            var grantTypeName = typeof(GrantType)
                .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .FirstOrDefault(m => m.Name == argumentValue);

            if (grantTypeName is { })
                return (string)grantTypeName.GetValue(null);

            // maybe value is given literally
            var literalGrantType = typeof(GrantType)
                .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Select(p => (string)p.GetValue(null))
                .FirstOrDefault(pv => pv.Equals(argumentValue));

            if (string.IsNullOrEmpty(literalGrantType))
                this.WriteError(new ErrorRecord(
                    new PSArgumentException("Grant type '{value}' is unknown"), "GrantType.Invalid", ErrorCategory.InvalidArgument, argumentValue));

            return literalGrantType;
        }
    }
}