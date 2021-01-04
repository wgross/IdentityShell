using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace IdentityShell.Cmdlets
{
    public abstract class IdentityCommandBase : PSCmdlet
    {
        public static IServiceProvider GlobalServiceProvider { protected get; set; }

        protected static T Await<T>(Task<T> task) => AsyncHelper.AwaitWithoutOriginalContext(task);

        protected static ICollection<string> Collection(object[] items) => items.Select(i => i.ToString()).ToHashSet();
    }

    public abstract class IdentityCommandBase<CTX> : IdentityCommandBase
        where CTX : DbContext
    {
        protected IServiceProvider LocalServiceProvider { get; set; }

        protected CTX Context { get; set; }

        protected override void BeginProcessing() => this.LocalServiceProvider = GlobalServiceProvider.CreateScope().ServiceProvider;

        protected override void EndProcessing()
        {
            this.Context.Dispose();
            this.Context = null;
        }

        protected static T PSArgumentValue<T>(object argumentValue)
        {
            return argumentValue switch
            {
                T t => t,
                PSObject pso => pso.ImmediateBaseObject switch
                {
                    T underlying => underlying,
                    _ => throw new PSArgumentException($"Underlying type is {pso.ImmediateBaseObject.GetType()}, expected is {typeof(T)}")
                },
                _ => throw new PSArgumentException($"Argument type is {argumentValue.GetType()}, expected is {typeof(T)} or {typeof(PSObject)}")
            };
        }
    }
}