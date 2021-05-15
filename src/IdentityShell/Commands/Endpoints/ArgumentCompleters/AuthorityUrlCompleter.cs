using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace IdentityShell.Commands.Endpoints
{
    public sealed class AuthorityUrlCompleter : ServiceProviderArgumentCompleterBase<IConfiguration>, IArgumentCompleter
    {
        private IConfiguration configuration;

        public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            this.configuration ??= this.FetchDataSource;

            return
                (this.configuration["Urls"]?.Split(";") ?? Enumerable.Empty<string>())
                .Where(n => n.Contains(wordToComplete ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                .Select(n => new CompletionResult(n));
        }
    }
}