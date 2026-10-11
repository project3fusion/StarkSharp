using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StarkSharp.Connectors.Components;
using StarkSharp.Fusion.Sharpion.Dotnet.Handlers;

namespace StarkSharp.Fusion.Sharpion.Dotnet
{
    public class Client
    {
        public static Client instance = new Client();
        private ClientWebSocket _ws;
        private CancellationTokenSource _cts;

        public static int SocketClientID;
        public string UserWalletAddress;
        public string UserBalanceOfEth;

        public async void ConnectToServer()
        {
            try
            {
                _cts = new CancellationTokenSource();
                _ws = new ClientWebSocket();
                Uri serverUri = new Uri($"ws://{Settings.Settings.webSocketipandport}");
                await _ws.ConnectAsync(serverUri, _cts.Token);
                Console.WriteLine("WebSocket Connection Open.");
                _ = Task.Run(() => ReceiveLoopAsync(_cts.Token));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket Connection Error: {ex.Message}");
            }
        }

        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[8192];
            try
            {
                while (_ws != null && _ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    using (var ms = new MemoryStream())
                    {
                        WebSocketReceiveResult result;
                        do
                        {
                            result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", cancellationToken);
                                Console.WriteLine("WebSocket Connection Close.");
                                return;
                            }
                            ms.Write(buffer, 0, result.Count);
                        } while (!result.EndOfMessage);

                        string message = Encoding.UTF8.GetString(ms.ToArray());
                        await Handler.HandShake(message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected on disconnect
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket Receive Error: {ex.Message}");
            }
        }

        public async void DisconnectFromServer()
        {
            try
            {
                _cts?.Cancel();
                if (_ws != null && _ws.State == WebSocketState.Open)
                {
                    await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed manually", CancellationToken.None);
                    Console.WriteLine("The WebSocket connection was closed manually.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing WebSocket: {ex.Message}");
            }
            finally
            {
                _ws?.Dispose();
                _ws = null;
            }
        }

        public bool IsSocketAlive() => _ws != null && _ws.State == WebSocketState.Open;

        public void ConnectWallet() => instance.SendDataFromJson(JsonConvert.SerializeObject(Packs.CreateLoginPack(false, false)));
        public void DisconnectWallet() => instance.SendDataFromJson(JsonConvert.SerializeObject(Packs.CreateDisconnectPack(SocketClientID, false, false)));
        public void BalanceOfWallet(string WalletAdress) => instance.SendDataFromJson(JsonConvert.SerializeObject(Packs.CreateBalanceOfPack(SocketClientID, WalletAdress)));
        public void SendTransaction(TransactionInteraction transactionInteraction) => instance.SendDataFromJson(JsonConvert.SerializeObject(Packs.CreateTransactionPack(SocketClientID, transactionInteraction)));

        public async void SendDataFromJson(string sendjson)
        {
            try
            {
                // This line checks if the given string is in valid JSON format.
                JsonConvert.DeserializeObject(sendjson);
            }
            catch (JsonReaderException jex)
            {
                // If an error occurs, it means the string is not in JSON format.
                Console.WriteLine("The provided string is not in a valid JSON format.");
                Console.WriteLine(jex.Message); // Prints the specific error message.
                return; // Exits the method.
            }

            try
            {
                if (_ws != null && _ws.State == WebSocketState.Open)
                {
                    byte[] dataToSend = Encoding.UTF8.GetBytes(sendjson);
                    await _ws.SendAsync(new ArraySegment<byte>(dataToSend), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception ex) // General exception for any WebSocket related error.
            {
                Console.WriteLine("Error occurred while sending data.");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
