using Chat.ChatRoom.Client;
using Chat.Client.Services;
using Chat.Lobby.Client;
using Grpc.Net.Client;

var channel = GrpcChannel.ForAddress("https://localhost:7234");

var lobbyClient = new Lobby.LobbyClient(channel);
var chatRoomClient = new ChatRoom.ChatRoomClient(channel);

FactoryService factoryService = new FactoryService(lobbyClient,chatRoomClient);

factoryService.uIService.settingsHandler();

await factoryService.uIService.uIHandler();