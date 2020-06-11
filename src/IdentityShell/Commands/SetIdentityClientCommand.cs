using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Claims;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "IdentityClient")]
    public class SetIdentityClientCommand : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string ClientId { get; set; }

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
        public ICollection<string> IdentityProviderRestrictions { get; set; }

        [Parameter]
        public bool IncludeJwtId { get; set; }

        [Parameter]
        public ICollection<Claim> Claims { get; set; }

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
        public ICollection<string> AllowedScopes { get; set; }

        [Parameter]
        public IDictionary<string, string> Properties { get; set; }

        [Parameter]
        public bool BackChannelLogoutSessionRequired { get; set; }

        [Parameter]
        public bool Enabled { get; set; }

        [Parameter]
        public string ProtocolType { get; set; }

        [Parameter]
        public ICollection<Secret> ClientSecrets { get; set; }

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
        public ICollection<string> AllowedCorsOrigins { get; set; }

        [Parameter]
        public bool RequireConsent { get; set; }

        [Parameter]
        public ICollection<string> AllowedGrantTypes { get; set; }

        [Parameter]
        public bool RequirePkce { get; set; }

        [Parameter]
        public bool AllowPlainTextPkce { get; set; }

        [Parameter]
        public bool AllowAccessTokensViaBrowser { get; set; }

        [Parameter]
        public ICollection<string> RedirectUris { get; set; }

        [Parameter]
        public ICollection<string> PostLogoutRedirectUris { get; set; }

        [Parameter]
        public string FrontChannelLogoutUri { get; set; }

        [Parameter]
        public bool FrontChannelLogoutSessionRequired { get; set; }

        [Parameter]
        public string BackChannelLogoutUri { get; set; }

        [Parameter]
        public bool AllowRememberConsent { get; set; }

        protected override void ProcessRecord()
        {
            using var serviceScope = Startup.AppServices.GetService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            var client = context.Clients.SingleOrDefault(c => c.ClientId == this.ClientId);
            if (client is null)
            {
                client = new IdentityServer4.EntityFramework.Entities.Client();
                context.Clients.Add(client);
            }

            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AllowOfflineAccess)))
            {
                client.AllowOfflineAccess = this.AllowOfflineAccess;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(IdentityTokenLifetime)))
            {
                client.IdentityTokenLifetime = this.IdentityTokenLifetime;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AccessTokenLifetime)))
            {
                client.AccessTokenLifetime = this.AccessTokenLifetime;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AuthorizationCodeLifetime)))
            {
                client.AuthorizationCodeLifetime = this.AuthorizationCodeLifetime;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AbsoluteRefreshTokenLifetime)))
            {
                client.AbsoluteRefreshTokenLifetime = this.AbsoluteRefreshTokenLifetime;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(SlidingRefreshTokenLifetime)))
            {
                client.SlidingRefreshTokenLifetime = this.SlidingRefreshTokenLifetime;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ConsentLifetime)))
            {
                client.ConsentLifetime = this.ConsentLifetime;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(RefreshTokenUsage)))
            {
                client.RefreshTokenUsage = (int)this.RefreshTokenUsage;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(UpdateAccessTokenClaimsOnRefresh)))
            {
                client.UpdateAccessTokenClaimsOnRefresh = this.UpdateAccessTokenClaimsOnRefresh;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(RefreshTokenExpiration)))
            {
                client.RefreshTokenExpiration = (int)this.RefreshTokenExpiration;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AccessTokenType)))
            {
                client.AccessTokenType = (int)this.AccessTokenType;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(EnableLocalLogin)))
            {
                client.EnableLocalLogin = this.EnableLocalLogin;
            }
            //if (this.MyInvocation.BoundParameters.ContainsKey(nameof(IdentityProviderRestrictions)))
            //{
            //    client.IdentityProviderRestrictions = this.IdentityProviderRestrictions.ToList();
            //}
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(IncludeJwtId)))
            {
                client.IncludeJwtId = this.IncludeJwtId;
            }
            //if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Claims)))
            //{
            //    client.Claims = this.Claims;
            //}
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AlwaysSendClientClaims)))
            {
                client.AlwaysSendClientClaims = this.AlwaysSendClientClaims;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ClientClaimsPrefix)))
            {
                client.ClientClaimsPrefix = this.ClientClaimsPrefix;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(PairWiseSubjectSalt)))
            {
                client.PairWiseSubjectSalt = this.PairWiseSubjectSalt;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(UserSsoLifetime)))
            {
                client.UserSsoLifetime = this.UserSsoLifetime;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(UserCodeType)))
            {
                client.UserCodeType = this.UserCodeType;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(DeviceCodeLifetime)))
            {
                client.DeviceCodeLifetime = this.DeviceCodeLifetime;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AlwaysIncludeUserClaimsInIdToken)))
            {
                client.AlwaysIncludeUserClaimsInIdToken = this.AlwaysIncludeUserClaimsInIdToken;
            }
            //if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AllowedScopes)))
            //{
            //    client.AllowedScopes = this.AllowedScopes;
            //}
            //if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Properties)))
            //{
            //    client.Properties = this.Properties;
            //}
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(BackChannelLogoutSessionRequired)))
            {
                client.BackChannelLogoutSessionRequired = this.BackChannelLogoutSessionRequired;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Enabled)))
            {
                client.Enabled = this.Enabled;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ClientId)))
            {
                client.ClientId = this.ClientId;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ProtocolType)))
            {
                client.ProtocolType = this.ProtocolType;
            }
            //if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ClientSecrets)))
            //{
            //    client.ClientSecrets = this.ClientSecrets;
            //}
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(RequireClientSecret)))
            {
                client.RequireClientSecret = this.RequireClientSecret;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ClientName)))
            {
                client.ClientName = this.ClientName;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Description)))
            {
                client.Description = this.Description;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ClientUri)))
            {
                client.ClientUri = this.ClientUri;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(LogoUri)))
            {
                client.LogoUri = this.LogoUri;
            }
            //if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AllowedCorsOrigins)))
            //{
            //    client.AllowedCorsOrigins = this.AllowedCorsOrigins;
            //}
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(RequireConsent)))
            {
                client.RequireConsent = this.RequireConsent;
            }
            //if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AllowedGrantTypes)))
            //{
            //    client.AllowedGrantTypes = this.AllowedGrantTypes;
            //}
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(RequirePkce)))
            {
                client.RequirePkce = this.RequirePkce;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AllowPlainTextPkce)))
            {
                client.AllowPlainTextPkce = this.AllowPlainTextPkce;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AllowAccessTokensViaBrowser)))
            {
                client.AllowAccessTokensViaBrowser = this.AllowAccessTokensViaBrowser;
            }
            //if (this.MyInvocation.BoundParameters.ContainsKey(nameof(RedirectUris)))
            //{
            //    client.RedirectUris = this.RedirectUris;
            //}
            //if (this.MyInvocation.BoundParameters.ContainsKey(nameof(PostLogoutRedirectUris)))
            //{
            //    client.PostLogoutRedirectUris = this.PostLogoutRedirectUris;
            //}
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(FrontChannelLogoutUri)))
            {
                client.FrontChannelLogoutUri = this.FrontChannelLogoutUri;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(FrontChannelLogoutSessionRequired)))
            {
                client.FrontChannelLogoutSessionRequired = this.FrontChannelLogoutSessionRequired;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(BackChannelLogoutUri)))
            {
                client.BackChannelLogoutUri = this.BackChannelLogoutUri;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AllowRememberConsent)))
            {
                client.AllowRememberConsent = this.AllowRememberConsent;
            }

            context.SaveChanges();
            this.WriteObject(client.ToModel());
        }
    }
}