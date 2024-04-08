using Chat.ChatRoom.Server;
using Chat.Server.Entities.Lists;
using Chat.Server.Entities.Models;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;

namespace Chat.Server.Services.Proto
{
    [Authorize]
    public class ChatRoomService(ChatRoomList chatRoomList) : ChatRoom.Server.ChatRoom.ChatRoomBase
    {
        public override Task<Empty> sendText(SendTextRequest request, ServerCallContext context)
        {
            Entities.Models.ChatRoom? chatRoom = chatRoomList.Where(a => a.id == request.ChatId).FirstOrDefault();
            string date = DateTime.Now.ToShortTimeString();
            if (chatRoom != null)
            {
                try
                {
                    foreach (var messageInfo in chatRoom.messageInfos)
                    {
                        date = DateTime.Now.ToShortTimeString();
                        messageInfo.messages.Enqueue(new Message()
                        {
                            date = date,
                            username = context.GetHttpContext().User.Identity.Name,
                            message = request.Message
                        });
                    }
                }
                catch
                {
                }
            }
            return Task.FromResult(new Empty());
        }
        public override async Task textChatStream(Empty request, IServerStreamWriter<TextChatStreamResponse> responseStream, ServerCallContext context)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                string username = context.GetHttpContext().User.Identity.Name;
                MessageInfo? messageInfo = chatRoomList.Select(a => a.messageInfos.Where(b => b.username == username).FirstOrDefault()).FirstOrDefault();
                while (true)
                {
                    if (messageInfo != null)
                    {
                        if (messageInfo.messages.Any())
                        {
                            Message message = messageInfo.messages.Dequeue();
                            await responseStream.WriteAsync(new TextChatStreamResponse()
                            {
                                Date = message.date,
                                Username = message.username,
                                Message = message.message
                            });
                        }
                    }
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }
                    await Task.Delay(1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override async Task voiceChatStream(Empty request, IServerStreamWriter<VoiceChatStreamResponse> responseStream, ServerCallContext context)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                string username = context.GetHttpContext().User.Identity.Name;
                VoiceInfo? voiceInfo = chatRoomList.Select(a => a.voiceInfos.Where(b => b.username == username).FirstOrDefault()).FirstOrDefault();
                while (true)
                {
                    if (voiceInfo != null)
                    {
                        if (voiceInfo.voices.Any())
                        {
                            Voice voice = voiceInfo.voices.Dequeue();
                            await responseStream.WriteAsync(new VoiceChatStreamResponse()
                            {
                                VoiceBytes = ByteString.CopyFrom(voice.bytes)
                            });
                        }
                    }
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public override async Task<Empty> sendVoiceStream(IAsyncStreamReader<SendVoiceStreamRequest> requestStream, ServerCallContext context)
        {
            SendVoiceStreamRequest current = new SendVoiceStreamRequest();
            var tokenSource = new CancellationTokenSource();
            string username = context.GetHttpContext().User.Identity.Name;
            Entities.Models.ChatRoom? chatRoom = chatRoomList
                .Where(a => a.voiceInfos.Where(a => a.username == username).Any()).FirstOrDefault();
            while (await requestStream.MoveNext(tokenSource.Token))
            {
                current = requestStream.Current;
                foreach (var item in chatRoom.voiceInfos)
                {
                    if (item.username != username)
                    {
                        item.voices.Enqueue(new()
                        {
                            bytes = current.VoiceBytes.ToByteArray()
                        });
                    }
                }
            }
            return new();
        }
    }
}
