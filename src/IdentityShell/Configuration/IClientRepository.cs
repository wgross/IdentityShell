using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;

namespace IdentityShell.Configuration
{
    public interface IClientRepository
    {
        Client FindClientById(string clientId);

        IEnumerable<Client> Query(Func<Client, bool> querySpecification = null);

        bool Remove(Client client);

        void Add(Client client);
    }
}