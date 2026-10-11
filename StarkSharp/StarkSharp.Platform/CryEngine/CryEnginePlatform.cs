using StarkSharp.Core.Interfaces;
using StarkSharp.Platforms.CryEngine.RPC;

namespace StarkSharp.Platforms.Cryengine
{
    public class CryEnginePlatform : Platform
    {
        public override PlatformName PlatformName => PlatformName.CryEngine;
        public override PlatformConnectorType ConnectorType => PlatformConnectorType.RPC;

        public static CryEnginePlatform New(PlatformConnectorType platformType) => platformType switch
        {
            PlatformConnectorType.RPC => new CryEngineRpcPlatform(),
            _ => new CryEnginePlatform()
        };
    }
}
