


using StarkSharp.Platforms.Winforms.RPC;

namespace StarkSharp.Platforms.Winforms
{
    public class WinFormPlatform : Platform
    {
        public static WinFormPlatform New(PlatformConnectorType platformType) => platformType switch
        {
            PlatformConnectorType.RPC => new WinFormRpcPlatform(),
            _ => new WinFormPlatform()
        };
    }
}
