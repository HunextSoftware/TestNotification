namespace TestNotification.Services
{
    //This is used as a simple mechanism to centralize the handling of notification actions.
    public interface INotificationActionService
    {
        void TriggerAction(string action);
    }
}
