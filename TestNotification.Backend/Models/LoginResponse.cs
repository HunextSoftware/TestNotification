using System;

namespace TestNotificationBackend.Models
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string Company { get; set; }
        public string SectorCompany { get; set; }

        public Guid Id { get; set; }

        // Send this object when data user login are not right ...
        public LoginResponse() { }

        // ... otherwise send this
        public LoginResponse(string username, string company, string sectorCompany, Guid id)
        {
            Username = username;
            Company = company;
            SectorCompany = sectorCompany;
            Id = id;
        }
    }
}
