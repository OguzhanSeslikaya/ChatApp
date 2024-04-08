using System.Collections;

namespace Chat.Server.Entities.Lists
{
    public class ChatRoomList : IEnumerable<Models.ChatRoom>
    {
        List<Models.ChatRoom> chatRooms = new();
        public void Add(Models.ChatRoom model) => chatRooms.Add(model);
        public IEnumerator<Models.ChatRoom> GetEnumerator() => chatRooms.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => chatRooms.GetEnumerator();
    }
}
