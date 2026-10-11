namespace StarkSharp.Api.RPC
{
    /// <summary>
    /// Supported Starknet networks.
    /// </summary>
    public enum StarknetNetwork
    {
        Sepolia,
        Mainnet
    }

    /// <summary>
    /// Holds the RPC endpoint URL and network configuration based on the latest Starknet documentation.
    /// Supports Sepolia testnet and Mainnet with standard public and custom RPC providers.
    /// </summary>
    public static class RpcSettingsProvider
    {
        // --- Well-Known Modern Public & Community Endpoints ---
        public const string CartridgeSepolia = "https://api.cartridge.gg/x/starknet/sepolia";
        public const string CartridgeMainnet = "https://api.cartridge.gg/x/starknet/mainnet";

        public const string DrpcSepolia = "https://starknet-sepolia.drpc.org";
        public const string DrpcMainnet = "https://starknet.drpc.org";

        public const string PublicNodeSepolia = "https://starknet-sepolia-rpc.publicnode.com";
        public const string PublicNodeMainnet = "https://starknet-rpc.publicnode.com";

        // --- Chain IDs (felt hex representations) ---
        public const string ChainIdMainnet = "0x534e5f4d41494e"; // "SN_MAIN"
        public const string ChainIdSepolia = "0x534e5f5345504f4c4941"; // "SN_SEPOLIA"

        // Default: Official Cartridge RPC (free, fast, community-standard)
        public const string DefaultSepoliaRpc = CartridgeSepolia;
        public const string DefaultMainnetRpc = CartridgeMainnet;

        private static string _apiUrl = DefaultSepoliaRpc;
        private static StarknetNetwork _network = StarknetNetwork.Sepolia;

        /// <summary>
        /// Gets or sets the active JSON-RPC endpoint URL.
        /// </summary>
        public static string ApiUrl
        {
            get => _apiUrl;
            set
            {
                _apiUrl = string.IsNullOrWhiteSpace(value) ? DefaultSepoliaRpc : value;
                global::StarkSharp.Settings.Settings.apiurl = _apiUrl;
            }
        }

        /// <summary>
        /// Current target network.
        /// </summary>
        public static StarknetNetwork Network
        {
            get => _network;
            set
            {
                _network = value;
                ApiUrl = _network == StarknetNetwork.Mainnet ? DefaultMainnetRpc : DefaultSepoliaRpc;
            }
        }

        /// <summary>
        /// Explicitly sets the RPC endpoint URL (e.g. your Alchemy, Infura, or QuickNode custom URL).
        /// </summary>
        public static void SetApiUrl(string url) => ApiUrl = url;

        /// <summary>
        /// Switches between Sepolia and Mainnet with default public RPC endpoints.
        /// </summary>
        public static void SetNetwork(StarknetNetwork network)
        {
            Network = network;
        }

        /// <summary>
        /// Formats an Alchemy RPC URL for the specified network and API key.
        /// </summary>
        public static string GetAlchemyUrl(string apiKey, StarknetNetwork network = StarknetNetwork.Sepolia)
        {
            string subdomain = network == StarknetNetwork.Mainnet ? "starknet-mainnet" : "starknet-sepolia";
            return $"https://{subdomain}.g.alchemy.com/v2/{apiKey}";
        }

        /// <summary>
        /// Formats an Infura RPC URL for the specified network and API key.
        /// </summary>
        public static string GetInfuraUrl(string apiKey, StarknetNetwork network = StarknetNetwork.Sepolia)
        {
            string subdomain = network == StarknetNetwork.Mainnet ? "starknet-mainnet" : "starknet-sepolia";
            return $"https://{subdomain}.infura.io/v3/{apiKey}";
        }
    }
}
