using IdentityModel;
using IdentityServerAspNetIdentity.Models;
using IdentityShell.Cmdlets;
using IdentityShell.Cmdlets.AspNetIdentity;
using System;
using System.Linq;
using System.Management.Automation;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace IdentityShell.Test.AspNetIdentity
{
    public class AspNetIdentityUserTest : AspNetIdentityCommandTestBase
    {
        private async Task<(ApplicationUser user, Claim[] claims)> ArrangeUser()
        {
            var userMgr = this.UserManager;

            var user = new ApplicationUser
            {
                UserName = "alice"
            };

            await userMgr.CreateAsync(user, "Pass123$");

            var claims = new Claim[]
            {
                new Claim(JwtClaimTypes.Name, "Alice Smith"),
                new Claim(JwtClaimTypes.GivenName, "Alice"),
                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                new Claim(JwtClaimTypes.EmailVerified, true.ToString(), ClaimValueTypes.Boolean),
                new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
            };

            await userMgr.AddClaimsAsync(user, claims);

            return (user, claims);
        }

        private PSObject ArrangeAspNetIdentityUser()
        {
            this.PowerShell
                .AddCommandEx<SetAspNetIdentityUserCommand>(cmd => cmd
                    .AddParameter(c => c.UserName, "alice")
                    .AddParameter(c => c.NewPassword, "Pass123$")
                    .AddParameter(c => c.PhoneNumberConfirmed, true)
                    .AddParameter(c => c.PhoneNumber, "123")
                    .AddParameter(c => c.TwoFactorEnabled, true)
                    .AddParameter(c => c.AccessFailedCount, 1)
                    .AddParameter(c => c.Email, "email")
                    .AddParameter(c => c.EmailConfirmed, true)
                    .AddParameter(c => c.LockoutEnabled, true)
                    .AddParameter(c => c.LockoutEnd, DateTimeOffset.Now));

            var result = this.PowerShell.Invoke().Single();
            this.PowerShell.Commands.Clear();
            return result;
        }

        private static void AssertAspNetIdentityUser(PSObject pso)
        {
            var applicationUser = pso.As<ApplicationUser>();

            Assert.Equal("alice", applicationUser.UserName);
            Assert.Equal("email", applicationUser.Email);
            Assert.Equal(1, applicationUser.AccessFailedCount);
            Assert.True(applicationUser.EmailConfirmed);
            Assert.True(applicationUser.LockoutEnabled);
            Assert.NotNull(applicationUser.LockoutEnd);
            Assert.Equal("EMAIL", applicationUser.NormalizedEmail);
            Assert.Equal("ALICE", applicationUser.NormalizedUserName);
            Assert.NotNull(applicationUser.PasswordHash);
            Assert.Equal("123", applicationUser.PhoneNumber);
            Assert.True(applicationUser.PhoneNumberConfirmed);
            Assert.NotNull(applicationUser.SecurityStamp);
            Assert.True(applicationUser.TwoFactorEnabled);
        }

        [Fact]
        public void IdentityShell_creates_user()
        {
            // ARRANGE

            var result = ArrangeAspNetIdentityUser();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.As<ApplicationUser>();

            this.PowerShell.Commands.Clear();
            this.PowerShell.AddCommandEx<GetAspNetIdentityUserCommand>();

            var resultFromRead = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            AssertAspNetIdentityUser(resultFromRead);
        }

        [Fact]
        public void IdentityShell_reads_user()
        {
            // ARRANGE

            var pso = this.ArrangeAspNetIdentityUser();

            // ACT

            var result = this.PowerShell.AddCommandEx<GetAspNetIdentityUserCommand>().Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            AssertAspNetIdentityUser(result);
        }

        [Fact]
        public void IdentityShell_reads_user_by_UserName()
        {
            // ARRANGE

            var pso = this.ArrangeAspNetIdentityUser();

            // ACT

            var result = this.PowerShell
                .AddCommandEx<GetAspNetIdentityUserCommand>(cmd => cmd.AddParameter(c => c.UserName, "alice"))
                .Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            AssertAspNetIdentityUser(result);
        }

        [Fact]
        public void IdentityShell_modifies_user()
        {
            // ARRANGE

            var arranged = this.ArrangeAspNetIdentityUser();

            // ACT

            var result = this.PowerShell
                .AddCommandEx<SetAspNetIdentityUserCommand>(cmd =>
                {
                    cmd.AddParameter(c => c.CurrentPassword, "Pass123$");
                    cmd.AddParameter(c => c.NewPassword, "Pass123$-changed");
                    cmd.AddParameter(c => c.UserName, "alice");
                })
                .Invoke()
                .Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.NotEqual(arranged.As<ApplicationUser>().PasswordHash, result.As<ApplicationUser>().PasswordHash);
        }

        [Fact]
        public void IdentityShell_removes_user()
        {
            // ARRANGE

            this.ArrangeAspNetIdentityUser();

            // ACT

            this.PowerShell
                .AddCommandEx<RemoveAspNetIdentityUserCommand>(cmd => cmd.AddParameter(c => c.UserName, "alice"))
                .Invoke();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();
            Assert.Empty(this.PowerShell.AddCommandEx<GetAspNetIdentityUserCommand>().Invoke().ToArray());
        }

        [Fact]
        public void IdentityShell_removes_user_from_pipe()
        {
            // ARRANGE

            var pso = this.ArrangeAspNetIdentityUser();

            // ACT

            this.PowerShell
                .AddCommandEx<RemoveAspNetIdentityUserCommand>()
                .Invoke(Array(pso));

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();
            Assert.Empty(this.PowerShell.AddCommandEx<GetAspNetIdentityUserCommand>().Invoke().ToArray());
        }

        [Fact]
        public void IdentityShell_creates_user_claim()
        {
            // ARRANGE

            var arranged = ArrangeAspNetIdentityUser();

            // ACT

            var claim = this.PowerShell
                .AddCommandEx<NewClaimCommand>(c => c
                    .AddParameter(c => c.Type, ClaimTypes.Email)
                    .AddParameter(c => c.ValueType, ClaimValueTypes.Email)
                    .AddParameter(c => c.Value, "changed-email"))
                .Invoke().Single();
            this.PowerShell.Commands.Clear();

            // ACT

            this.PowerShell
                .AddCommandEx<SetAspNetIdentityUserClaimCommand>(c => c
                    .AddParameter(c => c.UserName, "alice"))
                    .Invoke(new PSObject[] { claim });

            // ASSERT

            this.PowerShell.Commands.Clear();
            var resultFromRead = this.PowerShell
                .AddCommandEx<GetAspNetIdentityUserClaimCommand>(c => c
                    .AddParameter(c => c.UserName, "alice"))
                .Invoke()
                .ToArray();

            var modifiedClaim = resultFromRead.Select(pso => (Claim)pso.ImmediateBaseObject).Single(c => c.Type.Equals(ClaimTypes.Email));

            Assert.Equal(ClaimValueTypes.String, modifiedClaim.ValueType);
            Assert.Equal("changed-email", modifiedClaim.Value);
        }

        [Fact]
        public void IdentityShell_modifies_user_claims()
        {
            // ARRANGE

            var arranged = ArrangeAspNetIdentityUser();

            var claim = this.PowerShell
                .AddCommandEx<NewClaimCommand>(c => c
                    .AddParameter(c => c.Type, ClaimTypes.Email)
                    .AddParameter(c => c.ValueType, ClaimValueTypes.Email)
                    .AddParameter(c => c.Value, "changed-email"))
                .Invoke().Single();
            this.PowerShell.Commands.Clear();

            // ACT

            this.PowerShell
                .AddCommandEx<SetAspNetIdentityUserClaimCommand>(c => c
                    .AddParameter(c => c.UserName, "alice"))
                    .Invoke(new PSObject[] { claim });

            // ASSERT

            this.PowerShell.Commands.Clear();
            var resultFromRead = this.PowerShell
                .AddCommandEx<GetAspNetIdentityUserClaimCommand>(c => c
                    .AddParameter(c => c.UserName, "alice"))
                .Invoke()
                .ToArray();

            var modifiedClaim = resultFromRead.Select(pso => (Claim)pso.ImmediateBaseObject).Single(c => c.Type.Equals(ClaimTypes.Email));

            Assert.Equal(ClaimValueTypes.String, modifiedClaim.ValueType);
            Assert.Equal("changed-email", modifiedClaim.Value);
        }

        [Fact]
        public void IdentityShell_removes_user_claim()
        {
            // ARRANGE
            var arranged = ArrangeAspNetIdentityUser();

            var claim = this.PowerShell
               .AddCommandEx<NewClaimCommand>(c => c
                   .AddParameter(c => c.Type, ClaimTypes.Email)
                   .AddParameter(c => c.ValueType, ClaimValueTypes.Email)
                   .AddParameter(c => c.Value, "changed-email"))
               .Invoke().Single();
            this.PowerShell.Commands.Clear();

            // ACT

            this.PowerShell
                .AddCommandEx<RemoveAspNetIdentityUserClaimCommand>(c => c
                    .AddParameter(c => c.UserName, "alice"))
                .Invoke(Array(claim));

            // ASSERT

            this.PowerShell.Commands.Clear();
            var resultFromRead = this.PowerShell
                .AddCommandEx<GetAspNetIdentityUserClaimCommand>(c => c
                    .AddParameter(c => c.UserName, "alice"))
                .Invoke()
                .ToArray();

            Assert.Empty(resultFromRead);
        }
    }
}