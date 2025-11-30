using System;
using System.Collections.Generic;

namespace StarkSharp.Base.Cairo
{
    /// <summary>
    /// Represents a Cairo 0 contract
    /// </summary>
    public class CairoContract
    {
        public string Program { get; set; }
        public List<EntryPoint> EntryPointsByType { get; set; }
        public List<string> Abi { get; set; }

        public class EntryPoint
        {
            public string Selector { get; set; }
            public int Offset { get; set; }
            public List<string> Builtins { get; set; }
        }
    }
}
