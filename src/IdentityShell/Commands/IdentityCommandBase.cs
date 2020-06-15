using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    public abstract class IdentityCommandBase : PSCmdlet
    {
        public static IServiceProvider ServiceProvider { protected get; set; }

        public IServiceScope ServiceProviderScope => ServiceProvider.CreateScope();

        private ConfigurationDbContext context = null;

        protected ConfigurationDbContext Context => this.context ??= ServiceProviderScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

        protected IQueryable<IdentityServer4.EntityFramework.Entities.Client> QueryClients()
        {
            var query = Context.Clients.AsQueryable();

            query.Include(x => x.AllowedCorsOrigins).SelectMany(c => c.AllowedCorsOrigins).Load();
            query.Include(x => x.AllowedGrantTypes).SelectMany(c => c.AllowedGrantTypes).Load();
            query.Include(x => x.AllowedScopes).SelectMany(c => c.AllowedScopes).Load();
            query.Include(x => x.Claims).SelectMany(c => c.Claims).Load();
            query.Include(x => x.ClientSecrets).SelectMany(c => c.ClientSecrets).Load();
            query.Include(x => x.IdentityProviderRestrictions).SelectMany(c => c.IdentityProviderRestrictions).Load();
            query.Include(x => x.PostLogoutRedirectUris).SelectMany(c => c.PostLogoutRedirectUris).Load();
            query.Include(x => x.Properties).SelectMany(c => c.Properties).Load();
            query.Include(x => x.RedirectUris).SelectMany(c => c.RedirectUris).Load();

            return query;
        }

        protected IQueryable<IdentityServer4.EntityFramework.Entities.IdentityResource> QueryIdentityResource()
        {
            return Context.IdentityResources
                .AsQueryable()
                .Include(x => x.UserClaims)
                .Include(x => x.Properties);
        }

        protected IQueryable<IdentityServer4.EntityFramework.Entities.ApiResource> QueryApiResource()
        {
            return Context.ApiResources
                .AsQueryable()
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .Include(x => x.Secrets)
                .Include(x => x.Scopes);
        }
    }
}