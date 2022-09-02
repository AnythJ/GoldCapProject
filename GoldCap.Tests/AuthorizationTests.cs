using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http.Headers;

namespace GoldCap.Tests
{
    public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory<GoldCap.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        public AuthorizationTests(CustomWebApplicationFactory<GoldCap.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/Dashboard")]
        [InlineData("/Expenses")]
        [InlineData("/Categories")]
        public async Task Get_ControllersIndexActionsForAnUnauthenticatedUser_ReturnsRedirectToAccountLoginForm(string url)
        {
            // Arrange
            var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            // Act
            var response = await client.GetAsync(url);

            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("http://localhost/Account/Login",
                response.Headers.Location.OriginalString);
        }

        [Theory]
        [InlineData("/Dashboard")]
        [InlineData("/Expenses")]
        [InlineData("/Categories")]
        public async Task Get_ControllersIndexActionsForAnAuthenticatedUser_ReturnsViewFromUrl(string url)
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddMvc(options =>
                    {
                        options.Filters.Add(new AllowAnonymousFilter());
                        options.Filters.Add(new TestUserFilter());
                    });
                });
            })
                .CreateClient();

            // Act
            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            // Assert

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


    }
}
