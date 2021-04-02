using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;

namespace IdentityShell.Configuration
{
    public interface IApiScopeRepository
    {
        IEnumerable<ApiScope> FindApiScopesByName(string[] names);

        IEnumerable<ApiScope> Query(Func<ApiScope, bool> querySpecification = null);

        bool Remove(ApiScope apiScopeEntity);

        void Add(ApiScope apiScopeEntity);
    }
}