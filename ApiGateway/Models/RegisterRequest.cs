namespace ApiGateway.Models
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public DateTimeOffset SubmitTime { get; set; } = DateTimeOffset.UtcNow;
    }
}
