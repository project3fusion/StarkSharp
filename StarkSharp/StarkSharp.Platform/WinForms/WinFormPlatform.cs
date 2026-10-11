using StarkSharp.Core.Interfaces;
using StarkSharp.Platforms.Winforms.RPC;

namespace StarkSharp.Platforms.Winforms
{
    public class WinFormPlatform : Platform
    {
        public override PlatformName PlatformName => PlatformName.WinForms;
        public override PlatformConnectorType ConnectorType => PlatformConnectorType.RPC;

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
