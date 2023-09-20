using StarkSharp.Platforms.Unity.RPC;
using StarkSharp.Platforms.Unity.Sharpion;
using StarkSharp.Platforms.Unity.WebGL;

namespace StarkSharp.Platforms.Unity
{
    public class UnityPlatform : Platform
    {
        public static UnityPlatform New(PlatformConnectorType platformType) => platformType switch
        {
            PlatformConnectorType.WebGL => new UnityWebGLPlatform(),
            PlatformConnectorType.Sharpion => new UnitySharpionPlatform(),
            PlatformConnectorType.RPC => new UnityRpcPlatform(),
            _ => new UnityPlatform()
        };
    }
}
