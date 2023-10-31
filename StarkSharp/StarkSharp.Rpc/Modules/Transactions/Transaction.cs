using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StarkSharp.Connectors.Components;
using StarkSharp.Platforms;
using StarkSharp.Rpc.Modules.Transactions.Hash;
using StarkSharp.StarkCurve.Signature;
using StarkSharp.Tools.Notification;
using System;
using System.Numerics;


namespace StarkSharp.Rpc.Modules.Transactions
{
    public class Transaction : Modul
    {
        private string _maxFee;
        private string _chainId;
        private string _privateKey;
        private string _senderAddress;
        private string _contractAddress;
        private string _calldataHash;
        private string[] _calldata;
        private string _nonce;
        private readonly Platform _platform;

        public Transaction(Platform platform)
        {
            _platform = platform;
        }

        public JsonRpc CreateTransaction(TransactionInteraction transaction)
        {
            _maxFee = transaction.MaxFee.ToString();
            _chainId = transaction.ChainId.ToString();
            _privateKey = transaction.PrivateKey;
            _senderAddress = transaction.SenderAddress;
            _contractAddress = transaction.ContractAddress;

            TransactionHash.Call[] callArray = new TransactionHash.Call[]
            {
                new TransactionHash.Call
                {
                    To = _contractAddress,
                    Selector = transaction.FunctionName,
                    Data = transaction.FunctionArgs
                }
            };
            _calldataHash = "0x" + TransactionHash.Hash.ComputeCalldataHash(callArray, 1);
            _calldata = TransactionHash.Hash.FormatCalldata(callArray, (int)transaction.CairoVersion);

            string[] request = { "latest", _senderAddress };
            return JsonRpcHandler.GenerateRequestData("starknet_getNonce", request);
        }

        public void OnNonceComplete(Platform platform, TransactionInteraction transactionInteraction, object response)
        {
            try
            {
                if (response is JsonRpcResponse jsonResponse && jsonResponse.result != null)
                {
                    object nonce = ((JsonRpcResponse)response).result;
                    _nonce = nonce.ToString();
                    ECDSA.ECSignature signature = TransactionHash.Hash.SignInvokeTransaction(
                        "0x1",
                        _senderAddress,
                        _calldataHash,
                        _maxFee,
                        _chainId,
                        nonce.ToString(),
                        TransactionHash.Hash.HexToBigInteger(_privateKey)
                    );
                    string r = TransactionHash.Hash.BigIntegerToHex(signature.R);
                    string s = TransactionHash.Hash.BigIntegerToHex(signature.S);
                    //platform.PlatformLog("Transaction signed. R: " + signature.R + ", S: " + signature.S, NotificationType.Info);
                    var transactionRequest = new object[]
                    {
                        new
                        {
                            type = "INVOKE",
                            sender_address = _senderAddress,
                            calldata = _calldata,
                            max_fee = _maxFee,
                            version = "0x1",
                            signature = new string[] { r, s },
                            nonce = nonce.ToString()
                        }
                    };

                    var requestData = new JsonRpc
                    {
                        id = 1,
                        method = "starknet_estimateFee",
                        @params = new object[] { transactionRequest, "latest" }
                    };


                    _platform.PlatformRequest(requestData, response =>
                        OnEstimateFeeComplete(platform, response)
                    );
                }
                else
                {
                    platform.PlatformLog("Invalid response or result is null.", NotificationType.Error);
                    return;
                }

            }
            catch (Exception ex)
            {
                platform.PlatformLog("Something went wrong: " + ex.Message, NotificationType.Warning);
            }
        }

        private void OnEstimateFeeComplete(Platform platform, object response)
        {
            try
            {
                var result = ((JsonRpcResponse)response).result;
                var data = ((JArray)result).ToObject<object[]>();
                if (data == null)
                {
                    platform.PlatformLog("Invalid response or result is null.", NotificationType.Error);
                    return;
                }
                var overalFee = ((JObject)data[0])["overall_fee"];
                if (overalFee == null)
                {
                    platform.PlatformLog("Invalid response or result is null.", NotificationType.Error);
                    return;
                }

                BigInteger bigIntValue = BigInteger.Parse(((string)overalFee).Substring(2), System.Globalization.NumberStyles.HexNumber);
                bigIntValue *= 10;
                string maxFee = "0x" + bigIntValue.ToString("x");

                ECDSA.ECSignature signature = TransactionHash.Hash.SignInvokeTransaction(
                            "0x1",
                            _senderAddress,
                            _calldataHash,
                            maxFee,
                            _chainId,
                            _nonce,
                        TransactionHash.Hash.HexToBigInteger(_privateKey)
                        );

                string r = TransactionHash.Hash.BigIntegerToHex(signature.R);
                string s = TransactionHash.Hash.BigIntegerToHex(signature.S);

                var _signature = new string[] { r, s };

                var requestData = new JsonRpc
                {
                    id = 1,
                    method = "starknet_addInvokeTransaction",
                    @params = new object[] { new { type = "INVOKE", sender_address = _senderAddress, calldata = _calldata, max_fee = maxFee, version = "0x1", signature = new string[] { r, s }, nonce = _nonce } }
                };
                _platform.PlatformRequest(requestData, response =>
                                OnTransactionSendComplete(platform, response)
                            );
            }
            catch (Exception ex)
            {
                platform.PlatformLog("Something went wrong: " + ex.Message, NotificationType.Warning);
            }
        }


        private static void OnTransactionSendComplete(Platform platform, object result)
        {
            JsonRpcResponse response = result as JsonRpcResponse;
            if (response != null)
            {
                string jsonResponse = JsonConvert.SerializeObject(response, Formatting.Indented);

                JObject jsonObject = JObject.Parse(jsonResponse);

                if (jsonObject["error"] != null)
                {
                    string errorMessage = jsonObject["error"]["message"]?.ToString() ?? "No error message provided";
                    platform.PlatformLog($"Transaction send; But Transaction Status Failed! {errorMessage}", NotificationType.Error);
                }
                else
                {
                    platform.PlatformLog($"Transaction Send Complete! Result =>" + jsonResponse, NotificationType.Success);
                }
            }
            else
            {
                platform.PlatformLog("Result is not of type StarkSharp.Rpc.JsonRpcResponse", NotificationType.Error);
            }
        }
    }
}
