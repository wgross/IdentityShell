using Duende.IdentityServer.Test;
using System;
using System.Collections.Generic;

namespace IdentityShell.Configuration
{
    public interface ITestUserRepository
    {
        IEnumerable<TestUser> Query(Func<TestUser, bool> querySpecification = null);

        void Add(TestUser user);

        TestUser FindUsername(string username);

        bool Remove(TestUser user);
    }
}