//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using TestNotificationWebApp.Models;

//namespace TestNotificationWebApp.Controllers
//{
//    [ApiController]
//    [Route("/send")]
//    public class FormController : ControllerBase
//    {
//        private readonly HttpClient _client;

//        private const string RequestUri = "/api/notifications/requests";

//        public FormController()
//        {
//            _client = new HttpClient();
//            _client.DefaultRequestHeaders.Add("Accept", "application/json");
//        }

//        //[BindProperty]
//        //public NotificationRequest NotificationRequest { get; set; }

//        [HttpPost]
//        public async Task<string> OnPostAsync(NotificationRequest notificationRequest)
//        {
//            _client.DefaultRequestHeaders.Add("User-Id", notificationRequest.Tag);

//            string payload;
//            if (notificationRequest.Tag != null)
//                payload = PrepareRequestNotificationPayload(notificationRequest.Text, notificationRequest.Tag);
//            else
//                payload = PrepareRequestNotificationPayload(notificationRequest.Text, "");

//            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://serverpushnotification.azurewebsites.net{RequestUri}"));

//            //string serializedContent = null;
//            //await Task.Run(() => serializedContent = JsonConvert.SerializeObject(payload)).ConfigureAwait(false);
//            //if (serializedContent != null)
//            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

//            var response = await _client.SendAsync(request).ConfigureAwait(false);

//            if (response.IsSuccessStatusCode)
//                return $"The notification has been sent with successful: status code {response.StatusCode}";
//            else
//                return $"Error {response.StatusCode}: retry again";
//        }

//        string PrepareRequestNotificationPayload(string text, string tag) => RequestNotificationTemplate.body
//            .Replace("$(textNotification)", text, StringComparison.InvariantCulture)
//            .Replace("$(tagsNotification)", tag, StringComparison.InvariantCulture);



        //public async Task<bool> RequestNotificationAsync(NotificationRequest notificationRequest, CancellationToken token)
        //{
        //    if ((!notificationRequest.Silent &&
        //            string.IsNullOrWhiteSpace(notificationRequest?.Text)))
        //        return false;

        //    var androidPushTemplate = notificationRequest.Silent ?
        //        PushTemplates.Silent.Android :
        //        PushTemplates.Generic.Android;

        //    var iOSPushTemplate = notificationRequest.Silent ?
        //        PushTemplates.Silent.iOS :
        //        PushTemplates.Generic.iOS;

        //    var androidPayload = PrepareNotificationPayload(
        //        androidPushTemplate,
        //        notificationRequest.Text);

        //    var iOSPayload = PrepareNotificationPayload(
        //        iOSPushTemplate,
        //        notificationRequest.Text);

        //    try
        //    {
        //        if (notificationRequest.Tags.Length == 0)
        //        {
        //            // This will broadcast to all users registered in the notification hub
        //            await SendPlatformNotificationsAsync(androidPayload, iOSPayload, token);
        //        }

        //        // 10 is the tag limit for any boolean expression constructed using the AND logical operator (&&) and no one OR (||)
        //        else if (notificationRequest.Tags.Length <= 10)
        //        {
        //            // Build the expression about which tags are involved in the notification, bringing tags by JSON request body
        //            var tagExpression = new System.Text.StringBuilder();
        //            for (int i = 0; i < notificationRequest.Tags.Length; i++)
        //            {
        //                if (i == 0)
        //                    tagExpression.Append("(" + notificationRequest.Tags[i]);
        //                else
        //                    tagExpression.Append(" && " + notificationRequest.Tags[i]);

        //                if ((i + 1) == notificationRequest.Tags.Length)
        //                    tagExpression.Append(")");
        //            }

        //            await SendPlatformNotificationsAsync(androidPayload, iOSPayload, tagExpression.ToString(), token);
        //        }
        //        else
        //            _logger.LogError("Error: tags number not allowed. The valid tags number is from 0 to 10.");

        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Unexpected error sending notification");
        //        return false;
        //    }
        //}

        //string PrepareNotificationPayload(string template, string text) => template
        //    .Replace("$(alertMessage)", text, StringComparison.InvariantCulture);

        //Task SendPlatformNotificationsAsync(string androidPayload, string iOSPayload, CancellationToken token)
        //{
        //    var sendTasks = new Task[]
        //    {
        //        _hub.SendFcmNativeNotificationAsync(androidPayload, token),
        //        _hub.SendAppleNativeNotificationAsync(iOSPayload, token)
        //    };

        //    return Task.WhenAll(sendTasks);
        //}

        //Task SendPlatformNotificationsAsync(string androidPayload, string iOSPayload, string tagExpression, CancellationToken token)
        //{
        //    var sendTasks = new Task[]
        //    {
        //        _hub.SendFcmNativeNotificationAsync(androidPayload, tagExpression, token),
        //        _hub.SendAppleNativeNotificationAsync(iOSPayload, tagExpression, token)
        //    };

        //    return Task.WhenAll(sendTasks);
        //}
//    }
//}
