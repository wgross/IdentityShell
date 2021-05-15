using Microsoft.Extensions.DependencyInjection;

namespace IdentityShell.Commands
{
    public abstract class ServiceProviderArgumentCompleterBase<T>
    {
        protected T FetchDataSource => IdentityCommandBase.GlobalServiceProvider.CreateScope().ServiceProvider.GetRequiredService<T>();
    }
}