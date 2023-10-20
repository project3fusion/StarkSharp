using StarkSharp.Platforms.Dotnet.RPC;

namespace StarkSharp.Platforms.Dotnet
{
    public class DotnetPlatform : Platform
    {
        public static DotnetPlatform New(PlatformConnectorType platformType)
        {
            DotnetPlatform platform;

            switch (platformType)
            {
                case PlatformConnectorType.RPC:
                    platform = new DotnetRpcPlatform();
                    break;
                default:
                    platform = new DotnetPlatform();
                    break;
            }

            return platform;
        }

    }
}
