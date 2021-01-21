using Example.WebApi;
using IdentityModel.Client;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityShell.IntegTest.TestServer;
using IdentityShell.IntegTest.WebApi;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace IdentityShell.IntegTest
{
    public class ExampleWebApiTest
    {
        private Microsoft.AspNetCore.TestHost.TestServer IdentityShell { get; }

        private Microsoft.AspNetCore.TestHost.TestServer ExampleWebApi { get; }

        public ExampleWebApiTest()
        {
            this.IdentityShell = new TestServerHostFactory().Server;
            this.ExampleWebApi = new ExampleWebApiHostFactory(this.IdentityShell).Server;
        }

        private async Task ArrangeClientAuthorization()
        {
            using var cfgDbx = this.IdentityShell.Services.GetRequiredService<ConfigurationDbContext>();

            cfgDbx.ApiScopes.Add(new ApiScope
            {
                Name = "api-access"
            });

            cfgDbx.ApiResources.Add(new ApiResource
            {
                Name = "http://my/api",
                Scopes = new()
                {
                    new ApiResourceScope
                    {
                        Scope = "api-access",
                    }
                }
            });

            cfgDbx.Clients.Add(new Client
            {
                ClientId = "api-client",
                AllowedGrantTypes = new()
                {
                    new ClientGrantType
                    {
                        GrantType = IdentityServer4.Models.GrantType.ClientCredentials
                    }
                },
                AllowedScopes = new()
                {
                    new ClientScope
                    {
                        Scope = "api-access"
                    }
                },
                RequireClientSecret = true,
                ClientSecrets = new()
                {
                    new ClientSecret
                    {
                        Value = IdentityServer4.Models.HashExtensions.Sha256("secret")
                    }
                }
            });

            await cfgDbx.SaveChangesAsync();
        }

        [Fact]
        public async Task HttpClient_retrieves_weather_forecast()
        {
            // ARRANGE

            await this.ArrangeClientAuthorization();

            var identityShellClient = this.IdentityShell.CreateClient();
            var discoveryDocument = await identityShellClient.GetDiscoveryDocumentAsync();

            var tokenResponse = await identityShellClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "api-client",
                ClientSecret = "secret",
                Scope = "api-access",
            });

            var exampleWebApiClient = this.ExampleWebApi.CreateClient();
            exampleWebApiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            // ACT

            var result = await exampleWebApiClient.GetFromJsonAsync<WeatherForecast[]>($"{this.ExampleWebApi.BaseAddress}WeatherForecast");

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(5, result.Length);
        }

        [Fact]
        public async Task HttpClient_retrieving_weather_forecast_fails_without_token()
        {
            // ARRANGE

            await this.ArrangeClientAuthorization();

            var exampleWebApiClient = this.ExampleWebApi.CreateClient();

            // ACT

            var result = await Assert.ThrowsAsync<HttpRequestException>(() => exampleWebApiClient.GetFromJsonAsync<WeatherForecast[]>($"{this.ExampleWebApi.BaseAddress}WeatherForecast"));

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}