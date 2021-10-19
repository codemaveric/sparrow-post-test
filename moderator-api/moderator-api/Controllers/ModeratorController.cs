using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moderator_api.BindingModels;
using moderator_api.Core;
using moderator_api.Core.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace moderator_api.Controllers
{
    [Route("api/[controller]")]
    public class ModeratorController : Controller
    {
        private readonly ILogger<ModeratorController> _logger;
        private readonly ICognitiveClient _cognitiveClient;
        public ModeratorController(ILogger<ModeratorController> logger, ICognitiveClient cognitiveClient)
        {
            _logger = logger;
            _cognitiveClient = cognitiveClient;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post(IFormCollection formData)
        {
          
            if (formData == null || formData.Files.Count != 1 || string.IsNullOrEmpty(formData["htmlText"]))
            {
                // Return Error
                return BadRequest(ErrorMessagesModel.IMAGE_TEXT_REQUIRED);
            }


            if (formData["htmlText"].ToString().Length > 1024)
            {
                return BadRequest(ErrorMessagesModel.HTML_TEXT_TOO_LONG);
            }

            var imageFile = formData.Files["uploadFile"];

            string ext = System.IO.Path.GetExtension(imageFile.FileName);
            (bool success, string mediaType) = ValidateExt(ext);
            if (!success)
            {
                return BadRequest(ErrorMessagesModel.FILE_NOT_IMAGE);
            }

            if (imageFile.Length / (1024 * 1024) > 4)
            {
                return BadRequest(ErrorMessagesModel.IMAGE_SHOULD_BE_LESS_THAN_4_MB);
            }

            byte[] imageBytes = null;

            if (imageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    imageFile.CopyTo(ms);
                    imageBytes = ms.ToArray();
                }
            }

            var imageModeratorResponseTask = _cognitiveClient.ModerateImageAsync(imageBytes, mediaType);

            var textModeratorResponse = await _cognitiveClient.ModerateTextAsync(formData["htmlText"]);
            if (textModeratorResponse.moderateResp == null)
            {
                return BadRequest(ErrorMessagesModel.UNABLE_TO_PROCESS_TEXT_REQUEST);
            }
            var imageModeratorResponse = await imageModeratorResponseTask;
            if (imageModeratorResponse.moderateResp == null)
            {
                return BadRequest(ErrorMessagesModel.UNABLE_TO_PROCESS_IMAGE_REQUEST);
            }
            return Ok(new {
                ImageResult = new
                {
                    ExecutionResult = imageModeratorResponse.moderateResp,
                    ExecutionTime = imageModeratorResponse.executionTime.TotalMilliseconds
                },
                TextResult = new
                {
                    ExecutionResult = textModeratorResponse.moderateResp,
                    ExecutionTime = textModeratorResponse.executionTime.TotalMilliseconds
                },

            });
        }

        private (bool success, string mediaType) ValidateExt(string ext)
        {
            return ext switch
            {
                ".jpeg" => (true, "image/jpeg"),
                ".jpg" => (true, "image/jpeg"),
                ".gif" => (true, "image/gif"),
                ".png" => (true, "image/png"),
                ".bmp" => (true, "image/bmp"),
                _ => (false, string.Empty)
            };
        }
    }
}
