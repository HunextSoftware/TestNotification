namespace TestNotificationBackend.Models
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string Company { get; set; }
        public string SectorCompany { get; set; }

        // Send this object when data user login are not right ...
        public LoginResponse() { }

        // ... otherwise send this
        public LoginResponse(string Username, string Company, string SectorCompany)
        {
            this.Username = Username;
            this.Company = Company;
            this.SectorCompany = SectorCompany;
        }
    }
}
