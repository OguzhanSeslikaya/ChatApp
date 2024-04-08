using Chat.Lobby.Server;
using Chat.Server.Entities.Lists;
using Chat.Server.Entities.Models;
using Chat.Server.Services.Data;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace Chat.Server.Services.Proto
{
    public class LobbyService(IdentityService identityService, TokenService tokenService, ChatRoomList chatRoomList) : Lobby.Server.Lobby.LobbyBase
    {
        public override async Task<CreateUserResponse> register(CreateUserRequest request, ServerCallContext context)
        {
            bool succeeded = await identityService.writeUserAsync(new Entities.Models.AppUser()
            { username = request.Username, password = request.Password });
            return new CreateUserResponse()
            {
                Succeeded = succeeded,
                Message = (succeeded ? "Kaydınız başarı ile yapıldı." : "Kaydınız yapılamadı.")
            };
        }

        public override async Task<LoginUserResponse> login(LoginUserRequest request, ServerCallContext context)
        {
            AppUser user = new AppUser() { username = request.Username, password = request.Password };
            bool control = await identityService.loginAsync(user);
            return new LoginUserResponse()
            {
                Succeeded = control,
                Message = (control ? "Başarılı" : "Başarısız"),
                Token = (control ? tokenService.createToken(180, user).accessToken : "")
            };
        }

        [Authorize]
        public override Task<JoinChatResponse> joinChat(JoinChatRequest request, ServerCallContext context)
        {
            string username = context.GetHttpContext().User.Identity.Name;
            Entities.Models.ChatRoom? chatRoom = chatRoomList.Where(a => a.id == request.ChatId && a.password == request.ChatPasswd).FirstOrDefault();
            addMessageInfo(chatRoom, username);
            addVoiceInfo(chatRoom, username);
            return Task.FromResult(new JoinChatResponse()
            {
                Succeeded = (chatRoom == null ? false : true),
                ChatId = (chatRoom == null ? 0 : chatRoom.id),
                Message = (chatRoom == null ? "Şifre veya chat id yanlış." : "Sohbet odasına giriş yapıldı.")
            });
        }

        [Authorize]
        public override Task<CreateChatResponse> createChat(CreateChatRequest request, ServerCallContext context)
        {
            string username = context.GetHttpContext().User.Identity.Name;
            Random rnd = new Random();
            int _id;
            while (true)
            {
                _id = rnd.Next(1000, 9999);
                if (chatRoomList.FirstOrDefault(a => a.id == _id) == null)
                    break;
            }
            Entities.Models.ChatRoom chatRoom = new Entities.Models.ChatRoom() { id = _id, password = request.Password };
            addMessageInfo(chatRoom, username);
            addVoiceInfo(chatRoom, username);
            chatRoomList.Add(chatRoom);
            return Task.FromResult(new CreateChatResponse()
            {
                Succeeded = true,
                Message = "Sohbet odası başarı ile oluşturuldu.",
                ChatId = _id
            });
        }


        ///
        private void addMessageInfo(Entities.Models.ChatRoom chatRoom,string username)
        {
            chatRoom.messageInfos.Add(new MessageInfo()
            {
                username = username
            });
        }

        private void addVoiceInfo(Entities.Models.ChatRoom chatRoom, string username)
        {
            chatRoom.voiceInfos.Add(new VoiceInfo()
            {
                username = username
            });
        }
    }
}