using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using System.Collections;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Configuration
{
    [Cmdlet(VerbsCommon.Set, "IdentityApiScope")]
    [OutputType(typeof(ApiScope))]
    public sealed class SetIdentityApiScopeCommand : IdentityConfigurationCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        [Parameter()]
        public string DisplayName { get; set; }

        [Parameter()]
        public string Description { get; set; }

        [Parameter()]
        public bool Emphasize { get; set; }

        [Parameter()]
        public bool Enabled { get; set; }

        [Parameter()]
        public bool Required { get; set; }

        [Parameter()]
        public bool ShowInDiscoveryDocument { get; set; }

        [Parameter]
        public Hashtable Properties { get; set; }

        [Parameter]
        public string[] UserClaims { get; set; }

        protected override void ProcessRecord()
        {
            var apiScopeEntity = this.QueryApiScopes().SingleOrDefault(c => c.Name == this.Name);
            if (apiScopeEntity is null)
            {
                var apiScopeModel = this.SetBoundParameters(new ApiScope());
                this.Context.ApiScopes.Add(apiScopeModel.ToEntity());
                this.Context.SaveChanges();
                this.WriteObject(apiScopeModel);
            }
            else
            {
                var apiScopeModel = apiScopeEntity.ToModel();
                apiScopeModel = this.SetBoundParameters(apiScopeModel);
                apiScopeModel.ToEntity(apiScopeEntity);
                this.Context.SaveChanges();
                this.WriteObject(apiScopeModel);
            }
        }

        private ApiScope SetBoundParameters(ApiScope apiScope)
        {
            apiScope.Name = this.Name;

            if (this.IsParameterBound(nameof(this.DisplayName)))
                apiScope.DisplayName = this.DisplayName;

            if (this.IsParameterBound(nameof(this.Description)))
                apiScope.Description = this.Description;

            if (this.IsParameterBound(nameof(this.Emphasize)))
                apiScope.Emphasize = this.Emphasize;

            if (this.IsParameterBound(nameof(this.Enabled)))
                apiScope.Enabled = this.Enabled;

            if (this.IsParameterBound(nameof(this.Required)))
                apiScope.Required = this.Required;

            if (this.IsParameterBound(nameof(this.ShowInDiscoveryDocument)))
                apiScope.ShowInDiscoveryDocument = this.ShowInDiscoveryDocument;

            if (this.IsParameterBound(nameof(this.Properties)))
                apiScope.Properties = this.ToDictionary(this.Properties);

            if (this.IsParameterBound(nameof(this.UserClaims)))
                apiScope.UserClaims = this.UserClaims.ToList();

            return apiScope;
        }
    }
}