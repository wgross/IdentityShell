using Duende.IdentityServer.Models;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Set, "IdentityApiScope")]
    [OutputType(typeof(ApiScope))]
    public sealed class SetIdentityApiScopeCommand : IdentityCommandBase
    {
        [Parameter(ValueFromPipeline = true)]
        public ApiScope InputObject { get; set; }

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
            var apiScope = this.InputObject;

            var existingApiScope = this.LocalServiceProvider
                .GetRequiredService<IApiScopeRepository>()
                .Query(c => c.Name == this.Name)
                .FirstOrDefault();

            if (apiScope is null && existingApiScope is null)
            {
                apiScope = this.SetBoundParameters(new());

                this.LocalServiceProvider
                    .GetRequiredService<IApiScopeRepository>()
                    .Add(apiScope);
            }
            else if (apiScope is not null && existingApiScope is null)
            {
                this.LocalServiceProvider
                    .GetRequiredService<IApiScopeRepository>()
                    .Add(this.SetBoundParameters(new()));
            }
            else
            {
                this.SetBoundParameters(apiScope);
            }

            this.WriteObject(this.SetBoundParameters(apiScope));
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