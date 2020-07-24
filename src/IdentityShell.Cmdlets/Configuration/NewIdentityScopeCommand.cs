using IdentityServer4.Models;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "IdentityScope")]
    public class NewIdentityScopeCommand : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        [Parameter()]
        public string DisplayName { get; set; }

        [Parameter()]
        public string Description { get; set; }

        [Parameter()]
        public bool Emphasize { get; set; }

        [Parameter()]
        public bool Required { get; set; }

        [Parameter()]
        public bool ShowInDiscoveryDocument { get; set; }

        [Parameter]
        public object[] UserClaims { get; set; }

        protected override void ProcessRecord()
        {
            this.WriteObject(this.SetBoundParameters(new ApiScope(this.Name)));
        }

        private ApiScope SetBoundParameters(ApiScope scope)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(DisplayName)))
            {
                scope.DisplayName = this.DisplayName;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Description)))
            {
                scope.Description = this.Description;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Emphasize)))
            {
                scope.Emphasize = this.Emphasize;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(Required)))
            {
                scope.Required = this.Required;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ShowInDiscoveryDocument)))
            {
                scope.ShowInDiscoveryDocument = this.ShowInDiscoveryDocument;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(nameof(ShowInDiscoveryDocument)))
            {
                scope.UserClaims = this.UserClaims.Select(uc => uc.ToString()).ToList();
            }
            return scope;
        }
    }
}