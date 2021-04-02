using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;

namespace IdentityShell.Configuration
{
    public interface IApiResourceRepository
    {
        IEnumerable<ApiResource> FindApiResourcesByName(string[] vs);

        IEnumerable<ApiResource> Query(Func<ApiResource, bool> querySepcification = null);

        IEnumerable<ApiScope> FindApiScopesByName(string[] vs);

        void Add(ApiResource apiEntity);

        bool Remove(ApiResource apiEntity);
    }
}