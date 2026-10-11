# StarkSharp Setup and Usage (Pure .NET / C#)

StarkSharp is a pure C# / .NET SDK designed to interact with Starknet. It has no game-engine dependencies and targets `.NET 8` and `.NET Standard 2.0`.

## Quick Start

### 1. RPC API & Full-Node Endpoints

StarkSharp comes pre-configured with official, modern Starknet endpoints aligned with [Starknet Full-Nodes & RPC Services](https://www.starknet.io/fullnodes-rpc-services/):

#### A. Free Public RPC Endpoints (Default: Cartridge)
```csharp
using StarkSharp.Api.RPC;

// Default: Cartridge Sepolia (https://api.cartridge.gg/x/starknet/sepolia)
// Switch to Mainnet:
RpcSettingsProvider.SetNetwork(StarknetNetwork.Mainnet);

// Or use community public endpoints directly:
RpcSettingsProvider.SetApiUrl(RpcSettingsProvider.DrpcSepolia);
RpcSettingsProvider.SetApiUrl(RpcSettingsProvider.PublicNodeMainnet);
RpcSettingsProvider.SetApiUrl(RpcSettingsProvider.OnFinalityMainnet);
RpcSettingsProvider.SetApiUrl(RpcSettingsProvider.ZanMainnet);
```

#### B. Local Self-Hosted Full Nodes (Pathfinder, Juno, Madara)
Run your own full node for maximum decentralization and zero-latency reads:
```csharp
// Pathfinder (Rust node by Software Mansion - http://localhost:9545)
// Repo: https://github.com/software-mansion/pathfinder
RpcSettingsProvider.UseLocalPathfinder();

// Juno (Go node by Nethermind - http://localhost:6060)
// Repo: https://github.com/NethermindEth/juno
RpcSettingsProvider.UseLocalJuno();

// Madara (Starknet client by Madara Alliance - http://localhost:9944)
// Repo: https://github.com/madara-alliance/madara
RpcSettingsProvider.UseLocalMadara();
```

#### C. Dedicated Hosted Providers (BoltRPC, Alchemy, Infura, QuickNode)
```csharp
// BoltRPC (https://boltrpc.io/networks/starknet) - HTTPS & WebSocket
string boltHttps = RpcSettingsProvider.GetBoltRpcUrl("YOUR_API_KEY", StarknetNetwork.Mainnet);
string boltWs    = RpcSettingsProvider.GetBoltRpcWsUrl("YOUR_API_KEY", StarknetNetwork.Mainnet);
RpcSettingsProvider.SetApiUrl(boltHttps);

// Alchemy
RpcSettingsProvider.SetApiUrl(RpcSettingsProvider.GetAlchemyUrl("YOUR_API_KEY", StarknetNetwork.Sepolia));

// Infura
RpcSettingsProvider.SetApiUrl(RpcSettingsProvider.GetInfuraUrl("YOUR_API_KEY", StarknetNetwork.Mainnet));

// QuickNode
RpcSettingsProvider.SetApiUrl(RpcSettingsProvider.GetQuickNodeUrl("endpoint-name", "endpoint-token"));
```

### 2. Building and Testing

```bash
# Build the entire solution
dotnet build StarkSharp.sln

# Run all unit tests
dotnet test StarkSharp.sln
```

### 3. Dependency Injection (ASP.NET Core / Generic Host)

```csharp
using Microsoft.Extensions.DependencyInjection;
using StarkSharp.Core.DependencyInjection;
using StarkSharp.Core.Configuration;

var services = new ServiceCollection();
services.AddStarkSharp(new StarkSharpOptions
{
    RpcUrl = "https://api.cartridge.gg/x/starknet/sepolia"
});
```

### 4. Working with Tokens (ERC-20 & ERC-721 Contracts)

StarkSharp provides cohesive, strongly-typed contract clients:

```csharp
using System.Numerics;
using StarkSharp.Contracts;

// Create an ERC20 client instance
var ethToken = new ERC20Contract("0x049d36570d4e46f48e99674bd3fcc84644ddd6b96f7c741b1562b82f9e004dc7");

// 1. Read (View) Operations
BigInteger balance = await ethToken.GetBalanceAsync("0x0123456789abcdef...");
string symbol = await ethToken.GetSymbolAsync();     // "ETH"
string name = await ethToken.GetNameAsync();         // "Ether"
int decimals = await ethToken.GetDecimalsAsync();    // 18
BigInteger totalSupply = await ethToken.GetTotalSupplyAsync();

// 2. Write (Invoke) Transaction Builders
BigInteger amount = 1000000000000000000; // 1 ETH in wei
var transferTx = ethToken.CreateTransfer("0xRecipient...", amount);
var approveTx = ethToken.CreateApprove("0xSpender...", amount);

// 3. Or query in a single line:
BigInteger strkBalance = await ERC20Contract.QueryBalanceAsync("0x04718f5a0fc34cc1af16a1cdee98ffb20c31f5cd61d6ab07201858f4287c938d", "0xOwner...");
```

### 5. Starknet Data Types: `Uint256` & `Felt`

In Cairo / Starknet, 256-bit integers (`u256`) are represented as two 128-bit felts: `{ low, high }`. StarkSharp provides first-class support for `Uint256`:

```csharp
using StarkSharp.Rpc.Utils;

// Construct from BigInteger or low/high felts
Uint256 u = new Uint256(1000);
var (low, high) = u.ToFelts(); // ("0x3e8", "0x0")

// Parse directly from Starknet RPC felt array
Uint256 parsed = Uint256.FromResult(new[] { "0x3e8", "0x0" });

// Cairo short string encoding / decoding
string str = Felt.ToShortString("0x455448"); // "ETH"
```

### 6. Generic `starknet_call`

```csharp
using StarkSharp.Api.RPC;
using StarkSharp.Rpc.Utils;

var response = await JsonRpcRequestHandler.Call(
    contractAddress: "0x049d36570d4e46f48e99674bd3fcc84644ddd6b96f7c741b1562b82f9e004dc7",
    entryPointSelector: StarknetOps.CalculateFunctionSelector("balance_of"),
    "0x0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef" // calldata
);

if (!response.HasError)
{
    string[]? resultFelts = response.ResultAsArray;
}
```

### 7. Strongly-Typed Contract Function Enums

Instead of using raw magic strings like `"balance_of"` or `"transfer"`, StarkSharp provides strongly typed enums with extension methods:

- `ContractFunction`: Common standard functions (`BalanceOf`, `Transfer`, `TransferFrom`, `Approve`, `Allowance`, `Execute`, etc.)
- `ERC20Function`: Standard ERC-20 functions (`BalanceOf`, `Transfer`, `TransferFrom`, `Approve`, `Allowance`, `TotalSupply`, `Name`, `Symbol`, `Decimals`)
- `ERC721Function`: Standard ERC-721 NFT functions (`BalanceOf`, `OwnerOf`, `SafeTransferFrom`, `Approve`, `SetApprovalForAll`, etc.)
- `AccountFunction`: Standard Account contract functions (`Execute`, `Validate`, `GetNonce`, `GetPublicKey`, etc.)

```csharp
using StarkSharp.Rpc.Utils;

// 1. Get Starknet entrypoint name
string name = ERC20Function.BalanceOf.ToFunctionName(); // "balance_of"

// 2. Compute Starknet selector hash
string selector = ERC20Function.BalanceOf.ToSelector(); // "0x2e4263af13d05e91ec6ee4569fc0abbce5866654e3ca8e86528054f7000d4a1"

// 3. Or use StarknetOps overload directly:
string selector2 = StarknetOps.CalculateFunctionSelector(ContractFunction.BalanceOf);
```

### 8. Low-Level Contract Interactions Using Enums

```csharp
using StarkSharp.Components.Token.ERC20;
using StarkSharp.Components.Token.ERC721;

// ERC20 interaction generated with strongly-typed enum
var erc20Call = ERC20Standart.BalanceOf("0x049d36570d4e46f48e99674bd3fcc84644ddd6b96f7c741b1562b82f9e004dc7", "0x0123456789...");

// ERC721 interaction
var nftCall = ERC721Standart.OwnerOf("0x049d36570d4e46f48e99674bd3fcc84644ddd6b96f7c741b1562b82f9e004dc7", "1");
```
