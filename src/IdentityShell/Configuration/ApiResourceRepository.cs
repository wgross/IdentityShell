using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace IdentityShell.Configuration
{
    public class ApiResourceRepository : IApiResourceRepository
    {
        private readonly IdentityServerInMemoryConfig config;

        public ApiResourceRepository(IdentityServerInMemoryConfig config)
        {
            this.config = config;
        }

        public void Add(ApiResource apiResource)
        {
            this.config.ApiResources.Add(apiResource);
        }

        public IEnumerable<ApiResource> FindApiResourcesByName(string[] names)
        {
            return Query(ar => names.Contains(ar.Name, StringComparer.OrdinalIgnoreCase));
        }

        public IEnumerable<ApiScope> FindApiScopesByName(string[] names)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ApiResource> Query(Func<ApiResource, bool> querySepcification)
        {
            return querySepcification is null
                ? this.config.ApiResources.ToImmutableArray()
                : this.config.ApiResources.Where(querySepcification).ToImmutableArray();
        }

        public bool Remove(ApiResource apiResource)
        {
            return this.config.ApiResources.Remove(apiResource);
        }
    }
}