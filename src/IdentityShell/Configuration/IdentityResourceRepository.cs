using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace IdentityShell.Configuration
{
    public class IdentityResourceRepository : IIdentityResourceRepository
    {
        private readonly IdentityServerInMemoryConfig config;

        public IdentityResourceRepository(IdentityServerInMemoryConfig config)
        {
            this.config = config;
        }

        public void Add(IdentityResource identityResource)
        {
            this.config.IdentityResources.Add(identityResource);
        }

        public IEnumerable<IdentityResource> GetAll()
        {
            return this.Query();
        }

        public IEnumerable<IdentityResource> Query(Func<IdentityResource, bool> querySpecification = null)
        {
            return querySpecification is null
                ? this.config.IdentityResources.ToImmutableArray()
                : this.config.IdentityResources.Where(querySpecification).ToImmutableArray();
        }

        public bool Remove(IdentityResource identityResource)
        {
            return this.config.IdentityResources.Remove(identityResource);
        }
    }
}