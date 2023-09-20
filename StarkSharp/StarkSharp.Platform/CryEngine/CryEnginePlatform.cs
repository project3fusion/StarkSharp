
using StarkSharp.Platforms.CryEngine.RPC;

namespace StarkSharp.Platforms.Cryengine
{
    public class CryEnginePlatform : Platform
    {
        public static CryEnginePlatform New(PlatformConnectorType platformType) => platformType switch
        {
            //PlatformConnectorType.Sharpion => new UnitySharpionPlatform(),
            PlatformConnectorType.RPC => new CryEngineRpcPlatform(),
            _ => new CryEnginePlatform()
        };
    }
}
