using System;
using StarkSharp.Platforms.Godot.WebGL;
using StarkSharp.Platforms;
using StarkSharp.Fusion;
using System.Collections.Generic;
using StarkSharp.Platforms.Unity;

namespace StarkSharp.Platforms.Godot
{
	public class GodotPlatform : Platform
	{
        public static GodotPlatform New(PlatformConnectorType platformType) => platformType switch { PlatformConnectorType.WebGL => new GodotWebGLPlatform(), _ => throw new ArgumentOutOfRangeException(nameof(platformType), "Unsupported platform type.") };
    }
}