using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestNotificationWebApp.Models;

namespace TestNotificationWebApp.Pages
{
    public class InformationModel : PageModel
    {
        [BindProperty]
        public List<RegistrationData> Data { get; set; } = new List<RegistrationData>();
        readonly NotificationHubClient _hub;


        public InformationModel(IOptions<NotificationHubOptions> options)
        {
            _hub = NotificationHubClient.CreateClientFromConnectionString(
                options.Value.ConnectionString,
                options.Value.Name);
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var registrations = new List<RegistrationDescription>(await _hub.GetAllRegistrationsAsync(0, CancellationToken.None));

            for(int i = 0; i < registrations.Count; i++)
            {
                RegistrationData registrationData = new RegistrationData
                {
                    RegistrationId = registrations[i].RegistrationId,
                    ExpirationTime = registrations[i].ExpirationTime
                };

                HashSet<string> tags = (HashSet<string>)registrations[i].Tags;
                tags.CopyTo(registrationData.Tags);

                Data.Add(registrationData);
            }

            return Page();
        }
    }
}