using StarkSharp.Fusion.Sharpion;

namespace StarkSharp.Platforms.Unity.Sharpion
{
    public class UnitySharpionPlatform : UnityPlatform
    {
        private SharpionManager manager;

        public UnitySharpionPlatform()
        {
            manager = SharpionManager.New(PlatformName.Unity);
        }


        public override void ConnectWallet(string walletType, int id)
        {
            manager.ConnectWallet();
        }

        public override void SendTransaction(string walletType, int id, string contractAddress, string entryPoint, string callData)
        {
            manager.SendTransaction();
        }

        public void Test()
        {

        }

    }
}
