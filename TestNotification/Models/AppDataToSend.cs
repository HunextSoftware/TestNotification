using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace TestNotification.Models
{
    public class AppDataToSend
    {
        private uint deviceID { get; set; }
        private uint orgSectorID { get; set; }

        public AppDataToSend(uint deviceID, uint orgSectorID)
        {
            this.deviceID = deviceID;
            this.orgSectorID = orgSectorID;
        }
    }
}