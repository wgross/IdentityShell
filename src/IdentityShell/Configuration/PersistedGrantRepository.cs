using Duende.IdentityServer.Models;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace IdentityShell.Configuration
{
    public class PersistedGrantRepository : IPersistedGrantRepository
    {
        private readonly IdentityServerInMemoryConfig config;

        public PersistedGrantRepository(IdentityServerInMemoryConfig config)
        {
            this.config = config;
        }

        public IEnumerable<PersistedGrant> Query()
        {
            return this.config.PersistedGrants.ToImmutableArray();
        }

        public bool Remove(string key)
        {
            return true;
            // return this.config.PersistedGrants.Remove(key);
        }
    }
}