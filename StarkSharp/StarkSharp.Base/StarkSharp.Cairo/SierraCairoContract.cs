using System;
using System.Collections.Generic;

namespace StarkSharp.Base.Cairo
{
    /// <summary>
    /// Represents a Sierra (Cairo 1+) contract
    /// </summary>
    public class SierraCairoContract
    {
        public string ContractClassVersion { get; set; }
        public List<string> SierraProgram { get; set; }
        public EntryPoints EntryPoints { get; set; }
        public List<string> Abi { get; set; }

        public class EntryPoints
        {
            public List<EntryPoint> Constructor { get; set; }
            public List<EntryPoint> External { get; set; }
            public List<EntryPoint> L1Handler { get; set; }
        }

        public class EntryPoint
        {
            public string Selector { get; set; }
            public int FunctionIdx { get; set; }
        }
    }
}
