using Duende.IdentityServer.Models;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Set, "IdentityResource")]
    [CmdletBinding()]
    [OutputType(typeof(IdentityResource))]
    public class SetIdentityResourceCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public IdentityResource InputObject { get; set; }

        [Parameter()]
        public bool Enabled { get; set; }

        [Parameter()]
        public string DisplayName { get; set; }

        [Parameter()]
        public string Description { get; set; }

        [Parameter()]
        public bool ShowInDiscoveryDocument { get; set; }

        [Parameter()]
        public object[] UserClaims { get; set; }

        [Parameter()]
        public Hashtable Properties { get; set; }

        [Parameter()]
        public bool Required { get; set; }

        [Parameter()]
        public bool Emphasize { get; set; }

        protected override void ProcessRecord()
        {
            var identityResource = this.InputObject;
            var existingIdentityResource = this.LocalServiceProvider
                .GetRequiredService<IIdentityResourceRepository>()
                .Query(c => c.Name == this.Name)
                .FirstOrDefault();

            if (identityResource is null && existingIdentityResource is null)
            {
                identityResource = this.SetBoundParameters(new());
                this.LocalServiceProvider
                    .GetRequiredService<IIdentityResourceRepository>()
                    .Add(identityResource);
            }
            else if (identityResource is not null && existingIdentityResource is null)
            {
                identityResource = this.SetBoundParameters(identityResource);
                this.LocalServiceProvider
                    .GetRequiredService<IIdentityResourceRepository>()
                    .Add(identityResource);
            }
            else
            {
                identityResource = this.SetBoundParameters(identityResource);
            }

            this.WriteObject(identityResource);
        }

        private IdentityResource SetBoundParameters(IdentityResource identity)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Enabled)))
            {
                identity.Enabled = this.Enabled;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Name)))
            {
                identity.Name = this.Name;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(DisplayName)))
            {
                identity.DisplayName = this.DisplayName;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Description)))
            {
                identity.Description = this.Description;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Required)))
            {
                identity.Required = this.Required;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Emphasize)))
            {
                identity.Emphasize = this.Emphasize;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ShowInDiscoveryDocument)))
            {
                identity.ShowInDiscoveryDocument = this.ShowInDiscoveryDocument;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(UserClaims)))
            {
                identity.UserClaims = Collection(this.UserClaims);
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Properties)))
            {
                identity.Properties = this.Properties
                    .OfType<DictionaryEntry>()
                    .ToDictionary(keySelector: d => d.Key.ToString(), elementSelector: d => d.Value.ToString());
            }
            return identity;
        }
    }
}