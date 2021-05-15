using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace IdentityShell.Commands.Configuration.ArgumentCompleters
{
    public sealed class IdentityResourceNameCompleter : ServiceProviderArgumentCompleterBase<IIdentityResourceRepository>, IArgumentCompleter
    {
        private IIdentityResourceRepository identityResources = null;

        public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            // complete is called multiple times: reuse the repo
            // fetch the repo in first completion call instead in ctor: ctor should be fast
            this.identityResources ??= this.FetchDataSource;

            return this.identityResources
                .Query(ir => ir.Name.StartsWith(wordToComplete, System.StringComparison.OrdinalIgnoreCase))
                .ToArray() // materialize the query
                .Select(ir => new CompletionResult(ir.Name));
        }
    }
}