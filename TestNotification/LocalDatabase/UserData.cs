using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TestNotification.LocalDatabase
{
    public class UserData
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Company { get; set; }
        public string SectorCompany { get; set; }
    }
}
