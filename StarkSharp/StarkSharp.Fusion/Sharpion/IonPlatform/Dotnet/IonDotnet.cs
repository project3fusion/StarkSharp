
using StarkSharp.Connectors.Components;
using StarkSharp.Fusion.Sharpion.Dotnet;
using System.Numerics;

namespace StarkSharp.Fusion.Sharpion.Manager.IonPlatforms.Dotnet
{
    public class IonDotnet : IonPlatform
    {
        public static Client socket;
        public override void ConnectToServer() { Client.instance.ConnectToServer(); socket = Client.instance; }
        public override void DisconnectToServer() =>  socket.DisconnectFromServer();
        public override void ConnectWallet() => socket.ConnectWallet();
        public override void DisconnectWallet() => socket.DisconnectWallet(); 
        public override bool ConnectionStatus() =>  socket.IsSocketAlive();
        public override void BalanceOf(string walletadress) => socket.BalanceOfWallet(walletadress);
        public override void SendTransaction(TransactionInteraction transactionInteraction) => socket.SendTransaction(transactionInteraction);
    }
}
