using System.Numerics;
using Newtonsoft.Json;
using StarkSharp.Api.RPC;
using StarkSharp.Components.Token.ERC20;
using StarkSharp.Contracts;
using StarkSharp.Rpc;
using StarkSharp.Rpc.Utils;
using StarkSharp.StarkCurve.Signature;
using Xunit;

namespace StarkSharp.Tests
{
    public class StarknetTests
    {
        [Fact]
        public void NumericOps_StringToBigInt_ParsesDecimalAndHexCorrectly()
        {
            // Decimal
            var dec = NumericOps.StringToBigInt("123456789");
            Assert.Equal(new BigInteger(123456789), dec);

            // Hex with 0x
            var hex1 = NumericOps.StringToBigInt("0x10");
            Assert.Equal(new BigInteger(16), hex1);

            var hex2 = NumericOps.StringToBigInt("0xff");
            Assert.Equal(new BigInteger(255), hex2);

            // Large hex
            var hex3 = NumericOps.StringToBigInt("0x10000000000000000");
            Assert.Equal(BigInteger.Pow(2, 64), hex3);
        }

        [Fact]
        public void NumericOps_TransformHexToDecimalStr_ReturnsCorrectDecimal()
        {
            string decimalStr = NumericOps.TransformHexToDecimalStr("0xff");
            Assert.Equal("255", decimalStr);

            string decimalZero = NumericOps.TransformHexToDecimalStr("0x0");
            Assert.Equal("0", decimalZero);
        }

        [Fact]
        public void StarknetOps_CalculateFunctionSelector_CalculatesValidHex()
        {
            string selector = StarknetOps.CalculateFunctionSelector("balance_of");
            Assert.NotNull(selector);
            Assert.StartsWith("0x", selector);

            // Should be consistent
            string selectorAgain = StarknetOps.CalculateFunctionSelector("balance_of");
            Assert.Equal(selector, selectorAgain);
        }

        [Fact]
        public void Uint256_FromResult_ParsesLowAndHighFeltsCorrectly()
        {
            // 1 ETH = 10^18 wei = 0xDE0B6B3A7640000
            // In u256: low = 0xDE0B6B3A7640000, high = 0x0
            string[] resultFelts = new[] { "0xde0b6b3a7640000", "0x0" };
            BigInteger balance = Uint256.FromResult(resultFelts).Value;

            BigInteger expected = BigInteger.Parse("1000000000000000000");
            Assert.Equal(expected, balance);
        }

        [Fact]
        public void Uint256_FromResult_ParsesHighPartProperly()
        {
            // low = 1, high = 1 -> 1 + (1 << 128)
            string[] resultFelts = new[] { "0x1", "0x1" };
            BigInteger balance = Uint256.FromResult(resultFelts).Value;

            BigInteger expected = BigInteger.One + (BigInteger.One << 128);
            Assert.Equal(expected, balance);
        }

        [Fact]
        public void JsonRpcResponse_DeserializesStarknetCallResultArraySuccessfully()
        {
            string json = @"{
                ""jsonrpc"": ""2.0"",
                ""id"": 1,
                ""result"": [
                    ""0xde0b6b3a7640000"",
                    ""0x0""
                ]
            }";

            var response = JsonConvert.DeserializeObject<JsonRpcResponse>(json);

            Assert.NotNull(response);
            Assert.False(response.HasError);
            Assert.NotNull(response.result);

            string[]? arr = response.ResultAsArray;
            Assert.NotNull(arr);
            Assert.Equal(2, arr.Length);
            Assert.Equal("0xde0b6b3a7640000", arr[0]);

            BigInteger balance = Uint256.ParseResult(response);
            Assert.Equal(BigInteger.Parse("1000000000000000000"), balance);
        }

        [Fact]
        public void JsonRpcResponse_HandlesErrorResponseGracefully()
        {
            string errorJson = @"{
                ""jsonrpc"": ""2.0"",
                ""id"": 1,
                ""error"": {
                    ""code"": 21,
                    ""message"": ""Invalid message selector""
                }
            }";

            var response = JsonConvert.DeserializeObject<JsonRpcResponse>(errorJson);

            Assert.NotNull(response);
            Assert.True(response.HasError);
            Assert.Equal(21, response.error?.code);
            Assert.Equal("Invalid message selector", response.error?.message);

