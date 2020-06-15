using IdentityServer4.EntityFramework.Mappers;
using IdentityShell.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "IdentityResource")]
    [CmdletBinding()]
    [OutputType(typeof(IdentityServer4.Models.IdentityResource))]
    public class SetIdentityResourceCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public IdentityServer4.Models.IdentityResource InputObject { get; set; }

        [Parameter()]
        public bool Enabled { get; set; }

        [Parameter()]
        public string DisplayName { get; set; }

        [Parameter()]
        public string Description { get; set; }

        [Parameter()]
        public bool ShowInDiscoveryDocument { get; set; }

        [Parameter()]
        public ICollection<string> UserClaims { get; set; }

        [Parameter()]
        public Hashtable Properties { get; set; }

        [Parameter()]
        public bool Required { get; set; }

        [Parameter()]
        public bool Emphasize { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            using (this.ServiceProviderScope)
            using (this.Context)
            {
                IdentityServer4.Models.IdentityResource identityModel = this.InputObject;
                IdentityServer4.EntityFramework.Entities.IdentityResource identityEntity = null;

                if (identityModel is null)
                {
                    identityEntity = this.QueryIdentityResource().SingleOrDefault(c => c.Name == this.Name);
                    if (identityEntity is null)
                    {
                        identityModel = this.SetBoundParameters(new IdentityServer4.Models.IdentityResource());
                        this.Context.IdentityResources.Add(identityModel.ToEntity());
                    }
                    else
                    {
                        identityModel = this.SetBoundParameters(identityEntity.ToModel());
                        identityModel.ToEntity(identityEntity);
                    }
                }
                else
                {
                    identityEntity = this.QueryIdentityResource().SingleOrDefault(c => c.Name == this.Name);
                    this.SetBoundParameters(identityModel);
                    identityModel.ToEntity(identityEntity);
                }

                this.Context.SaveChanges();
                this.WriteObject(identityModel);
            }
        }

        private IdentityServer4.Models.IdentityResource SetBoundParameters(IdentityServer4.Models.IdentityResource identity)
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
                identity.UserClaims = this.UserClaims;
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