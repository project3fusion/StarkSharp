using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StarkSharp.Connectors.Components;
using StarkSharp.Platforms;
using StarkSharp.Rpc.Modules.Transactions.Hash;
using StarkSharp.StarkCurve.Signature;
using StarkSharp.Tools.Notification;
using System;


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
                    string serializedFunctionArgs = JsonConvert.SerializeObject(transactionInteraction.FunctionArgs);
                    TransactionRpc requestData = JsonRpcHandler.GenerateTransactionRequestData(
                        transactionInteraction.SenderAddress,
                        serializedFunctionArgs,
                        transactionInteraction.MaxFee,
                        nonce.ToString(),
                        new string[] { r, s },
                        "INVOKE",
                        transactionInteraction.Version
                    );
                    _platform.PlatformRequest(requestData, response =>
                        OnTransactionSendComplete(platform,response)
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

        private static void OnTransactionSendComplete(Platform platform,object result)
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
