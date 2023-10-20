
using StarkSharp.Platforms.Winforms.RPC;

namespace StarkSharp.Platforms.Winforms
{
    public class WinFormsPlatform : Platform
    {
        public static WinFormsPlatform New(PlatformConnectorType platformType)
        {
            WinFormsPlatform platform;

            switch (platformType)
            {
                case PlatformConnectorType.RPC:
                    platform = new WinFormRpcPlatform();
                    break;
                default:
                    platform = new WinFormsPlatform();
                    break;
            }

            return platform;
        }
    }
}
