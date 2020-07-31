using System;

namespace TestNotificationWebApp.Models
{
    public class UserData
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Company { get; set; }
        public string SectorCompany { get; set; }

        public UserData(Guid id, string username)
        {
            Id = id;
            Username = username;
        }
    }
}