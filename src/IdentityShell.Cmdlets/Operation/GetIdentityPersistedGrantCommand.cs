using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Operation
{
    [Cmdlet(VerbsCommon.Get, "IdentityPersistedGrant")]
    [OutputType(typeof(PersistedGrant))]
    public sealed class GetIdentityPersistedGrantCommand : IdentityOperationCommandBase
    {
        protected override void ProcessRecord()
        {
            this.Context.PersistedGrants
                .Select(e => PersistedGrantMappers.ToModel(e))
                .ToList()
                .ForEach(m => this.WriteObject(m));
        }
    }
}