            BigInteger balance = Uint256.ParseResult(response);
            Assert.Equal(BigInteger.Zero, balance);
        }

        [Fact]
        public void StarkSigner_PedersenParams_LoadsSuccessfully()
        {
            // Verify ECDSA class initializes and loads pedersen parameters without exception
            Assert.True(ECDSA.FieldPrime > BigInteger.Zero);
            Assert.NotNull(ECDSA.ConstantPoints);
            Assert.NotEmpty(ECDSA.ConstantPoints);
        }

        [Fact]
        public void ContractFunction_Enums_ReturnAccurateFunctionNamesAndSelectors()
        {
            // Verify function names
            Assert.Equal("balance_of", ContractFunction.BalanceOf.ToFunctionName());
            Assert.Equal("transfer", ContractFunction.Transfer.ToFunctionName());
            Assert.Equal("transfer_from", ContractFunction.TransferFrom.ToFunctionName());
            Assert.Equal("approve", ContractFunction.Approve.ToFunctionName());
            Assert.Equal("allowance", ContractFunction.Allowance.ToFunctionName());
            Assert.Equal("__execute__", ContractFunction.Execute.ToFunctionName());

            Assert.Equal("balance_of", ERC20Function.BalanceOf.ToFunctionName());
            Assert.Equal("transfer", ERC20Function.Transfer.ToFunctionName());
            Assert.Equal("transfer_from", ERC20Function.TransferFrom.ToFunctionName());
            Assert.Equal("approve", ERC20Function.Approve.ToFunctionName());
            Assert.Equal("allowance", ERC20Function.Allowance.ToFunctionName());
            Assert.Equal("total_supply", ERC20Function.TotalSupply.ToFunctionName());

            Assert.Equal("balance_of", ERC721Function.BalanceOf.ToFunctionName());
            Assert.Equal("owner_of", ERC721Function.OwnerOf.ToFunctionName());
            Assert.Equal("safe_transfer_from", ERC721Function.SafeTransferFrom.ToFunctionName());

            Assert.Equal("__execute__", AccountFunction.Execute.ToFunctionName());
            Assert.Equal("__validate__", AccountFunction.Validate.ToFunctionName());
            Assert.Equal("get_nonce", AccountFunction.GetNonce.ToFunctionName());

            // Verify selectors match StarknetOps.CalculateFunctionSelector
            string rawBalanceOfSelector = StarknetOps.CalculateFunctionSelector("balance_of");
            Assert.Equal(rawBalanceOfSelector, ContractFunction.BalanceOf.ToSelector());
            Assert.Equal(rawBalanceOfSelector, ERC20Function.BalanceOf.ToSelector());
            Assert.Equal(rawBalanceOfSelector, ERC721Function.BalanceOf.ToSelector());
            Assert.Equal(rawBalanceOfSelector, StarknetOps.CalculateFunctionSelector(ERC20Function.BalanceOf));

            string rawTransferSelector = StarknetOps.CalculateFunctionSelector("transfer");
            Assert.Equal(rawTransferSelector, ContractFunction.Transfer.ToSelector());
            Assert.Equal(rawTransferSelector, ERC20Function.Transfer.ToSelector());
            Assert.Equal(rawTransferSelector, StarknetOps.CalculateFunctionSelector(ERC20Function.Transfer));

            string rawExecuteSelector = StarknetOps.CalculateFunctionSelector("__execute__");
            Assert.Equal(rawExecuteSelector, ContractFunction.Execute.ToSelector());
            Assert.Equal(rawExecuteSelector, AccountFunction.Execute.ToSelector());
            Assert.Equal(rawExecuteSelector, StarknetOps.CalculateFunctionSelector(AccountFunction.Execute));
        }

        [Fact]
        public void ERC20Standart_BalanceOf_ProducesCorrectSelector()
        {
            var interaction = ERC20Standart.BalanceOf("0x123", "0xabc");
            Assert.Equal("0x123", interaction.ContractAdress);
            Assert.Equal(ERC20Function.BalanceOf.ToSelector(), interaction.EntryPoint);
        }

        [Fact]
        public void Uint256_ConvertsAndOperatesCorrectly()
        {
            // Low = 100, High = 2 -> 100 + 2 * 2^128
            BigInteger twoPow128 = BigInteger.One << 128;
            BigInteger expected = 100 + (2 * twoPow128);

            var u = new Uint256(100, 2);
            Assert.Equal(expected, u.Value);
            Assert.Equal(new BigInteger(100), u.Low);
            Assert.Equal(new BigInteger(2), u.High);

            // FromFelts
            var parsed = Uint256.FromFelts("0x64", "0x2");
            Assert.Equal(u, parsed);

            // ToFelts roundtrip
            var (lowHex, highHex) = u.ToFelts();
            Assert.Equal("0x64", lowHex);
            Assert.Equal("0x2", highHex);

            // Arithmetic
            Uint256 a = 50;
            Uint256 b = 25;
            Assert.Equal(new Uint256(75), a + b);
            Assert.Equal(new Uint256(25), a - b);
            Assert.Equal(new Uint256(1250), a * b);
            Assert.Equal(new Uint256(2), a / b);

            // Comparison
            Assert.True(a > b);
            Assert.True(b < a);
            Assert.True(a == 50);
        }

        [Fact]
        public void Felt_ShortString_EncodesAndDecodesProperly()
        {
            // "STRK"
            string original = "STRK";
            BigInteger encoded = Felt.FromShortString(original);
            string decoded = Felt.ToShortString(encoded);
            Assert.Equal(original, decoded);

            // "ETH" (0x455448)
            Assert.Equal("ETH", Felt.ToShortString("0x455448"));

            // Cairo 1 ByteArray decoding test: [num_full_words, w0, ..., pending_word, pending_len]
            // "Hello World" -> 0 full words, pending_word = "Hello World", pending_len = 11
            BigInteger helloWorldFelt = Felt.FromShortString("Hello World");
            string[] cairo1ByteArray = new string[]
            {
                "0x0", // 0 full words
                Felt.ToHex(helloWorldFelt), // pending word
                "0xb"  // pending len = 11
            };
            string decodedCairo1 = Felt.DecodeCairoString(cairo1ByteArray);
            Assert.Equal("Hello World", decodedCairo1);
        }

        [Fact]
        public void ERC20Contract_GeneratesInteractionsWithProperUint256Calldata()
        {
            var contract = new ERC20Contract("0x049d36570d4e46f48e99674bd3fcc84644ddd6b96f7c741b1562b82f9e004dc7");
            BigInteger amount = 1000000000000000000; // 1 ETH (0xDE0B6B3A7640000)

            var interaction = contract.CreateTransfer("0xRecipient123", amount);
            Assert.Equal("0x049d36570d4e46f48e99674bd3fcc84644ddd6b96f7c741b1562b82f9e004dc7", interaction.ContractAdress);
            Assert.Equal(ERC20Function.Transfer.ToSelector(), interaction.EntryPoint);

            // Calldata should contain recipient, low, and high
            Assert.Contains("0xRecipient123", interaction.CallData);
            Assert.Contains("0xde0b6b3a7640000", interaction.CallData);
            Assert.Contains("0x0", interaction.CallData);
        }

        [Fact]
        public void ERC721Contract_GeneratesInteractionsCorrectly()
        {
            var nft = new ERC721Contract("0xNFTContractAddress");
            BigInteger tokenId = 42;

            var interaction = nft.CreateTransferFrom("0xOwner", "0xBuyer", tokenId);
            Assert.Equal("0xNFTContractAddress", interaction.ContractAdress);
            Assert.Equal(ERC721Function.TransferFrom.ToSelector(), interaction.EntryPoint);
            Assert.Contains("0xOwner", interaction.CallData);
            Assert.Contains("0xBuyer", interaction.CallData);
            Assert.Contains("0x2a", interaction.CallData); // 42 in hex
        }

        [Fact]
        public void RpcSettingsProvider_LocalFullNodesAndHostedUrls_AreFormattedCorrectly()
        {
            // Local full nodes
            RpcSettingsProvider.UseLocalPathfinder();
            Assert.Equal(RpcSettingsProvider.PathfinderLocal, RpcSettingsProvider.ApiUrl);

            RpcSettingsProvider.UseLocalJuno();
            Assert.Equal(RpcSettingsProvider.JunoLocal, RpcSettingsProvider.ApiUrl);

            RpcSettingsProvider.UseLocalMadara();
            Assert.Equal(RpcSettingsProvider.MadaraLocal, RpcSettingsProvider.ApiUrl);

            // BoltRPC endpoints
            string boltMainnet = RpcSettingsProvider.GetBoltRpcUrl("test_key", StarknetNetwork.Mainnet);
            Assert.Equal("https://endpoints.boltrpc.io/rpc/test_key/starknet", boltMainnet);

            string boltSepolia = RpcSettingsProvider.GetBoltRpcUrl("test_key", StarknetNetwork.Sepolia);
            Assert.Equal("https://endpoints.boltrpc.io/rpc/test_key/starknet-sepolia", boltSepolia);

            string boltWs = RpcSettingsProvider.GetBoltRpcWsUrl("test_key", StarknetNetwork.Mainnet);
            Assert.Equal("wss://endpoints.boltrpc.io/ws/test_key/starknet", boltWs);

            // Alchemy and Infura
            string alchemy = RpcSettingsProvider.GetAlchemyUrl("key123", StarknetNetwork.Sepolia);
            Assert.Equal("https://starknet-sepolia.g.alchemy.com/v2/key123", alchemy);

            string infura = RpcSettingsProvider.GetInfuraUrl("key123", StarknetNetwork.Mainnet);
            Assert.Equal("https://starknet-mainnet.infura.io/v3/key123", infura);

            // QuickNode
            string qn = RpcSettingsProvider.GetQuickNodeUrl("my-node", "my-token");
            Assert.Equal("https://my-node.quiknode.pro/my-token/", qn);

            // Reset back to Sepolia
            RpcSettingsProvider.SetNetwork(StarknetNetwork.Sepolia);
            Assert.Equal(RpcSettingsProvider.DefaultSepoliaRpc, RpcSettingsProvider.ApiUrl);
        }
    }
}
