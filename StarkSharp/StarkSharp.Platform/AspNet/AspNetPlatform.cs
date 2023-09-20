namespace StarkSharp.Platforms.AspNet
{
    public class AspNetPlatform : Platform
    {
        public static AspNetPlatform New(PlatformConnectorType platformType) => platformType switch
        {
            PlatformConnectorType.RPC => new AspNetRPCController(),
            _ => new AspNetPlatform()
        };

    }
}
