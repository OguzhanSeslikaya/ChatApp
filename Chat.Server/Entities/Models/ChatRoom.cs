namespace Chat.Server.Entities.Models
{
    public class ChatRoom
    {
        public int id { get; set; }
        public string password { get; set; }
        public List<MessageInfo> messageInfos { get; set; } = new List<MessageInfo>();
        public List<VoiceInfo> voiceInfos { get; set; } = new List<VoiceInfo>();
    }
}
