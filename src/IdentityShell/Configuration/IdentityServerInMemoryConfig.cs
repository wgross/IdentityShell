// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using System.Collections.Generic;

namespace IdentityShell.Configuration
{
    public sealed class IdentityServerInMemoryConfig
    {
        public IList<IdentityResource> IdentityResources { get; } = new List<IdentityResource>();
        //    new IdentityResource[]
        //    {
        //        new IdentityResources.OpenId(),
        //        new IdentityResources.Profile(),
        //    };

        public IList<ApiScope> ApiScopes { get; } = new List<ApiScope>();
        //{
        //    new ApiScope("scope1"),
        //    new ApiScope("scope2"),
        //};

        public IList<Client> Clients { get; } = new List<Client>();
        //new Client[]

        //{
        //    // m2m client credentials flow client
        //    new Client
        //    {
        //        ClientId = "m2m.client",
        //        ClientName = "Client Credentials Client",

        //        AllowedGrantTypes = GrantTypes.ClientCredentials,
        //        ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

        //        AllowedScopes = { "scope1" }
        //    },

        //    // interactive client using code flow + pkce
        //    new Client
        //    {
        //        ClientId = "interactive",
        //        ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

        //        AllowedGrantTypes = GrantTypes.Code,

        //        RedirectUris = { "https://localhost:44300/signin-oidc" },
        //        FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
        //        PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

        //        AllowOfflineAccess = true,
        //        AllowedScopes = { "openid", "profile", "scope2" }
        //    },
        //};

        public IList<ApiResource> ApiResources { get; } = new List<ApiResource>();

        public IList<PersistedGrant> PersistedGrants { get; } = new List<PersistedGrant>();

        public List<TestUser> TestUsers { get; } = new List<TestUser>();
    }
}