using System;
namespace moderator_api.Core.Models
{
    public class ErrorMessagesModel
    {
        public const string HTML_TEXT_TOO_LONG = "We currently unable to process text greater than 1028. Please provide HTML text less or equal than 1028 character length.";
        public const string IMAGE_TEXT_REQUIRED = "Image and HTML text is required to process this request";
        public const string FILE_NOT_IMAGE = "File provided must be an image with the following extension .jpeg, .jpg, .png, .gif and .bmp";
        public const string IMAGE_SHOULD_BE_LESS_THAN_4_MB = "We are not able to process image that is greater than 4 mb. Please provide image less than 4 mb";
        public const string UNABLE_TO_PROCESS_TEXT_REQUEST = "Unable to process text moderation, Please try again later.";
        public const string UNABLE_TO_PROCESS_IMAGE_REQUEST = "Unable to process image moderation, Please try again later.";
    }
}
