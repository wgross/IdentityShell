using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using System;
using System.Collections.Generic;

namespace IdentityShell.Configuration
{
    public interface IIdentityResourceRepository
    {
        IEnumerable<IdentityResource> GetAll();

        IEnumerable<IdentityResource> Query(Func<IdentityResource, bool> querySpecification = null);

        bool Remove(IdentityResource identityEntity);

        void Add(IdentityResource identityEntity);

    }
}