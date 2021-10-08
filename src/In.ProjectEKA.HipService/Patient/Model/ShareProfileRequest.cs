using System;
using System.ComponentModel.DataAnnotations;


namespace In.ProjectEKA.HipService.Patient.Model
{
    public class ShareProfileRequest
    {
        [Required]
        public string RequestId { get;}
        [Required]
        public DateTime? Timestamp { get;}
        public Profile Profile { get; }
        public ShareProfileRequest(string requestId,DateTime? timestamp, Profile profile)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Profile = profile;
        }
    }
}