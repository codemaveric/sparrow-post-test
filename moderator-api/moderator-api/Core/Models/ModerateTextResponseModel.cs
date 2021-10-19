using System;
using System.Collections.Generic;

namespace moderator_api.Core.Models
{
    public class PIIModel
    {
        public string Detected { get; set; }
        public string SubType { get; set; }
        public string CountryCode { get; set; }
        public string Text { get; set; }
        public int Index { get; set; }
    }

    public class PII
    {
        public List<PIIModel> Email { get; set; }
        public List<PIIModel> IPA { get; set; }
        public List<PIIModel> Phone { get; set; }
        public List<PIIModel> Address { get; set; }
    }

    public class Category
    {
        public double Score { get; set; }
    }

    public class Classification
    {
        public Category Category1 { get; set; }
        public Category Category2 { get; set; }
        public Category Category3 { get; set; }
        public bool ReviewRecommended { get; set; }
    }

    public class Terms
    {
        public int Index { get; set; }
        public int OriginalIndex { get; set; }
        public int ListId { get; set; }
        public string Term { get; set; }
    }

    public class Status
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public object Exception { get; set; }
    }

    public class ModerateTextResponseModel
    {
        public string OriginalText { get; set; }
        public string NormalizedText { get; set; }
        public string AutoCorrectedText { get; set; }
        public object Misrepresentation { get; set; }
        public PII PII { get; set; }
        public Classification Classification { get; set; }
        public string Language { get; set; }
        public List<Terms> Terms { get; set; }
        public Status Status { get; set; }
        public string TrackingId { get; set; }
    }
}
