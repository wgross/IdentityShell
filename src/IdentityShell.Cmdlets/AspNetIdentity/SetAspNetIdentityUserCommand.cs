using IdentityServerAspNetIdentity.Models;
using System;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.AspNetIdentity
{
    [Cmdlet(VerbsCommon.Set, "AspNetIdentityUser")]
    [OutputType(typeof(ApplicationUser))]
    public class SetAspNetIdentityUserCommand : AspNetIdentityUserCommandBase
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string UserName { get; set; }

        [Parameter]
        public string NewPassword { get; set; }

        [Parameter]
        public string CurrentPassword { get; set; }

        [Parameter]
        public DateTimeOffset? LockoutEnd { get; set; }

        [Parameter]
        public bool TwoFactorEnabled { get; set; }

        [Parameter]
        public bool PhoneNumberConfirmed { get; set; }

        [Parameter]
        public virtual string PhoneNumber { get; set; }

        [Parameter]
        public bool EmailConfirmed { get; set; }

        [Parameter]
        public virtual string Email { get; set; }

        [Parameter]
        public bool LockoutEnabled { get; set; }

        [Parameter]
        public virtual int AccessFailedCount { get; set; }

        protected override void ProcessRecord()
        {
            var user = Await(this.UserManager.FindByNameAsync(this.UserName));

            if (user is null)
            {
                user = this.SetBoundParameters(new ApplicationUser
                {
                    UserName = this.UserName
                });

                this.CheckIdentityResult(this.UserManager.CreateAsync(user, this.NewPassword));
            }
            else
            {
                if (this.MyInvocation.BoundParameters.ContainsKey(nameof(NewPassword)))
                {
                    this.CheckIdentityResult(() => this.UserManager.ChangePasswordAsync(user, this.CurrentPassword, this.NewPassword));
                }

                this.SetBoundParameters(user);
                this.CheckIdentityResult(() => this.UserManager.UpdateAsync(user));
            }

            this.WriteObject(user);
        }

        private ApplicationUser SetBoundParameters(ApplicationUser user)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(AccessFailedCount)))
            {
                user.AccessFailedCount = this.AccessFailedCount;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Email)))
            {
                user.Email = this.Email;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(EmailConfirmed)))
            {
                user.EmailConfirmed = this.EmailConfirmed;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(LockoutEnabled)))
            {
                user.LockoutEnabled = this.LockoutEnabled;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(LockoutEnd)))
            {
                user.LockoutEnd = this.LockoutEnd;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(PhoneNumber)))
            {
                user.PhoneNumber = this.PhoneNumber;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(PhoneNumberConfirmed)))
            {
                user.PhoneNumberConfirmed = this.PhoneNumberConfirmed;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(TwoFactorEnabled)))
            {
                user.TwoFactorEnabled = this.TwoFactorEnabled;
            }

            return user;
        }
    }
}