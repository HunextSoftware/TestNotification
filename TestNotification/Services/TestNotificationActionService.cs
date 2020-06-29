using System;
using System.Collections.Generic;
using System.Linq;
using TestNotification.Models;

namespace TestNotification.Services
{
    public class TestNotificationActionService : ITestNotificationActionService
    {
        readonly Dictionary<string, TestNotificationAction> _actionMappings = new Dictionary<string, TestNotificationAction>
        {
            { "action_a", TestNotificationAction.ActionA },
            { "action_b", TestNotificationAction.ActionB }
        };

        public event EventHandler<TestNotificationAction> ActionTriggered = delegate { };

        public void TriggerAction(string action)
        {
            if (!_actionMappings.TryGetValue(action, out var testNotificationAction))
                return;

            List<Exception> exceptions = new List<Exception>();

            foreach (var handler in ActionTriggered?.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(this, testNotificationAction);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }
    }
}
