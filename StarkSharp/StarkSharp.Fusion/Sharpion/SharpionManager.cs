using StarkSharp.Fusion.Sharpion.Manager.IonPlatforms;
using StarkSharp.Fusion.Sharpion.Manager.IonPlatforms.Dotnet;
using StarkSharp.Fusion.Sharpion.Manager.IonPlatforms.Unity;
using System;
using System.Numerics;
using static StarkSharp.Platforms.Platform;

namespace StarkSharp.Fusion.Sharpion.Manager
{
    public class SharpionManager
    {

        IonPlatform IonPlatform;
        public SharpionManager(IonPlatform platform) {this.IonPlatform = platform;}

        public static SharpionManager New(PlatformName name)
        {
            IonPlatform platform = name switch
            {
                PlatformName.Unity => new IonUnity(),
                PlatformName.Dotnet => new IonDotnet(),
                _ => throw new NotSupportedException($"Platform '{name}' is not supported.")
            };

            return new SharpionManager(platform);
        }

        public virtual void ConnectToServer() { IonPlatform.ConnectToServer(); }
        public virtual void DisconnectToServer() { IonPlatform.DisconnectToServer(); }
        public virtual bool ConnectionStatus() { return IonPlatform.ConnectionStatus(); }
        public virtual void ConnectWallet() { IonPlatform.ConnectWallet(); }
        public virtual void DisconnectWallet() { IonPlatform.DisconnectWallet(); }
        public virtual void BalanceOf(string walletadress) { IonPlatform.BalanceOf(walletadress); }
        public virtual void SendTransaction(string Receivingaddress, BigInteger amount) { IonPlatform.SendTransaction(Receivingaddress,amount); }

    }
}
