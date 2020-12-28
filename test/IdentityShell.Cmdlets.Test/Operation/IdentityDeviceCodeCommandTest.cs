using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityShell.Cmdlets;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Xunit;

namespace IdentityShell.Cmdlets.Test.Operation
{
    [Collection(nameof(IdentityCommandBase.GlobalServiceProvider))]
    public class IdentityDeviceCodeCommandTest : IdentityOperationCommandTestBase
    {
        [Fact]
        public void IdentityShell_reads_DeviceCode_by_UserCode()
        {
            // ARRANGE

            var deviceCode = this.ArrangeDeviceCode();

            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityDeviceCode")
                    .AddParameter("UserCode", "userCode");

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Equal(deviceCode.AuthorizedScopes, result.Property<IEnumerable<string>>("AuthorizedScopes"));
            Assert.Equal(deviceCode.IsAuthorized, result.Property<bool>("IsAuthorized"));
            Assert.Equal(deviceCode.IsOpenId, result.Property<bool>("IsOpenId"));
            Assert.Equal(deviceCode.Lifetime, result.Property<int>("Lifetime"));
            Assert.Equal(deviceCode.RequestedScopes, result.Property<IEnumerable<string>>("RequestedScopes"));
            Assert.Equal("userCode", result.Property<string>("UserCode"));
        }

        [Fact]
        public void IdentityShell_reads_DeviceCode_by_DeviceCode()
        {
            // ARRANGE

            var deviceCode = this.ArrangeDeviceCode();

            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityDeviceCode")
                    .AddParameter("DeviceCode", "deviceCode");

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Equal(deviceCode.AuthorizedScopes, result.Property<IEnumerable<string>>("AuthorizedScopes"));
            Assert.Equal(deviceCode.IsAuthorized, result.Property<bool>("IsAuthorized"));
            Assert.Equal(deviceCode.IsOpenId, result.Property<bool>("IsOpenId"));
            Assert.Equal(deviceCode.Lifetime, result.Property<int>("Lifetime"));
            Assert.Equal(deviceCode.RequestedScopes, result.Property<IEnumerable<string>>("RequestedScopes"));
            Assert.Equal("deviceCode", result.Property<string>("DeviceCode"));
        }

        [Fact]
        public void IdentityShell_removes_DeviceCode_by_DeviceCode()
        {
            // ARRANGE

            this.ArrangeDeviceCode();

            // ACT

            this.PowerShell
                .AddCommand("Remove-IdentityDeviceCode")
                    .AddParameter("DeviceCode", "deviceCode");

            this.PowerShell.Invoke();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();
            var existingDeviceCode = this.PowerShell
                .AddCommand("Get-IdentityDeviceCode")
                    .AddParameter("DeviceCode", "deviceCode")
                .Invoke()
                .ToArray();

            Assert.Empty(existingDeviceCode);
        }

        [Fact]
        public void IdentityShell_removes_DeviceCode_by_DeviceCode_from_Pipe()
        {
            // ARRANGE

            var deviceCode = this.ArrangeDeviceCode();

            // ACT

            this.PowerShell
                .AddCommand("Remove-IdentityDeviceCode");

            var pso = PSObject.AsPSObject(deviceCode);
            pso.Properties.Add(new PSNoteProperty(nameof(DeviceCode), "deviceCode"));

            this.PowerShell.Invoke(new[] { pso });

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();
            var existingDeviceCode = this.PowerShell
                .AddCommand("Get-IdentityDeviceCode")
                    .AddParameter("DeviceCode", "deviceCode")
                .Invoke()
                .ToArray();

            Assert.Empty(existingDeviceCode);
        }

        //[Fact]
        //public void IdentityShell_removes_grant_by_pipe()
        //{
        //    // ARRANGE

        //    var grant = this.ArrangePersistedGrant();

        //    // ACT

        //    this.PowerShell
        //        .AddCommand("Remove-IdentityDeviceFlowCode");

        //    this.PowerShell.Invoke(new[] { PSObject.AsPSObject(grant) });

        //    // ASSERT

        //    Assert.False(this.PowerShell.HadErrors);

        //    this.PowerShell.Commands.Clear();
        //    var existingGrants = this.PowerShell.AddCommand("Get-IdentityDeviceFlowCode").Invoke().ToArray();

        //    Assert.Empty(existingGrants);
        //}

        private DeviceCode ArrangeDeviceCode()
        {
            var store = this.serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IDeviceFlowStore>();

            var model = new DeviceCode
            {
                ClientId = "clientId",
                CreationTime = DateTime.Now,
                AuthorizedScopes = new[] { "scpope1" },
                Lifetime = 1,
                IsAuthorized = true,
                IsOpenId = true,
                RequestedScopes = new[] { "scpope2" },
            };

            store.StoreDeviceAuthorizationAsync("deviceCode", "userCode", model);

            return model;
        }
    }
} 