<h1>Starksharp Godot Setup</h1>

<h3>Required platforms</h3>

<img src="https://img.shields.io/badge/Godot-3.5 LTS+-green">

**Important:** Currently Godot 4 doesn't support C# applications to be exported to web. That's why, StarkSharp is not compatible with Godot 4.

<h3>Setup</h3>

1. Download [StarkSharp](../StarkSharp/) repository from GitHub.

2. Place StarkSharp folder inside your Godot project.

3. Go to [StarkSharp.Platforms](../StarkSharp/StarkSharp.Platforms/) and remove other platform folders other than Godot.

4. Open your Godot project folder. Add Newtonsoft.Json Library using Nuget. (You can use Visual Studio Code or Visual Studio to add Newtonsoft.Json to your library.)

5. If you are using webgl, you need GodotBrowserManager.cs attached to a node, higher in hierarchy than others. (This will be automatic with plugin ui but still being implemented) and if you are using RPC you need to add GodotRPCManager.cs attached to a node. These are required because of godot's structure.

6. You can set your specifications in here.

7. Create a new script and write [example](../StarkSharp/StarkSharp.Examples/) codes.

8. You are now ready to go!
