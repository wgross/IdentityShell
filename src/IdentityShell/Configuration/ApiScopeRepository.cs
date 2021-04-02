using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace IdentityShell.Configuration
{
    public class ApiScopeRepository : IApiScopeRepository
    {
        private readonly IdentityServerInMemoryConfig config;

        public ApiScopeRepository(IdentityServerInMemoryConfig config)
        {
            this.config = config;
        }

        public void Add(ApiScope apiScope)
        {
            this.config.ApiScopes.Add(apiScope);
        }

        public IEnumerable<ApiScope> FindApiScopesByName(string[] names)
        {
            return this.Query(s => names.Contains(s.Name, StringComparer.OrdinalIgnoreCase));
        }

        public IEnumerable<ApiScope> Query(Func<ApiScope, bool> querySpecification = null)
        {
            return querySpecification is null
                ? this.config.ApiScopes.ToImmutableArray()
                : this.config.ApiScopes.Where(querySpecification).ToImmutableArray();
        }

        public bool Remove(ApiScope apiScope)
        {
            return this.config.ApiScopes.Remove(apiScope);
        }
    }
}