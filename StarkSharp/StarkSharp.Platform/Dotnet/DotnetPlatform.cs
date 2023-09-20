using System;
using StarkSharp.Platforms.Dotnet.RPC;
using StarkSharp.Platforms.Unity.RPC;
using StarkSharp.Platforms.Unity.Sharpion;
using StarkSharp.Platforms.Unity.WebGL;
using StarkSharp.Platforms.Unity;

namespace StarkSharp.Platforms.Dotnet
{
    public class AspNetPlatform : Platform
    {

        public static AspNetPlatform New(PlatformConnectorType platformType) => platformType switch
        {
            PlatformConnectorType.RPC => new DotnetRpcPlatform(),
            _ => new AspNetPlatform()
        };

       

       
    }



}
