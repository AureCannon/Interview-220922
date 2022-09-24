using System.Net;
using System.Text.Json;
using Ct.Domain.Exceptions;
using Ct.Interview.Web.Api.Contracts;
using Ct.Interview.Web.Api.Middlewares;
using FluentAssertions;

namespace Ct.Web.Api.Tests
{
    public class AsxListedCompaniesIntegrationTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly HttpClient _httpClient;

        public AsxListedCompaniesIntegrationTests(TestFixture testFixture)
        {
            _httpClient = testFixture.Client;
        }

        [Theory]
        [InlineData("MOQX")]
        [InlineData("moqx")]
        [InlineData("MoqX")]
        public async Task Should_fetch_asx_listed_companies_Async(string asxCode)
        {
            //Arrange          
            var expectedResult = new AsxListedCompanyResponse
            {
                AsxCode = "MOQX",
                CompanyName = "MOQ LIMITED"
            };

            // Act
            var result = await SendRequestAsync(asxCode);

            // Assert
            result.Count().Should().Be(1);
            result[0].Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task Should_throw_when_asx_code_is_not_found()
        {
            //Arrange
            const string expected = "ASX code does not exist.";

            // Act
            var ex = await Assert.ThrowsAsync<RecordNotFoundException>(async () => await SendRequestAsync("dummyCode"));

            // Assert

            ex.Message.Should().BeEquivalentTo(expected);
        }

        private async Task<AsxListedCompanyResponse[]> SendRequestAsync(string asxCode)
        {
            var baseUri = new Uri("https://localhost/api/");
            var uri = new Uri(baseUri, $"AsxListedCompanies/{asxCode}");

            using var response = await _httpClient.GetAsync(uri);

            await ValidateAsync(response);

            return await ReadJsonAsync<AsxListedCompanyResponse[]>(response);
        }

        private static async Task ValidateAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStreamAsync();
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var error = await JsonSerializer.DeserializeAsync<AsxExceptionDto>(json, jsonOptions);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new RecordNotFoundException(error.Message);

                throw new Exception(error.Message);
            }
        }

        private static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStreamAsync();

            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            return await JsonSerializer.DeserializeAsync<T>(json, jsonOptions);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public Task InitializeAsync() => Task.CompletedTask;
    }
}