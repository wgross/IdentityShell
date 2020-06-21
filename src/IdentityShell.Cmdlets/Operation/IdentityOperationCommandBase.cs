using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityShell.Cmdlets.Operation
{
    public abstract class IdentityOperationCommandBase : IdentityCommandBase<PersistedGrantDbContext>
    {
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.Context = this.LocalServiceProvider.GetRequiredService<PersistedGrantDbContext>();
        }
    }
}