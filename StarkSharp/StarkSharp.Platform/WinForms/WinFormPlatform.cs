using StarkSharp.Platforms.Winforms.RPC;

namespace StarkSharp.Platforms.Winforms
{
    public class WinFormPlatform : Platform
    {
        public static WinFormPlatform New(PlatformConnectorType platformType)
        {
            switch (platformType)
            {
                case PlatformConnectorType.RPC:
                    return new WinFormRpcPlatform();
                default:
                    return new WinFormPlatform();
            }
        }

    }
}
