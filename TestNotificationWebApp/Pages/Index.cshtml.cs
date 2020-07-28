using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestNotificationWebApp.Configuration;
using TestNotificationWebApp.Models;

namespace TestNotificationWebApp.Pages
{
    public class IndexModel : PageModel
    {
        readonly HttpClient _client;
        readonly string _baseApiUrl;
        const string RequestUriGet = "/api/notifications/users/all";
        const string RequestUriPost = "/api/notifications/requests";

        public static bool isVisibleOk = false;
        public static bool isVisibleError = false;

        readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public NotificationRequest NotificationRequest { get; set; }
        [BindProperty]
        public List<UserData> UserData { get; set; } = new List<UserData>();

   
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _baseApiUrl = Config.BackendServiceEndpoint;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (ModelState.IsValid)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_baseApiUrl}{RequestUriGet}"));
                var response = await _client.SendAsync(request).ConfigureAwait(false);

                UserData = JsonConvert.DeserializeObject<List<UserData>>(await response.Content.ReadAsStringAsync());
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_baseApiUrl}{RequestUriPost}"))
            {
                Content = new StringContent(PrepareRequestNotificationPayload(NotificationRequest.Text, NotificationRequest.Tag), Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                isVisibleOk = true;
                isVisibleError = false;
                _logger.LogDebug($"{response.StatusCode}: the notification has been sent with successful.");
            }
            else if (((int)response.StatusCode).Equals(422))
            {
                isVisibleOk = true;
                isVisibleError = false;
                _logger.LogWarning($"{response.StatusCode}: the notification has been sent correctly to all Android devices. Please, set Apple Platform Notification (APN) by Azure Hub Notification to receive OK status code.");
            }
            else
            {
                isVisibleOk = false;
                isVisibleError = true;
                _logger.LogError($"An error occured: {response.StatusCode}. Please, retry again.");
            }

            return RedirectToPage("./Index");
        }

        string PrepareRequestNotificationPayload(string text, string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return NotificationRequestTemplate.body
                    .Replace("$(textNotification)", text, StringComparison.InvariantCulture)
                    .Replace(" \"$(tagsNotification)\" ", string.Empty, StringComparison.InvariantCulture);
            //.Replace(", \"tags\": [ \"$(tagsNotification)\" ] ", string.Empty, StringComparison.InvariantCulture); --> removing "tags" key, we will obtain the same result of the row above
            else
                return NotificationRequestTemplate.body
                    .Replace("$(textNotification)", text, StringComparison.InvariantCulture)
                    .Replace("$(tagsNotification)", tag, StringComparison.InvariantCulture);
        }
    }
}
