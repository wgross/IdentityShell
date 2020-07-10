﻿using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Configuration
{
    [Cmdlet(VerbsCommon.Remove, "IdentityClient")]
    public class RemoveIdentityClientCommand : IdentityConfigurationCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string ClientId { get; set; }

        protected override void ProcessRecord()
        {
            var clientEntity = this.QueryClients().SingleOrDefault(c => c.ClientId == this.ClientId);

            if (clientEntity is null)
            {
                this.WriteError(new ErrorRecord(
                    exception: new PSInvalidOperationException($"client(clientId='{0}') doesn't exist"),
                    errorId: "client.not_found",
                    errorCategory: ErrorCategory.ObjectNotFound,
                    targetObject: this.ClientId));
                return;
            }

            this.Context.Clients.Remove(clientEntity);
            this.Context.SaveChanges();
        }
    }
}