﻿using System;
using System.Threading.Tasks;

namespace TestNotification.Services
{
    //This will handle the interaction between the client and backend service.
    public interface INotificationRegistrationService
    {
        //Need this methods to link them to server endpoints
        Task DeregisterDeviceAsync();
        Task RegisterDeviceAsync(string guid);
        Task RefreshRegistrationAsync();
    }
}
