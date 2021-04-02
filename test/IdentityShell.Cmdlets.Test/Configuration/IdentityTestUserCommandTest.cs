using Duende.IdentityServer.Test;
using IdentityModel;
using IdentityShell.Commands.Configuration;
using System;
using System.Linq;
using System.Management.Automation;
using System.Security.Claims;
using Xunit;

namespace IdentityShell.Commands.Test.Configuration
{
    [Collection(nameof(IdentityCommandBase.GlobalServiceProvider))]
    public class IdentityTestUserCommandTest : IdentityConfigurationCommandTestBase
    {
        private TestUser ArrangeUser()
        {
            TestUser user = new()
            {
                SubjectId = Guid.NewGuid().ToString(),
                Username = "alice",
                IsActive = true,
                Claims = new Claim[]
                {
                    new(JwtClaimTypes.Name, "Alice Smith"),
                    new(JwtClaimTypes.GivenName, "Alice"),
                    new(JwtClaimTypes.FamilyName, "Smith"),
                    new(JwtClaimTypes.Email, "AliceSmith@email.com"),
                    new(JwtClaimTypes.EmailVerified, true.ToString(), ClaimValueTypes.Boolean),
                    new(JwtClaimTypes.WebSite, "http://alice.com"),
                    new(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", Duende.IdentityServer.IdentityServerConstants.ClaimValueTypes.Json)
                },
                Password = "password",
                ProviderName = "providerName",
                ProviderSubjectId = "providerSubjectId"
            };

            return user;
        }

        private PSObject ArrangeTestUser()
        {
            this.PowerShell
                .AddCommandEx<SetTestUserCommand>(cmd => cmd
                    .AddParameter(c => c.Username, "alice")
                    .AddParameter(c => c.SubjectId, "subjectId")
                    .AddParameter(c => c.Password, "Pass123$")
                    .AddParameter(c => c.ProviderName, "providerName")
                    .AddParameter(c => c.ProviderSubjectId, "providerSubjectId")
                    .AddParameter(c => c.IsActive, true)
                    .AddParameter(c => c.Claims, this.ArrangeUserClaims()));

            var result = this.PowerShell.Invoke().Single();
            this.PowerShell.Commands.Clear();
            return result;
        }

        private Claim[] ArrangeUserClaims() => new Claim[]
        {
            new(JwtClaimTypes.Name, "Alice Smith"),
            new(JwtClaimTypes.GivenName, "Alice"),
            new(JwtClaimTypes.FamilyName, "Smith"),
            new(JwtClaimTypes.Email, "AliceSmith@email.com"),
            new(JwtClaimTypes.EmailVerified, true.ToString(), ClaimValueTypes.Boolean),
            new(JwtClaimTypes.WebSite, "http://alice.com"),
            new(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", Duende.IdentityServer.IdentityServerConstants.ClaimValueTypes.Json)
        };

        private void AssertTestUser(PSObject pso)
        {
            var applicationUser = pso.As<TestUser>();

            Assert.Equal("alice", applicationUser.Username);
            Assert.Equal("subjectId", applicationUser.SubjectId);
            Assert.True(applicationUser.IsActive);
            Assert.Equal("Pass123$", applicationUser.Password);
            Assert.Equal("providerName", applicationUser.ProviderName);
            Assert.Equal("providerSubjectId", applicationUser.ProviderSubjectId);
            Assert.All(this.ArrangeUserClaims(), c =>
            {
                var claim = applicationUser.Claims.Single(cc => cc.Type == c.Type);

                Assert.Equal(c.Value, claim.Value);
                Assert.Equal(c.ValueType, claim.ValueType);
                Assert.Equal(c.Issuer, claim.Issuer);
            });
        }

        [Fact]
        public void IdentityShell_creates_user()
        {
            // ACT
            var result = this.ArrangeTestUser();

            // ASSERT
            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.As<TestUser>();

            this.PowerShell.Commands.Clear();
            this.PowerShell.AddCommandEx<GetTestUserCommand>();

            var resultFromRead = this.PowerShell.Invoke().Single();

            // ASSERT
            Assert.False(this.PowerShell.HadErrors);
            this.AssertTestUser(resultFromRead);
        }

        [Fact]
        public void IdentityShell_reads_user()
        {
            // ARRANGE
            var pso = this.ArrangeTestUser();

            // ACT
            var result = this.PowerShell.AddCommandEx<GetTestUserCommand>().Invoke().Single();

            // ASSERT
            Assert.False(this.PowerShell.HadErrors);
            this.AssertTestUser(result);
        }

        [Fact]
        public void IdentityShell_reads_user_by_UserName()
        {
            // ARRANGE
            var pso = this.ArrangeTestUser();

            // ACT
            var result = this.PowerShell
                .AddCommandEx<GetTestUserCommand>(cmd => cmd.AddParameter(c => c.Username, "alice"))
                .Invoke().Single();

            // ASSERT
            Assert.False(this.PowerShell.HadErrors);
            this.AssertTestUser(result);
        }

        [Fact]
        public void IdentityShell_modifies_user()
        {
            // ARRANGE
            var arranged = this.ArrangeTestUser();

            // ACT
            var result = this.PowerShell
                .AddCommandEx<SetTestUserCommand>(cmd =>
                {
                    cmd.AddParameter(c => c.Password, "Pass123$-changed");
                    cmd.AddParameter(c => c.Username, "alice");
                })
                .Invoke()
                .Single();

            // ASSERT1
            Assert.False(this.PowerShell.HadErrors);
            Assert.Equal(arranged.As<TestUser>().Password, result.As<TestUser>().Password);
        }

        [Fact]
        public void IdentityShell_removes_user()
        {
            // ARRANGE
            this.ArrangeTestUser();

            // ACT
            this.PowerShell
                .AddCommandEx<RemoveTestUserCommand>(cmd => cmd.AddParameter(c => c.Username, "alice"))
                .Invoke();

            // ASSERT
            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();
            Assert.Empty(this.PowerShell.AddCommandEx<GetTestUserCommand>().Invoke().ToArray());
        }

        [Fact]
        public void IdentityShell_removes_user_from_pipe()
        {
            // ARRANGE
            var pso = this.ArrangeTestUser();

            // ACT
            this.PowerShell
                .AddCommandEx<RemoveTestUserCommand>()
                .Invoke(Array(pso));

            // ASSERT
            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();
            Assert.Empty(this.PowerShell.AddCommandEx<GetTestUserCommand>().Invoke().ToArray());
        }
    }
}