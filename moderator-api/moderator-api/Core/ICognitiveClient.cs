using System;
using System.Threading.Tasks;
using moderator_api.Core.Models;

namespace moderator_api.Core
{
    public interface ICognitiveClient
    {
        Task<(ModerateTextResponseModel moderateResp, TimeSpan executionTime)> ModerateTextAsync(string body);
        Task<(ModerateImageResponseModel moderateResp, TimeSpan executionTime)> ModerateImageAsync(byte[] imageBytes, string mediaType);
    }
}
