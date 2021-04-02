using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace IdentityShell.Configuration
{
    public class ClientRepository : IClientRepository
    {
        private readonly IdentityServerInMemoryConfig config;

        public ClientRepository(IdentityServerInMemoryConfig config)
        {
            this.config = config;
        }

        public void Add(Client client)
        {
            this.config.Clients.Add(client);
        }

        public Client FindClientById(string clientId)
        {
            return this.Query(c => c.ClientId.Equals(clientId)).FirstOrDefault();
        }

        public IEnumerable<Client> Query(Func<Client, bool> querySpecification = null)
        {
            return querySpecification is null
                ? this.config.Clients.ToImmutableArray()
                : this.config.Clients.Where(querySpecification).ToImmutableArray();
        }

        public bool Remove(Client client)
        {
            return this.config.Clients.Remove(client);
        }
    }
}