using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Client.Entities.Models
{
    public class ChatInformation
    {
        public int chatId { get; set; } = 0;
        public Queue<byte[]> voiceBytesToSend { get; set; } = new Queue<byte[]>();
        public string username { get; set; }

    }
}
