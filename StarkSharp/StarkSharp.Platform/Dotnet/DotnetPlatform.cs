using System;
using StarkSharp.Platforms.Dotnet.RPC;
using StarkSharp.Platforms.Unity.RPC;
using StarkSharp.Platforms.Unity.Sharpion;
using StarkSharp.Platforms.Unity.WebGL;
using StarkSharp.Platforms.Unity;

namespace StarkSharp.Platforms.Dotnet
{
    public class DotnetPlatform : Platform
    {
        public static DotnetPlatform New(PlatformConnectorType platformType) => platformType switch
        {
            PlatformConnectorType.RPC => new DotnetRpcPlatform(),
            _ => new DotnetPlatform()
        };
    }
}
