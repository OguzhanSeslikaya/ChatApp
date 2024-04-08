namespace Chat.Server.Entities.Models
{
    public class Token
    {
        public string accessToken { get; set; }
        public DateTime expiration { get; set; }
    }
}
