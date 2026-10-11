using StarkSharp.Core.Interfaces;

namespace StarkSharp.Platforms.AspNet
{
    public class AspNetPlatform : Platform
    {
        public override PlatformName PlatformName => PlatformName.AspNet;
        public override PlatformConnectorType ConnectorType => PlatformConnectorType.RPC;

        public static AspNetPlatform New(PlatformConnectorType platformType) => platformType switch
        {
            PlatformConnectorType.RPC => new AspNetRPCController(),
            _ => new AspNetPlatform()
        };
    }
}
