using Chat.ChatRoom.Client;
using Chat.Client.Entities.Models;
using Chat.Lobby.Client;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Client.Services
{
    public class GRPCInstanceBuilderService(ChatInformation chatInformation)
    {
        private string _accessToken { get; set; } = "";
        public string accessToken
        {
            get => _accessToken;
            set => _accessToken = value;
        }
        public Metadata headersBuilder()
        {
            Metadata headers = new Metadata();
            headers.Add("Authorization", $"Bearer {_accessToken}");
            return headers;
        }
        public CreateUserRequest createUserRequestBuilder()
        {
            Console.Write("Oluşturmak istediğiniz kullanıcı adını giriniz: ");
            string username = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Oluşturmak istediğiniz şifreyi giriniz: ");
            string password = Console.ReadLine();
            Console.WriteLine();
            return new CreateUserRequest()
            {
                Password = password,
                Username = username
            };
        }
        public LoginUserRequest loginUserRequestBuilder()
        {
            Console.Write("kullanıcı adınızı giriniz: ");
            string username = Console.ReadLine();
            Console.WriteLine();
            chatInformation.username = username;
            Console.Write("şifrenizi giriniz: ");
            string password = Console.ReadLine();
            Console.WriteLine();
            return new LoginUserRequest()
            {
                Username = username,
                Password = password
            };
        }
        public JoinChatRequest joinChatRequestBuilder()
        {
            Console.Write("Sohbet id sini giriniz: ");
            string chatId = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Sohbet şifresini giriniz: ");
            string password = Console.ReadLine();
            Console.WriteLine();
            return new JoinChatRequest()
            {
                ChatId = int.Parse(chatId),
                ChatPasswd = password
            };
        }
        public CreateChatRequest createChatRequsetBuilder()
        {
            Console.Write("Sohbet şifresini giriniz: ");
            string password = Console.ReadLine();
            Console.WriteLine();
            return new CreateChatRequest()
            {
                Password = password
            };
        }
        public SendTextRequest sendTextRequestBuilder()
        {
            string message = Console.ReadLine();
            return new SendTextRequest()
            {
                Message = message,
                ChatId = chatInformation.chatId
            };
        }
    }
}
