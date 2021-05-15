using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace IdentityShell.Commands.Configuration.ArgumentCompleters
{
    public sealed class IdentityClientIdCompleter : ServiceProviderArgumentCompleterBase<IClientRepository>, IArgumentCompleter
    {
        private IClientRepository clients = null;

        public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            // complete is called multiple times: reuse the client repo
            // fetch the repo in first completion call instead in ctor: ctor should be fast
            this.clients ??= this.FetchDataSource;

            return this.clients
                .Query(c => c.ClientId.StartsWith(wordToComplete, System.StringComparison.OrdinalIgnoreCase))
                .ToArray() // materialize the query
                .Select(c => new CompletionResult(c.ClientId));
        }
    }
}