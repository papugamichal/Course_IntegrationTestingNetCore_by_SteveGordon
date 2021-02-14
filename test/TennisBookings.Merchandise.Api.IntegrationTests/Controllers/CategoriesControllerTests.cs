using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using TennisBookings.Merchandise.Api.IntegrationTests.Models;
using TennisBookings.Merchandise.Api.IntegrationTests.TestHelpers.Serialization;
using Xunit;

namespace TennisBookings.Merchandise.Api.IntegrationTests.Controllers
{
    public class CategoriesControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public CategoriesControllerTests(WebApplicationFactory<Startup> factory)
        {
            factory.ClientOptions.BaseAddress = new Uri("http://localhost/api/categories");
            _client = factory.CreateClient();
            // _client = factory.CreateDefaultClient(new Uri("http://localhost/api/categories"));
        }

        [Fact]
        public async Task GetAll_ReturnsSuccessfullStatusCode()
        {
            var response = await this._client.GetAsync("");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAll_ReturnsExpectedMediaType()
        {
            var response = await this._client.GetAsync("");

            Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task GetAll_ReturnsContent()
        {
            var response = await this._client.GetAsync("");

            Assert.NotNull(response.Content);
            Assert.True(response.Content.Headers.ContentLength > 0);
        }

        [Fact]
        public async Task GetAll_ReturnsExpectedJson()
        {
            var expected = new List<string> { "Accessories", "Bags", "Balls", "Clothing", "Rackets" };

            var responeStream = await _client.GetStreamAsync("");
            var model = await JsonSerializer.DeserializeAsync<ExpectedCategoriesModel>(responeStream, 
                JsonSerializerHelper.DefaultDeserialisationOptions);

            Assert.NotNull(model?.AllowedCategories);
            Assert.Equal(expected.OrderBy(s => s), model.AllowedCategories.OrderBy(s => s));
        }

        [Fact]
        public async Task GetAll_ReturnsExpectedResponse_With_SystemNetHttpJson()
        {
            /* GetFromJsonAsync also ensure Succeesfull status code, application/json header */
            var expected = new List<string> { "Accessories", "Bags", "Balls", "Clothing", "Rackets" };

            var model = await _client.GetFromJsonAsync<ExpectedCategoriesModel>("");

            Assert.NotNull(model?.AllowedCategories);
            Assert.Equal(expected.OrderBy(s => s), model.AllowedCategories.OrderBy(s => s));
        }

        [Fact]
        public async Task GetAll_SetsExpectedCacheControlHeader()
        {
            var response = await _client.GetAsync("");

            var header = response.Headers.CacheControl;

            Assert.True(header.MaxAge.HasValue);
            Assert.Equal(TimeSpan.FromMinutes(5), header.MaxAge);
            Assert.True(header.Public);
        }
    }
}
