using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using moderator_api.Core.Models;

namespace moderator_api.Core
{
    public class CognitiveClient : ICognitiveClient
    {

        private readonly ILogger<CognitiveClient> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        public CognitiveClient(ILogger<CognitiveClient> logger, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<(ModerateImageResponseModel moderateResp, TimeSpan executionTime)> ModerateImageAsync(byte[] imageBytes, string mediaType)
        {
            if (imageBytes == null)
            {
                throw new ArgumentNullException(nameof(imageBytes));
            }

            if (string.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException(nameof(mediaType));
            }

            string url = $"{_configuration["Cognitive:URL"]}/contentmoderator/moderate/v1.0/ProcessImage/Evaluate";
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Add("Ocp-Apim-Subscription-Key", _configuration["Cognitive:SubscriptionKey"]);
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response;
            var stopWatch = Stopwatch.StartNew();
            using (var content = new ByteArrayContent(imageBytes))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);
                request.Content = content;
                response = await client.SendAsync(request);
            }

            ModerateImageResponseModel moderateResponse = null;

            if (response == null || !response.IsSuccessStatusCode)
            {
                // If request fails return null ModerateImageResponse.
                return (moderateResponse, stopWatch.Elapsed);
            }
            using (var responseStream = await response.Content.ReadAsStreamAsync())
                return (await JsonSerializer.DeserializeAsync<ModerateImageResponseModel>(responseStream), stopWatch.Elapsed);
        }

        public async Task<(ModerateTextResponseModel moderateResp, TimeSpan executionTime)> ModerateTextAsync(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                throw new ArgumentNullException(nameof(body));
            }
            string url = $"{_configuration["Cognitive:URL"]}/contentmoderator/moderate/v1.0/ProcessText/Screen?classify=True";
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Add("Ocp-Apim-Subscription-Key", _configuration["Cognitive:SubscriptionKey"]);
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response;
            var stopWatch = Stopwatch.StartNew();
            using (var content = new StringContent(body))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                request.Content = content;
                response = await client.SendAsync(request);
            }

            ModerateTextResponseModel moderateResponse = null;

            if (response == null || !response.IsSuccessStatusCode)
            {
                // If request fails return null ModerateImageResponse.
                return (moderateResponse, stopWatch.Elapsed);
            }

            using (var responseStream = await response.Content.ReadAsStreamAsync())
                return (await JsonSerializer.DeserializeAsync<ModerateTextResponseModel>(responseStream), stopWatch.Elapsed);
        }
    }
}
