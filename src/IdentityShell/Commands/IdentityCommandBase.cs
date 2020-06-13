using Microsoft.Extensions.DependencyInjection;
using System;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    public abstract class IdentityCommandBase : PSCmdlet
    {
        public static IServiceProvider ServiceProvider { protected get; set; }
    }
}