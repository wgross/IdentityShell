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

        [Fact]
        public async Task IdentityShell_reads_all_users()
        {
            // ARRANGE

            var (user, _) = await this.ArrangeUser();

            // ACT

            this.PowerShell.AddCommandEx<GetAspNetIdentityUserCommand>();

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Single(result);
            Assert.Equal(user.UserName, result.Single().Property<string>("UserName"));
            Assert.Equal(user.Email, result.Single().Property<string>("Email"));
            Assert.Equal(user.AccessFailedCount, result.Single().Property<int>("AccessFailedCount"));
            Assert.Equal(user.ConcurrencyStamp, result.Single().Property<string>("ConcurrencyStamp"));
            Assert.Equal(user.EmailConfirmed, result.Single().Property<bool>("EmailConfirmed"));
            Assert.Equal(user.LockoutEnabled, result.Single().Property<bool>("LockoutEnabled"));
            Assert.Equal(user.LockoutEnd, result.Single().Property<DateTimeOffset?>("LockoutEnd"));
            Assert.Equal(user.NormalizedEmail, result.Single().Property<string>("NormalizedEmail"));
            Assert.Equal(user.NormalizedUserName, result.Single().Property<string>("NormalizedUserName"));
            Assert.Equal(user.PasswordHash, result.Single().Property<string>("PasswordHash"));
            Assert.Equal(user.PhoneNumber, result.Single().Property<string>("PhoneNumber"));
            Assert.Equal(user.PhoneNumberConfirmed, result.Single().Property<bool>("PhoneNumberConfirmed"));
            Assert.Equal(user.SecurityStamp, result.Single().Property<string>("SecurityStamp"));
            Assert.Equal(user.TwoFactorEnabled, result.Single().Property<bool>("TwoFactorEnabled"));
        }

        [Fact]
        public void IdentityShell_creates_new_user()
        {
            // ARRANGE

            var claims = new Claim[]
            {
                new Claim(JwtClaimTypes.Name, "Alice Smith"),
                new Claim(JwtClaimTypes.GivenName, "Alice"),
                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
            };

            // ACT

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

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = (ApplicationUser)result.ImmediateBaseObject;

            this.PowerShell.Commands.Clear();
            this.PowerShell.AddCommandEx<GetAspNetIdentityUserCommand>();

            var resultFromRead = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Equal(resultValue.UserName, resultFromRead.Property<string>("UserName"));
            Assert.Equal(resultValue.Email, resultFromRead.Property<string>("Email"));
            Assert.Equal(resultValue.AccessFailedCount, resultFromRead.Property<int>("AccessFailedCount"));
            Assert.Equal(resultValue.ConcurrencyStamp, resultFromRead.Property<string>("ConcurrencyStamp"));
            Assert.Equal(resultValue.EmailConfirmed, resultFromRead.Property<bool>("EmailConfirmed"));
            Assert.Equal(resultValue.LockoutEnabled, resultFromRead.Property<bool>("LockoutEnabled"));
            Assert.Equal(resultValue.LockoutEnd, resultFromRead.Property<DateTimeOffset?>("LockoutEnd"));
            Assert.Equal(resultValue.NormalizedEmail, resultFromRead.Property<string>("NormalizedEmail"));
            Assert.Equal(resultValue.NormalizedUserName, resultFromRead.Property<string>("NormalizedUserName"));
            Assert.Equal(resultValue.PasswordHash, resultFromRead.Property<string>("PasswordHash"));
            Assert.Equal(resultValue.PhoneNumber, resultFromRead.Property<string>("PhoneNumber"));
            Assert.Equal(resultValue.PhoneNumberConfirmed, resultFromRead.Property<bool>("PhoneNumberConfirmed"));
            Assert.Equal(resultValue.SecurityStamp, resultFromRead.Property<string>("SecurityStamp"));
            Assert.Equal(resultValue.TwoFactorEnabled, resultFromRead.Property<bool>("TwoFactorEnabled"));
        }

        [Fact]
        public async Task IdentityShell_modifies_user()
        {
            // ARRANGE

            var (user, _) = await this.ArrangeUser();

            // ACT

            this.PowerShell
                .AddCommandEx<SetAspNetIdentityUserCommand>(cmd => cmd
                    .AddParameter(c => c.UserName, "alice")
                    .AddParameter(c => c.CurrentPassword, "Pass123$")
                    .AddParameter(c => c.NewPassword, "Pass123$-changed")
                    .AddParameter(c => c.PhoneNumberConfirmed, true)
                    .AddParameter(c => c.PhoneNumber, "123")
                    .AddParameter(c => c.TwoFactorEnabled, true)
                    .AddParameter(c => c.AccessFailedCount, 1)
                    .AddParameter(c => c.Email, "email")
                    .AddParameter(c => c.EmailConfirmed, true)
                    .AddParameter(c => c.LockoutEnabled, false)
                    .AddParameter(c => c.LockoutEnd, DateTimeOffset.Now));

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = (ApplicationUser)result.ImmediateBaseObject;

            this.PowerShell.Commands.Clear();
            this.PowerShell.AddCommandEx<GetAspNetIdentityUserCommand>();

            var resultFromRead = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Equal(resultValue.UserName, resultFromRead.Property<string>("UserName"));
            Assert.Equal(resultValue.NormalizedUserName, resultFromRead.Property<string>("NormalizedUserName"));
            Assert.Equal(resultValue.Email, resultFromRead.Property<string>("Email"));
            Assert.Equal(resultValue.NormalizedEmail, resultFromRead.Property<string>("NormalizedEmail"));
            Assert.Equal(resultValue.AccessFailedCount, resultFromRead.Property<int>("AccessFailedCount"));
            Assert.Equal(resultValue.ConcurrencyStamp, resultFromRead.Property<string>("ConcurrencyStamp"));
            Assert.Equal(resultValue.EmailConfirmed, resultFromRead.Property<bool>("EmailConfirmed"));
            Assert.Equal(resultValue.LockoutEnabled, resultFromRead.Property<bool>("LockoutEnabled"));
            Assert.Equal(resultValue.LockoutEnd, resultFromRead.Property<DateTimeOffset?>("LockoutEnd"));
            Assert.Equal(resultValue.PasswordHash, resultFromRead.Property<string>("PasswordHash"));
            Assert.Equal(resultValue.PhoneNumber, resultFromRead.Property<string>("PhoneNumber"));
            Assert.Equal(resultValue.PhoneNumberConfirmed, resultFromRead.Property<bool>("PhoneNumberConfirmed"));
            Assert.Equal(resultValue.SecurityStamp, resultFromRead.Property<string>("SecurityStamp"));
            Assert.Equal(resultValue.TwoFactorEnabled, resultFromRead.Property<bool>("TwoFactorEnabled"));

            Assert.NotEqual(resultValue.Email, user.Email);
            Assert.NotEqual(resultValue.NormalizedEmail, user.NormalizedEmail);
            Assert.NotEqual(resultValue.EmailConfirmed, user.EmailConfirmed);
            Assert.NotEqual(resultValue.AccessFailedCount, user.AccessFailedCount);
            Assert.NotEqual(resultValue.LockoutEnabled, user.LockoutEnabled);
            Assert.NotEqual(resultValue.LockoutEnd, user.LockoutEnd);
            Assert.NotEqual(resultValue.NormalizedEmail, user.NormalizedEmail);
            Assert.NotEqual(resultValue.PasswordHash, user.PasswordHash);
            Assert.NotEqual(resultValue.PhoneNumber, user.PhoneNumber);
            Assert.NotEqual(resultValue.PhoneNumberConfirmed, user.PhoneNumberConfirmed);
            Assert.NotEqual(resultValue.PhoneNumberConfirmed, user.PhoneNumberConfirmed);
            Assert.NotEqual(resultValue.TwoFactorEnabled, user.TwoFactorEnabled);
        }

        [Fact]
        public async Task IdentityShell_reads_user_claims()
        {
            // ARRANGE
            var (user, claims) = await this.ArrangeUser();

            // ACT

            this.PowerShell
                .AddCommandEx<GetAspNetIdentityUserClaimCommand>(c => c.AddParameter(c => c.UserName, user.UserName));

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Claim claim(string type) => claims.Single(c => c.Type.Equals(type));

            Assert.All(result.Select(r => (Claim)r.ImmediateBaseObject), r =>
            {
                Assert.Equal(claim(r.Type).Value, r.Value);
                // types are different//Assert.Equal(claim(r.Type).ValueType, r.ValueType);
            });
        }

        [Fact]
        public async Task IdentityShell_modifies_user_claims()
        {
            // ARRANGE
            var (user, claims) = await this.ArrangeUser();

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
                    .AddParameter(c => c.UserName, user.UserName))
                    .Invoke(new PSObject[] { claim });

            // ASSERT

            this.PowerShell.Commands.Clear();
            var resultFromRead = this.PowerShell
                .AddCommandEx<GetAspNetIdentityUserClaimCommand>(c => c
                    .AddParameter(c => c.UserName, user.UserName))
                .Invoke()
                .ToArray();

            var modifiedClaim = resultFromRead.Select(pso => (Claim)pso.ImmediateBaseObject).Single(c => c.Type.Equals(ClaimTypes.Email));

            Assert.Equal(ClaimValueTypes.String, modifiedClaim.ValueType);
            Assert.Equal("changed-email", modifiedClaim.Value);
        }
    }
}