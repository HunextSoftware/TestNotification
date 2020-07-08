namespace TestNotificationBackend.Models
{
    public class LoginResponse
    {
        public bool isThereUser { get; set; }
        public string GUID { get; set; }
        public string Username { get; set; }
        public string Company { get; set; }
        public string SectorCompany { get; set; }

        // Send this object when data user login are not right ...
        public LoginResponse(bool isThereUser)
        {
            this.isThereUser = isThereUser;
        }

        // ...otherwise send this
        public LoginResponse(bool isThereUser, string GUID, string Username, string Company, string SectorCompany)
        {
            this.isThereUser = isThereUser;
            this.GUID = GUID;
            this.Username = Username;
            this.Company = Company;
            this.SectorCompany = SectorCompany;
        }

    }
}
