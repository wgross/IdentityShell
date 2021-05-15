using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace IdentityShell.Commands
{
    public abstract class IdentityCommandBase : PSCmdlet
    {
        public static IServiceProvider GlobalServiceProvider { protected internal get; set; }

        protected static ICollection<string> Collection(object[] items) => items.Select(i => i.ToString()).ToHashSet();

        protected IServiceProvider LocalServiceProvider { get; set; }

        protected static T Await<T>(Task<T> task) => AsyncHelper.AwaitWithoutOriginalContext(task);

        /// <summary>
        /// Starts Command processing. A command processing has its own dependency injection scope.
        /// </summary>
        protected override void BeginProcessing() => this.LocalServiceProvider = GlobalServiceProvider.CreateScope().ServiceProvider;

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

        protected Dictionary<string, string> ToDictionary(Hashtable hashtable) => hashtable
          .OfType<DictionaryEntry>()
          .ToDictionary(
              keySelector: d => d.Key.ToString(),
              elementSelector: d => d.Value.ToString());

        protected bool IsParameterBound(string parameterName) => this.MyInvocation.BoundParameters.ContainsKey(parameterName);
    }
}