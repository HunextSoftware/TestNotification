//Model class to save fundamental data into the push notification server

namespace TestNotification.Models
{
    public class AppDataToSend
    {

        public AppDataToSend(uint deviceId, uint orgSectorId)
        {
            DeviceId = deviceId;
            OrgSectorId = orgSectorId;
        }
        public uint DeviceId { get; set; }
        public uint OrgSectorId { get; set; }

        public string PrintAppDataToSend => $"{DeviceId} {OrgSectorId}";  
    }
}