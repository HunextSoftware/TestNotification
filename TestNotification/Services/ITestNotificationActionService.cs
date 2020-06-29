using System;
using TestNotification.Models;

namespace TestNotification.Services
{
    //This type is specific to the TestNotification application and uses the TestNotificationAction enumeration to identify the action that is being triggered in a strongly-typed manner.
    public interface ITestNotificationActionService : INotificationActionService
    {
        event EventHandler<TestNotificationAction> ActionTriggered;
    }

}
