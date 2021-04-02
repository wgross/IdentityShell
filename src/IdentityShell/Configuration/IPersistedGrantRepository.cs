using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace IdentityShell.Configuration
{
    public interface IPersistedGrantRepository
    {
        bool Remove(string key);

        IEnumerable<PersistedGrant> Query();
    }
}