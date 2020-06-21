using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Management.Automation;
using Xunit;

namespace IdentityShell.Test.Operation
{
    public class IdentityPersistedGrantCommandTest : IdentityOperationCommandTestBase
    {
        [Fact]
        public void IdentityShell_reads_all_grants()
        {
            // ARRANGE

            var grant = this.ArrangePersistedGrant();

            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityPersistedGrant");

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Equal(grant.Key, result.Property<string>("Key"));
            Assert.Equal(grant.ClientId, result.Property<string>("ClientId"));
            Assert.Equal(grant.CreationTime, result.Property<DateTime>("CreationTime"));
            Assert.Equal(grant.Data, result.Property<string>("Data"));
            Assert.Equal(grant.Expiration, result.Property<DateTime>("Expiration"));
            Assert.Equal(grant.SubjectId, result.Property<string>("SubjectId"));
            Assert.Equal(grant.Type, result.Property<string>("Type"));
        }

        [Fact]
        public void IdentityShell_removes_grant_by_key()
        {
            // ARRANGE

            var grant = this.ArrangePersistedGrant();

            // ACT

            this.PowerShell
                .AddCommand("Remove-IdentityPersistedGrant")
                    .AddParameter("Key", grant.Key);

            this.PowerShell.Invoke();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();
            var existingGrants = this.PowerShell.AddCommand("Get-IdentityPersistedGrant").Invoke().ToArray();

            Assert.Empty(existingGrants);
        }

        [Fact]
        public void IdentityShell_removes_grant_by_pipe()
        {
            // ARRANGE

            var grant = this.ArrangePersistedGrant();

            // ACT

            this.PowerShell
                .AddCommand("Remove-IdentityPersistedGrant");

            this.PowerShell.Invoke(new[] { PSObject.AsPSObject(grant) });

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();
            var existingGrants = this.PowerShell.AddCommand("Get-IdentityPersistedGrant").Invoke().ToArray();

            Assert.Empty(existingGrants);
        }

        private PersistedGrant ArrangePersistedGrant()
        {
            using var dbx = this.serviceProvider.CreateScope().ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

            var model = new PersistedGrant
            {
                ClientId = "clientId",
                Data = "data",
                CreationTime = DateTime.Now,
                Expiration = DateTime.Now.AddDays(1),
                Key = "key",
                SubjectId = "subjectId",
                Type = "type"
            };

            dbx.PersistedGrants.Add(PersistedGrantMappers.ToEntity(model));
            dbx.SaveChanges();

            return model;
        }
    }
}