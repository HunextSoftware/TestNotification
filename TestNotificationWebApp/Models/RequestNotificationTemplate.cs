namespace TestNotificationWebApp.Models
{
    /* 
     * This class will be useful when Hunext developers implement this POC into Hunext Mobile app (for the POC, use Postman).
     * The body string represent the JSON content/body of a notification HTTP(S) request, which will be implemented by the back end when some news occur.
     * 
     * The fields are the following:
     * 
     * - text: the message visible in the notification, it has no character number limits.
     *   => example: "text" = "Hey man! How are things?"
     *   
     * - tags: a vector of tags who permit to target notifications to specific users.
     *   => standard example (company && sectorCompany): "tags" = [ "HunextHRSolutions", "Software" ]
     *   => real example (guid user): "tags" = [ "x113k8j0-c4ad-004f-bc4e-71242341298b" ]
     *   => cardinality: 0 <= tags <= 10 */
    public class RequestNotificationTemplate
    {
        public const string body = "{ \"text\": \"$(textNotification)\", \"tags\": [ \"$(tagsNotification)\" ] }";
    }
}
