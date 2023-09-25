using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

namespace StarkSharp.Platforms.Godot.RPC
{
    public class GodotRPCManager : Node
    {
        public static GodotRPCManager Instance;

        public override void _Ready() => Instance = this;

        public GodotRPCRequestNode CreateNewNode()
        {
            GodotRPCRequestNode newGodotRPCRequestNode = new GodotRPCRequestNode();
            AddChild(newGodotRPCRequestNode);
            return newGodotRPCRequestNode;
        }
    }
}
