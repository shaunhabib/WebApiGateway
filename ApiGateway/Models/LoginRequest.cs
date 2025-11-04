namespace ApiGateway.Models
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public DateTimeOffset SubmittedDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
