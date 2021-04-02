using Duende.IdentityServer.Test;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;
using System.Security.Claims;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Set, "TestUser")]
    [OutputType(typeof(TestUser))]
    public class SetTestUserCommand : IdentityCommandBase
    {
        [Parameter()]
        public TestUser InputObject { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Username { get; set; }

        [Parameter]
        public string Password { get; set; }

        [Parameter]
        public bool IsActive { get; set; }

        [Parameter]
        public virtual int AccessFailedCount { get; set; }

        [Parameter]
        public string ProviderName { get; private set; }

        [Parameter]
        public string ProviderSubjectId { get; private set; }

        [Parameter]
        public string SubjectId { get; private set; }

        [Parameter]
        public Claim[] Claims { get; private set; }

        protected override void ProcessRecord()
        {
            var user = this.InputObject;
            var existingUser = this.LocalServiceProvider
                .GetRequiredService<ITestUserRepository>()
                .Query(u => u.Username == this.Username)
                .FirstOrDefault();

            if (user is null && existingUser is null)
            {
                user = this.SetBoundParameters(new());
                this.LocalServiceProvider
                    .GetRequiredService<ITestUserRepository>()
                    .Add(user);
            }
            else if (user is not null && existingUser is null)
            {
                this.LocalServiceProvider
                    .GetRequiredService<ITestUserRepository>()
                    .Add(this.SetBoundParameters(user));
            }
            else
            {
                user = this.SetBoundParameters(existingUser);
            }
            this.WriteObject(user);
        }

        private TestUser SetBoundParameters(TestUser user)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Username)))
            {
                user.Username = this.Username;
            }

            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(SubjectId)))
            {
                user.SubjectId = this.SubjectId;
            }

            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(IsActive)))
            {
                user.IsActive = this.IsActive;
            }

            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Password)))
            {
                user.Password = this.Password;
            }

            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Password)))
            {
                user.ProviderName = this.ProviderName;
            }

            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Password)))
            {
                user.ProviderSubjectId = this.ProviderSubjectId;
            }

            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Password)))
            {
                user.SubjectId = this.SubjectId;
            }

            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Claims)))
            {
                user.Claims = this.Claims;
            }

            return user;
        }
    }
}