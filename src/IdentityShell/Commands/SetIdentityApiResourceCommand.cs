using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityShell.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "IdentityApiResource")]
    [CmdletBinding()]
    [OutputType(typeof(IdentityServer4.Models.IdentityResource))]
    public class SetIdentityApiResourceCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public IdentityServer4.Models.ApiResource InputObject { get; set; }

        [Parameter()]
        public bool Enabled { get; set; }

        [Parameter()]
        public string DisplayName { get; set; }

        [Parameter()]
        public string Description { get; set; }

        [Parameter()]
        public ICollection<string> UserClaims { get; set; }

        [Parameter()]
        public Hashtable Properties { get; set; }

        [Parameter()]
        public ICollection<IdentityServer4.Models.Secret> ApiSecrets { get; set; }

        [Parameter()]
        public ICollection<Scope> Scopes { get; set; }

        protected override void ProcessRecord()
        {
            IdentityServer4.Models.ApiResource apiModel = this.InputObject;
            IdentityServer4.EntityFramework.Entities.ApiResource apiEntity = null;

            if (apiModel is null)
            {
                apiEntity = this.QueryApiResource().SingleOrDefault(c => c.Name == this.Name);
                if (apiEntity is null)
                {
                    apiModel = this.SetBoundParameters(new IdentityServer4.Models.ApiResource());
                    this.Context.ApiResources.Add(apiModel.ToEntity());
                }
                else
                {
                    apiModel = this.SetBoundParameters(apiEntity.ToModel());
                    apiModel.ToEntity(apiEntity);
                }
            }
            else
            {
                apiEntity = this.QueryApiResource().SingleOrDefault(c => c.Name == this.Name);
                this.SetBoundParameters(apiModel);
                apiModel.ToEntity(apiEntity);
            }

            this.Context.SaveChanges();
            this.WriteObject(apiModel);
        }

        private IdentityServer4.Models.ApiResource SetBoundParameters(IdentityServer4.Models.ApiResource apiModel)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Enabled)))
            {
                apiModel.Enabled = this.Enabled;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Name)))
            {
                apiModel.Name = this.Name;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(DisplayName)))
            {
                apiModel.DisplayName = this.DisplayName;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Description)))
            {
                apiModel.Description = this.Description;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(UserClaims)))
            {
                apiModel.UserClaims = this.UserClaims;
            }

            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(UserClaims)))
            {
                apiModel.UserClaims = this.UserClaims;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Properties)))
            {
                apiModel.Properties = this.Properties
                    .OfType<DictionaryEntry>()
                    .ToDictionary(keySelector: d => d.Key.ToString(), elementSelector: d => d.Value.ToString());
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ApiSecrets)))
            {
                apiModel.ApiSecrets = this.ApiSecrets;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Scopes)))
            {
                apiModel.Scopes = this.Scopes;
            }

            return apiModel;
        }
    }
}