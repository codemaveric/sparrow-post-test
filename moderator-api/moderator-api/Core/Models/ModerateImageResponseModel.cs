using System;
namespace moderator_api.Core.Models
{
    public class ModerateImageResponseModel
    {
        public double AdultClassificationScore { get; set; }
        public bool IsImageAdultClassified { get; set; }
        public double RacyClassificationScore { get; set; }
        public bool IsImageRacyClassified { get; set; }
        public object[] AdvancedInfo { get; set; }
        public bool Result { get; set; }
        public ModerateStatus Status { get; set; }
        public string TrackingId { get; set; }
    }

    public class ModerateStatus
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public string Exception { get; set; }
    }
}
