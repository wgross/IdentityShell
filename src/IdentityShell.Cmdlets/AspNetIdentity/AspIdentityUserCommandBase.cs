using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace IdentityShell.Cmdlets.AspNetIdentity
{
    public abstract class AspNetIdentityUserCommandBase : IdentityCommandBase<ApplicationDbContext>
    {
        protected UserManager<ApplicationUser> UserManager { get; private set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.Context = this.LocalServiceProvider.GetRequiredService<ApplicationDbContext>();
            this.UserManager = this.LocalServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        protected void CheckIdentityResult(Task<IdentityResult> result)
        {
            var resultValue = Await(result);
            if (!resultValue.Succeeded)
                throw new PSInvalidOperationException(resultValue.Errors.First().Description);
        }

        protected void CheckIdentityResult(Func<Task<IdentityResult>> result)
        {
            var resultValue = AsyncHelper.RunSync(result);
            if (!resultValue.Succeeded)
                throw new PSInvalidOperationException(resultValue.Errors.First().Description);
        }
    }
}