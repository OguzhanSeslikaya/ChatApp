namespace Chat.Server.Entities.Models
{
    public class VoiceInfo
    {
        public string username { get; set; }
        public Queue<Voice> voices { get; set; } = new Queue<Voice>();
    }
}
