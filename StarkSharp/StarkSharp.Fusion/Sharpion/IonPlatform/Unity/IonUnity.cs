using StarkSharp.Connectors.Components;
using StarkSharp.Fusion.Sharpion.Unity;
using StarkSharp.Platforms;
using System;
using System.Numerics;

namespace StarkSharp.Fusion.Sharpion.Manager.IonPlatforms.Unity
{
    public class IonUnity : IonPlatform
    {
        public static Socket socket;
        public override void ConnectToServer() { Socket.instance.ConnectToServer(); socket = Socket.instance; }
        public override void DisconnectToServer() =>  socket.DisconnectFromServer();
        public override void ConnectWallet() => socket.ConnectWallet();
        public override void DisconnectWallet() => socket.DisconnectWallet(); 
        public override bool ConnectionStatus() =>  socket.IsSocketAlive();
        public override void BalanceOf(string walletadress) => socket.BalanceOfWallet(walletadress);
        public override void SendTransaction(TransactionInteraction transactionInteraction) => socket.SendTransaction(transactionInteraction);
    }
}
