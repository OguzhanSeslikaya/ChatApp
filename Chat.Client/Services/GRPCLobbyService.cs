using Chat.Lobby.Client;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chat.ChatRoom.Client.ChatRoom;
using static Chat.Lobby.Client.Lobby;

namespace Chat.Client.Services
{
    public class GRPCLobbyService(LobbyClient lobbyClient)
    {
        public async Task<CreateUserResponse> registerAsync(CreateUserRequest createUserRequest)
        {
            if (!string.IsNullOrWhiteSpace(createUserRequest.Username))
                if (!string.IsNullOrWhiteSpace(createUserRequest.Password))
                    return await lobbyClient.registerAsync(createUserRequest);
            return new CreateUserResponse() { Succeeded = false, Message = "Kullanıcı adı veya şifre uygun değil." };
        }
        public async Task<LoginUserResponse> loginAsync(LoginUserRequest loginUserRequest)
        {
            if (!string.IsNullOrWhiteSpace(loginUserRequest.Username))
                if (!string.IsNullOrWhiteSpace(loginUserRequest.Password))
                    return await lobbyClient.loginAsync(loginUserRequest);
            return new LoginUserResponse() { Succeeded = false, Message = "Kullanıcı adı veya şifre uygun değil." };
        }
        public async Task<CreateChatResponse> createChatAsync(CreateChatRequest createChatRequest,Metadata headers)
        {
            if (!string.IsNullOrWhiteSpace(createChatRequest.Password))
                return await lobbyClient.createChatAsync(createChatRequest, headers: headers);
            return new CreateChatResponse() { Succeeded = false, Message = "Sohbet oluşturma işlemi başarısız." };
        }
        public async Task<JoinChatResponse> joinChat(JoinChatRequest joinChatRequest,Metadata headers)
        {
            if (joinChatRequest.ChatId > 0)
                if (!string.IsNullOrWhiteSpace(joinChatRequest.ChatPasswd))
                    return await lobbyClient.joinChatAsync(joinChatRequest, headers: headers);
            return new JoinChatResponse() { Succeeded = false, Message = "Sohbete katılma isteği başarısız.", ChatId = 0 };
        }
    }
}
