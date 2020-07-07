using LiteDB;
using System;

namespace TestNotification.Models
{
    public class UserData
    {
        [BsonId]
        public string GUID { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Company { get; set; }
        public string SectorCompany { get; set; }
    }
}