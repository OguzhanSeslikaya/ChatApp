namespace Chat.Server.Entities.Models
{
    public class MessageInfo
    {
        public string username { get; set; }
        public Queue<Message> messages { get; set; } = new Queue<Message>();
    }
}
