using StarkSharp.Core.Interfaces;

namespace StarkSharp.Core.Configuration
{
    /// <summary>
    /// Configuration options for StarkSharp
    /// </summary>
    public class StarkSharpOptions : IStarkSharpConfiguration
    {
        public string RpcUrl { get; set; } = "https://alpha-mainnet.starknet.io";
        public string ChainId { get; set; } = "SN_MAIN";
        public PlatformName PlatformName { get; set; } = PlatformName.Dotnet;
        public PlatformConnectorType ConnectorType { get; set; } = PlatformConnectorType.RPC;
        public int DefaultMaxRetries { get; set; } = 500;
        public float DefaultCheckInterval { get; set; } = 2f;
    }
}

