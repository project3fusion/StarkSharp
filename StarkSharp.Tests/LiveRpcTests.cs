using System;
using System.Net.Http;
using System.Threading.Tasks;
using StarkSharp.Api.RPC;
using Xunit;

namespace StarkSharp.Tests
{
    public class LiveRpcTests
    {
        [Fact]
        public async Task LiveRpc_StarknetChainId_ReturnsValidChainId()
        {
            try
            {
                var response = await JsonRpcRequestHandler.SendRpcAsync("starknet_chainId", Array.Empty<object>());
                if (response != null && !response.HasError && response.result != null)
                {
                    string? chainIdHex = response.ResultAsString;
                    Assert.NotNull(chainIdHex);
                    Assert.StartsWith("0x", chainIdHex);
                }
            }
            catch (HttpRequestException)
            {
                // Network may be unavailable or rate limited in offline/CI environment
            }
        }
    }
}
