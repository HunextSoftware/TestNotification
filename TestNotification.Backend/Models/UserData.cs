using System;

namespace TestNotificationBackend.Models
{
    public class UserData
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Company { get; set; }
        public string SectorCompany { get; set; }
    }
}