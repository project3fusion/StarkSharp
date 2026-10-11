using StarkSharp.Connectors.Components;
using StarkSharp.Core.Interfaces;
using StarkSharp.Fusion.Sharpion.Manager.IonPlatforms;
using StarkSharp.Fusion.Sharpion.Manager.IonPlatforms.Dotnet;
using System;
using System.Numerics;

namespace StarkSharp.Fusion.Sharpion.Manager
{
    public class SharpionManager
    {
        IonPlatform IonPlatform;
        public SharpionManager(IonPlatform platform) { this.IonPlatform = platform; }

        public static SharpionManager New(PlatformName name)
        {
            IonPlatform platform;

            switch (name)
            {
                case PlatformName.Dotnet:
                    platform = new IonDotnet();
                    break;
                default:
                    throw new NotSupportedException($"Platform '{name}' is not supported in .NET SDK.");
            }

            return new SharpionManager(platform);
        }


        public virtual void ConnectToServer() { IonPlatform.ConnectToServer(); }
        public virtual void DisconnectToServer() { IonPlatform.DisconnectToServer(); }
        public virtual bool ConnectionStatus() { return IonPlatform.ConnectionStatus(); }
        public virtual void ConnectWallet() { IonPlatform.ConnectWallet(); }
        public virtual void DisconnectWallet() { IonPlatform.DisconnectWallet(); }
        public virtual void BalanceOf(string walletadress) { IonPlatform.BalanceOf(walletadress); }
        public virtual void SendTransaction(TransactionInteraction transactionInteraction) { IonPlatform.SendTransaction(transactionInteraction); }

    }
}
