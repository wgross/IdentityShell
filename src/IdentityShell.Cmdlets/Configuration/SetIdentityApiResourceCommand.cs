using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityShell.Data;
using System.Collections;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Configuration
{
    [Cmdlet(VerbsCommon.Set, "IdentityApiResource")]
    [CmdletBinding()]
    [OutputType(typeof(IdentityServer4.Models.IdentityResource))]
    public class SetIdentityApiResourceCommand : IdentityConfigurationCommandBase
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
        public object[] UserClaims { get; set; }

        [Parameter()]
        public Hashtable Properties { get; set; }

        [Parameter()]
        public object[] ApiSecrets { get; set; }

        [Parameter()]
        public object[] Scopes { get; set; }

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
                // manipulate the given identity Model in place
                this.SetBoundParameters(apiModel);

                // copy values to new or existing entity
                apiEntity = this.QueryApiResource().SingleOrDefault(c => c.Name == this.Name);
                if (apiEntity is null)
                {
                    this.Context.ApiResources.Add(apiModel.ToEntity());
                }
                else
                {
                    apiModel.ToEntity(apiEntity);
                }
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
                apiModel.UserClaims = Collection(this.UserClaims);
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Properties)))
            {
                apiModel.Properties = this.Properties
                    .OfType<DictionaryEntry>()
                    .ToDictionary(keySelector: d => d.Key.ToString(), elementSelector: d => d.Value.ToString());
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ApiSecrets)))
            {
                apiModel.ApiSecrets = this.ApiSecrets.Select(s => this.PSArgumentCast<Secret>(s)).ToList();
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Scopes)))
            {
                apiModel.Scopes = this.Scopes.Select(s => this.PSArgumentCast<Scope>(s)).ToList();
            }

            this.ValidateNullability(apiModel);
            return apiModel;
        }

        private void ValidateNullability(ApiResource apiModel)
        {
            if (!apiModel.Scopes.Any())
                this.WriteWarning($"apiResource(name='{apiModel.Name}') has no scope. An api resources requires at least one scope");
        }
    }
}