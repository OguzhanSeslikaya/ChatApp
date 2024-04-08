using Chat.ChatRoom.Client;
using Chat.Client.Entities.Models;
using Chat.Lobby.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chat.Lobby.Client.Lobby;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chat.Client.Services
{
    public class UIService(
        GRPCLobbyService gRPCLobbyService,
        GRPCInstanceBuilderService gRPCInstanceBuilderService,
        ChatInformation chatInformation,
        GRPCChatRoomService gRPCChatRoomService)
    {
        public void settingsHandler()
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        public async Task uIHandler()
        {
            int number = 9;
            while (number > 0)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(gRPCInstanceBuilderService.accessToken))
                    {
                        loginSelection(out number);
                        await loginSwitchAsync(number);
                    }
                    else
                    {
                        outerChatSelection(out number);
                        bool succeeded = await outerChatSwitch(number);
                        if (succeeded)
                        {
                            innerChatSelection(out number);
                            await innerChatSwitch(number);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        ///

        private void outerChatSelection(out int number)
        {
            Console.WriteLine("***************");
            Console.WriteLine("Chate katılmak için 1");
            Console.WriteLine("Chat oluşturmak için 2");
            Console.WriteLine("***************");
            Console.Write("Seçmek istediğiniz seçeneğin numarasını giriniz: ");
            number = int.Parse(Console.ReadLine());
        }

        private async Task<bool> outerChatSwitch(int number)
        {
            switch (number)
            {
                case 1:
                    Console.WriteLine("******************");
                    JoinChatResponse joinChatResponse = await gRPCLobbyService.joinChat(gRPCInstanceBuilderService.joinChatRequestBuilder(), gRPCInstanceBuilderService.headersBuilder());
                    if (joinChatResponse.Succeeded)
                    {
                        chatInformation.chatId = joinChatResponse.ChatId;
                        Console.WriteLine("Chat Id: " + joinChatResponse.ChatId);
                        return true;
                    }
                    else
                        Console.WriteLine(joinChatResponse.Succeeded + " " + joinChatResponse.Message);
                    break;
                case 2:
                    Console.WriteLine("******************");
                    CreateChatResponse createChatResponse = await gRPCLobbyService.createChatAsync(gRPCInstanceBuilderService.createChatRequsetBuilder(), gRPCInstanceBuilderService.headersBuilder());
                    if (createChatResponse.Succeeded)
                    {
                        chatInformation.chatId = createChatResponse.ChatId;
                        Console.WriteLine("Chat Id: " + createChatResponse.ChatId);
                        return true;
                    }
                    else
                        Console.WriteLine(createChatResponse.Succeeded + " " + createChatResponse.Message);
                    break;
                default:
                    break;
            }
            return false;
        }

        private void innerChatSelection(out int number)
        {
            Console.WriteLine("***************");
            Console.WriteLine("Sesli sohbete katılmak için 1");
            Console.WriteLine("Yazılı sohbete katılmak için 2");
            Console.WriteLine("***************");
            Console.Write("Seçmek istediğiniz seçeneğin numarasını giriniz: ");
            number = int.Parse(Console.ReadLine());
        }

        private async Task innerChatSwitch(int number)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            switch (number)
            {
                case 1:
                    Console.WriteLine("sesli sohbet");
                    gRPCChatRoomService.joinVoiceChatStream(gRPCInstanceBuilderService.headersBuilder());
                    //if (chatInformation.username == "admin")
                    gRPCChatRoomService.sendVoiceStream(gRPCInstanceBuilderService.headersBuilder());
                    while (true)
                    {
                        await Task.Delay(1000);
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    break;
                case 2:
                    Console.WriteLine("yazılı sohbet");
                    gRPCChatRoomService.joinTextChatStream(gRPCInstanceBuilderService.headersBuilder());
                    while (true)
                    {
                        SendTextRequest sendTextRequest = gRPCInstanceBuilderService.sendTextRequestBuilder();
                        removeLine();
                        await gRPCChatRoomService.sendTextAsync(sendTextRequest, gRPCInstanceBuilderService.headersBuilder());
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void loginSelection(out int number)
        {
            Console.WriteLine("***************");
            Console.WriteLine("Giriş yapmak için 1");
            Console.WriteLine("Kayıt olmak için 2");
            Console.WriteLine("***************");
            Console.Write("Seçmek istediğiniz seçeneğin numarasını giriniz: ");
            number = int.Parse(Console.ReadLine());
        }

        private async Task loginSwitchAsync(int number)
        {
            switch (number)
            {
                case 1:
                    Console.WriteLine("******************");
                    LoginUserResponse loginUserResponse = await gRPCLobbyService.loginAsync(gRPCInstanceBuilderService.loginUserRequestBuilder());
                    gRPCInstanceBuilderService.accessToken = loginUserResponse.Token;
                    Console.WriteLine(loginUserResponse.Succeeded + " " + loginUserResponse.Message);
                    break;
                case 2:
                    Console.WriteLine("******************");
                    CreateUserResponse createUserResponse = await gRPCLobbyService.registerAsync(gRPCInstanceBuilderService.createUserRequestBuilder());
                    Console.WriteLine(createUserResponse.Succeeded + " " + createUserResponse.Message);
                    break;
                default:
                    break;
            }
        }
        private void removeLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }


    }
}
