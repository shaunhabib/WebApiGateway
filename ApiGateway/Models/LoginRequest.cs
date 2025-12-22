using PEPCore;

namespace ApiGateway.Models
{
    public class LoginRequest : MessageBase
    {
        public int MessageId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public int NumOfReq { get; set; }
        public TimeSpan? SubmittedTimeFromGW { get; set; }
    }
}
