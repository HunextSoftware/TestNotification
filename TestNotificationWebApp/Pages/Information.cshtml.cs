using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestNotificationWebApp.Models;

namespace TestNotificationWebApp.Pages
{
    public class InformationModel : PageModel
    {
        [BindProperty]
        public int TotalRegistration { get; set; } = 0;
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

            TotalRegistration = registrations.Count;

            for(int i = 0; i < TotalRegistration; i++)
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

        //Console.WriteLine($"{registrations[0].RegistrationId}\n {array[0]}, {array[1]}\n  {registrations[0].ExpirationTime}\n {registrations[0].PnsHandle}\n {registrations[0].Serialize()}");

        //var allRegistrations = await _hub.GetAllRegistrationsAsync(0, CancellationToken.None);
        //var continuationToken = allRegistrations.ContinuationToken;
        //var registrationDescriptionsList = new List<RegistrationDescription>(allRegistrations);

        //while (!string.IsNullOrWhiteSpace(continuationToken))
        //{
        //    var otherRegistrations = await _hub.GetAllRegistrationsAsync(continuationToken, 0);
        //    registrationDescriptionsList.AddRange(otherRegistrations);
        //    continuationToken = otherRegistrations.ContinuationToken;
        //}

        //return Page();
    }
}