using Duende.IdentityServer.Models;
using IdentityShell.Commands.Configuration.ArgumentCompleters;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Set, "IdentityApiResource")]
    [CmdletBinding()]
    [OutputType(typeof(IdentityResource))]
    public class SetIdentityApiResourceCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ArgumentCompleter(typeof(IdentityApiResourceNameCompleter))]
        public string Name { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public ApiResource InputObject { get; set; }

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
        public string[] Scopes { get; set; }

        protected override void ProcessRecord()
        {
            var apiResource = this.InputObject;

            var existingApiResource = this.LocalServiceProvider
                .GetRequiredService<IApiResourceRepository>()
                .Query(c => c.Name == this.Name)
                .FirstOrDefault();

            if (apiResource is null && existingApiResource is null)
            {
                apiResource = this.SetBoundParameters(new());

                this.LocalServiceProvider
                    .GetRequiredService<IApiResourceRepository>()
                    .Add(apiResource);
            }
            else if (apiResource is not null && existingApiResource is null)
            {
                this.LocalServiceProvider
                   .GetRequiredService<IApiResourceRepository>()
                   .Add(this.SetBoundParameters(apiResource));
            }
            else
            {
                this.SetBoundParameters(apiResource);
            }

            this.WriteObject(apiResource);
        }

        private ApiResource SetBoundParameters(ApiResource apiResource)
        {
            if (this.IsParameterBound(nameof(Enabled)))
            {
                apiResource.Enabled = this.Enabled;
            }
            if (this.IsParameterBound(nameof(Name)))
            {
                apiResource.Name = this.Name;
            }
            if (this.IsParameterBound(nameof(DisplayName)))
            {
                apiResource.DisplayName = this.DisplayName;
            }
            if (this.IsParameterBound(nameof(Description)))
            {
                apiResource.Description = this.Description;
            }
            if (this.IsParameterBound(nameof(UserClaims)))
            {
                apiResource.UserClaims = Collection(this.UserClaims);
            }
            if (this.IsParameterBound(nameof(Properties)))
            {
                apiResource.Properties = this.Properties
                    .OfType<DictionaryEntry>()
                    .ToDictionary(keySelector: d => d.Key.ToString(), elementSelector: d => d.Value.ToString());
            }
            if (this.IsParameterBound(nameof(ApiSecrets)))
            {
                apiResource.ApiSecrets = this.ApiSecrets.Select(s => PSArgumentValue<Secret>(s)).ToList();
            }
            if (this.IsParameterBound(nameof(Scopes)))
            {
                apiResource.Scopes = this.Scopes.Select(s => PSArgumentValue<string>(s)).ToList();
            }

            this.ValidateNullability(apiResource);
            return apiResource;
        }

        private void ValidateNullability(ApiResource apiModel)
        {
            if (!apiModel.Scopes.Any())
                this.WriteWarning($"apiResource(name='{apiModel.Name}') has no scope. An api resources requires at least one scope");
        }
    }
}