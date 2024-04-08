using Chat.Client.Entities.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chat.ChatRoom.Client.ChatRoom;
using static Chat.Lobby.Client.Lobby;

namespace Chat.Client.Services
{
    public class FactoryService
    {
        private UIService _uIService;
        private GRPCInstanceBuilderService _gRPCInstanceBuilderService;
        private GRPCLobbyService _gRPCLobbyService;
        private GRPCChatRoomService _gRPCChatRoomService;
        private List<Task> _tasks;
        private ChatInformation _chatInformation;
        private WaveInEvent _waveIn;
        private WaveOutEvent _waveOut;
        private BufferedWaveProvider _bufferedWaveProvider;

        public WaveInEvent waveIn { get => _waveIn; }
        public WaveOutEvent waveOut { get => _waveOut; }
        public BufferedWaveProvider bufferedWaveProvider { get => _bufferedWaveProvider; }
        public List<Task> tasks { get => _tasks; }
        public ChatInformation chatInformation { get => _chatInformation; }
        public GRPCInstanceBuilderService gRPCInstanceBuilderService { get => _gRPCInstanceBuilderService; }
        public UIService uIService { get => _uIService; }
        public GRPCLobbyService gRPCLobbyService { get => _gRPCLobbyService; }
        public GRPCChatRoomService gRPCChatRoomService { get => _gRPCChatRoomService; }

        public FactoryService(LobbyClient lobbyClient, ChatRoomClient chatRoomClient)
        {
            _waveIn = new WaveInEvent();
            _waveIn.WaveFormat = new WaveFormat(44100, 2);
            _waveOut = new WaveOutEvent();
            _bufferedWaveProvider = new BufferedWaveProvider(_waveIn.WaveFormat);
            _waveOut.Init(_bufferedWaveProvider);
            waveIn.DataAvailable += OnDataAvailable;

            _tasks = new List<Task>();
            _chatInformation = new ChatInformation();
            _gRPCInstanceBuilderService = new GRPCInstanceBuilderService(_chatInformation);
            _gRPCLobbyService = new GRPCLobbyService(lobbyClient);
            _gRPCChatRoomService = new GRPCChatRoomService(chatRoomClient, _tasks, _waveIn, _waveOut, _bufferedWaveProvider, _chatInformation);
            _uIService = new UIService(_gRPCLobbyService, _gRPCInstanceBuilderService, _chatInformation, _gRPCChatRoomService);
        }

        ///

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            _chatInformation.voiceBytesToSend.Enqueue(e.Buffer);
        }
    }
}
