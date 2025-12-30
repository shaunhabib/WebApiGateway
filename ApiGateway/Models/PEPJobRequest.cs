using PEPCore;

namespace ApiGateway.Models
{
    public class PEPJobRequest : MessageBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }

        public bool RequiresPickup { get; set; }
        public string AuthorizedBy { get; set; }

        public DateTimeOffset? AvailableFrom { get; set; }
        public DateTimeOffset? ClosingTime { get; set; }
        public string PickupInstructions { get; set; }
        public DateTimeOffset? GatewayTimestamp { get; set; }
    }
}
