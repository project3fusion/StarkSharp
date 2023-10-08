using System;
using System.Collections.Generic;

using StarkSharp.Platforms;
using StarkSharp.Platforms.Godot.WebGL;
using StarkSharp.Platforms.Godot.RPC;

namespace StarkSharp.Platforms.Godot
{
	public class GodotPlatform : Platform
	{
        public static GodotPlatform New(PlatformConnectorType platformType)
        {
            switch (platformType)
            {
                case PlatformConnectorType.WebGL: return new GodotWebGLPlatform();
                case PlatformConnectorType.RPC: return new GodotRPCPlatform();
                default: throw new ArgumentOutOfRangeException(nameof(platformType), "Unsupported platform type.");
            }
        }
    }
}