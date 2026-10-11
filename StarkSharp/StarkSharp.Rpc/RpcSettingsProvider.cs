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
        // --- Local Self-Hosted Full Nodes ---
        /// <summary>Default local RPC endpoint for Pathfinder (Rust client by Equilibrium / Software Mansion).</summary>
        public const string PathfinderLocal = "http://localhost:9545";

        /// <summary>Default local RPC endpoint for Juno (Go client by Nethermind).</summary>
        public const string JunoLocal = "http://localhost:6060";

        /// <summary>Default local RPC endpoint for Madara (Starknet sequencer & client by Madara Alliance).</summary>
        public const string MadaraLocal = "http://localhost:9944";

        // --- Official & Community Public RPC Endpoints (Free, No API Key Required) ---
        /// <summary>Official Cartridge public RPC endpoint for Sepolia testnet.</summary>
        public const string CartridgeSepolia = "https://api.cartridge.gg/x/starknet/sepolia";
        /// <summary>Official Cartridge public RPC endpoint for Mainnet.</summary>
        public const string CartridgeMainnet = "https://api.cartridge.gg/x/starknet/mainnet";

        /// <summary>dRPC public endpoint for Sepolia testnet.</summary>
        public const string DrpcSepolia = "https://starknet-sepolia.drpc.org";
        /// <summary>dRPC public endpoint for Mainnet.</summary>
        public const string DrpcMainnet = "https://starknet.drpc.org";

        /// <summary>PublicNode community endpoint for Sepolia testnet.</summary>
        public const string PublicNodeSepolia = "https://starknet-sepolia-rpc.publicnode.com";
        /// <summary>PublicNode community endpoint for Mainnet.</summary>
        public const string PublicNodeMainnet = "https://starknet-rpc.publicnode.com";

        /// <summary>OnFinality public endpoint for Starknet Mainnet.</summary>
        public const string OnFinalityMainnet = "https://starknet.api.onfinality.io/public";

        /// <summary>ZAN public endpoint for Starknet Mainnet.</summary>
        public const string ZanMainnet = "https://api.zan.top/public/starknet-mainnet";

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

        /// <summary>Sets active RPC endpoint to a local Pathfinder node (http://localhost:9545).</summary>
        public static void UseLocalPathfinder(int port = 9545) => ApiUrl = $"http://localhost:{port}";

        /// <summary>Sets active RPC endpoint to a local Juno node (http://localhost:6060).</summary>
        public static void UseLocalJuno(int port = 6060) => ApiUrl = $"http://localhost:{port}";

        /// <summary>Sets active RPC endpoint to a local Madara node (http://localhost:9944).</summary>
        public static void UseLocalMadara(int port = 9944) => ApiUrl = $"http://localhost:{port}";

        // --- Hosted Provider Helpers ---

        /// <summary>Formats a BoltRPC HTTPS endpoint URL for the specified network and API key (https://boltrpc.io/networks/starknet).</summary>
        public static string GetBoltRpcUrl(string apiKey, StarknetNetwork network = StarknetNetwork.Mainnet)
        {
            string networkSlug = network == StarknetNetwork.Mainnet ? "starknet" : "starknet-sepolia";
            return $"https://endpoints.boltrpc.io/rpc/{apiKey}/{networkSlug}";
        }

        /// <summary>Formats a BoltRPC WebSocket endpoint URL for the specified network and API key.</summary>
        public static string GetBoltRpcWsUrl(string apiKey, StarknetNetwork network = StarknetNetwork.Mainnet)
        {
            string networkSlug = network == StarknetNetwork.Mainnet ? "starknet" : "starknet-sepolia";
            return $"wss://endpoints.boltrpc.io/ws/{apiKey}/{networkSlug}";
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

        /// <summary>Formats a QuickNode RPC URL.</summary>
        public static string GetQuickNodeUrl(string endpointName, string endpointToken) =>
            $"https://{endpointName}.quiknode.pro/{endpointToken}/";

        /// <summary>Formats a Chainstack RPC URL.</summary>
        public static string GetChainstackUrl(string endpointUrl) => endpointUrl;

        /// <summary>Formats a Lava Protocol gateway URL.</summary>
        public static string GetLavaUrl(string apiKey) =>
            $"https://g.w.lavanet.xyz:443/gateway/strk/rpc-http/{apiKey}";

        /// <summary>Formats a Reddio Starknet RPC URL.</summary>
        public static string GetReddioUrl(string apiKey, StarknetNetwork network = StarknetNetwork.Mainnet)
        {
            string slug = network == StarknetNetwork.Mainnet ? "starknet-mainnet" : "starknet-sepolia";
            return $"https://{slug}.reddio.com/v1/{apiKey}";
        }

        /// <summary>Formats a NOWNodes Starknet RPC URL.</summary>
        public static string GetNowNodesUrl(string apiKey) =>
            $"https://strk.nownodes.io/{apiKey}";

        /// <summary>Formats a GetBlock Starknet RPC URL.</summary>
        public static string GetGetBlockUrl(string apiKey) =>
            $"https://go.getblock.io/{apiKey}";
    }
}
