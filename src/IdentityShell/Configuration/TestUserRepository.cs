using Duende.IdentityServer.Test;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace IdentityShell.Configuration
{
    public class TestUserRepository : ITestUserRepository
    {
        private readonly IdentityServerInMemoryConfig config;

        public TestUserRepository(IdentityServerInMemoryConfig config)
        {
            this.config = config;
        }

        public void Add(TestUser user)
        {
            this.config.TestUsers.Add(user);
        }

        public TestUser FindUsername(string username)
        {
            return this.Query(u => u.Username.Equals(username)).FirstOrDefault();
        }

        public IEnumerable<TestUser> Query(Func<TestUser, bool> querySpecification)
        {
            return querySpecification is null
                ? this.config.TestUsers.ToImmutableArray()
                : this.config.TestUsers.Where(querySpecification).ToImmutableArray();
        }

        public bool Remove(TestUser user)
        {
            return this.config.TestUsers.Remove(user);
        }
    }
}