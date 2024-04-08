using Chat.ChatRoom.Client;
using Chat.Client.Entities.Models;
using Google.Protobuf;
using Grpc.Core;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chat.ChatRoom.Client.ChatRoom;

namespace Chat.Client.Services
{
    public class GRPCChatRoomService
        (
        ChatRoomClient chatRoomClient,
        List<Task> tasks,
        WaveInEvent waveIn,
        WaveOutEvent waveOut,
        BufferedWaveProvider bufferedWaveProvider,
        ChatInformation chatInformation
        )
    {
        public void joinTextChatStream(Metadata headers)
        {
            var textChatStreamResponse = chatRoomClient.textChatStream(new(), headers: headers);

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            var textStreamTask = Task.Run(async () =>
            {
                var current = new TextChatStreamResponse();
                while (await textChatStreamResponse.ResponseStream.MoveNext(tokenSource.Token))
                {
                    current = textChatStreamResponse.ResponseStream.Current;
                    await Console.Out.WriteLineAsync(current.Date + " " + current.Username + " : " + current.Message);
                }
            });
            tasks.Add(textStreamTask);
        }
        public void joinVoiceChatStream(Metadata headers)
        {
            var voiceChatStreamResponse = chatRoomClient.voiceChatStream(new(), headers: headers);
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            byte[] bytesVoices = new byte[17640];
            var voiceChatStreamTask = Task.Run(async () =>
            {
                waveOut.Play();
                var current = new VoiceChatStreamResponse();
                while (await voiceChatStreamResponse.ResponseStream.MoveNext(tokenSource.Token))
                {
                    current = voiceChatStreamResponse.ResponseStream.Current;
                    bytesVoices = current.VoiceBytes.ToByteArray();
                    bufferedWaveProvider.AddSamples(bytesVoices, 0, bytesVoices.Length);
                }
                waveOut.Stop();
            });
            tasks.Add(voiceChatStreamTask);
        }

        public async Task sendTextAsync(SendTextRequest sendTextRequest, Metadata headers)
        {
            if (sendTextRequest.Message != null)
                await chatRoomClient.sendTextAsync(sendTextRequest, headers: headers);
        }

        public void sendVoiceStream(Metadata headers)
        {
            var sendVoiceStream = chatRoomClient.sendVoiceStream(headers: headers);
            var cancellationTokenSource = new CancellationTokenSource();
            var sendVoiceTask = Task.Run(async () =>
            {
                waveIn.StartRecording();
                while (true)
                {
                    if (chatInformation.voiceBytesToSend.Any())
                    {
                        await sendVoiceStream.RequestStream.WriteAsync(new SendVoiceStreamRequest()
                        {
                            ChatId = chatInformation.chatId,
                            VoiceBytes = ByteString.CopyFrom(chatInformation.voiceBytesToSend.Dequeue())
                        });
                    }

                    if (cancellationTokenSource.Token.IsCancellationRequested)
                        break;
                }
                waveIn.StopRecording();
                await sendVoiceStream.ResponseAsync;
            });
            tasks.Add(sendVoiceTask);
        }
    }
}
