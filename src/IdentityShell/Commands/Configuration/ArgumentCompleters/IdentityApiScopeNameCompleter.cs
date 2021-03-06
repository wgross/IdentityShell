﻿using IdentityShell.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace IdentityShell.Commands.Configuration.ArgumentCompleters
{
    public sealed class IdentityApiScopeNameCompleter : ServiceProviderArgumentCompleterBase<IApiScopeRepository>, IArgumentCompleter
    {
        private IApiScopeRepository apiScopes = null;

        public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            // complete is called multiple times: reuse the repo
            // fetch the repo in first completion call instead in ctor: ctor should be fast
            this.apiScopes ??= this.FetchDataSource;

            return this.apiScopes
                .Query(s => s.Name.StartsWith(wordToComplete, System.StringComparison.OrdinalIgnoreCase))
                .ToArray() // materialize the query
                .Select(s => new CompletionResult(s.Name));
        }
    }
}