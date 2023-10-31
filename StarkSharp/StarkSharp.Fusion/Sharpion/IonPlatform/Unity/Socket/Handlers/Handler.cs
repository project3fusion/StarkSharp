using System;
using System.Threading.Tasks;

using UnityEngine;

using static StarkSharp.Fusion.Sharpion.Unity.Handlers.Enum;
using static StarkSharp.Fusion.Sharpion.Unity.Handlers.Packs;

namespace StarkSharp.Fusion.Sharpion.Unity.Handlers
{
    public class Handler : MonoBehaviour
    {
        public static async Task HandShake(string datahandjson)
        {
            try
            {
                Packet handshakepacket = JsonUtility.FromJson<Packet>(datahandjson);
                switch (handshakepacket.type)
                {
                    case (int)ClientEnum.Login:
                        await HandleLoginPacketAsync(JsonUtility.FromJson<LoginPacket>(datahandjson));
                        break;
                    case (int)ClientEnum.WalletPack:
                        await HandleConnectionPacketAsync(JsonUtility.FromJson<WalletPack>(datahandjson));
                        break;
                    case (int)ClientEnum.Balance:
                        await HandleBalancePacketAsync(JsonUtility.FromJson<BalancePacket>(datahandjson));
                        break;
                    case (int)ClientEnum.Transaction:
                        await HandleTransactionPacketAsync(JsonUtility.FromJson<TransactionPacket>(datahandjson));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error in HandShake: " + ex.Message);
            }
        }
        public static async Task HandleLoginPacketAsync(LoginPacket loginPacket)
        {
            if (loginPacket == null)
            {
                Debug.LogError("Received a null login packet in HandleLoginPacketAsync.");
                return;
            }
            try
            {
                if (loginPacket.islog)
                {
                    Debug.Log($"Login Data From Server: {loginPacket.SocketID}, {loginPacket.message}, {loginPacket.auth} ,{loginPacket.message} ,{loginPacket.packetid}");
                }
                else if (loginPacket.auth)
                {
                    Debug.Log($"Login Data From Server (User Connected Wallet): {loginPacket.message}");
                }
                else
                {
                    Debug.Log($"Login Data From Server: {loginPacket.message}");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error processing the login packet in HandleLoginPacketAsync: {ex.Message}");
            }
        }
        public static async Task HandleConnectionPacketAsync(WalletPack connectionPacket)
        {
            // Safeguard against null packets to prevent potential NullReferenceException.
            if (connectionPacket == null)
            {
                Debug.LogError("Received a null connection packet in HandleConnectionPacketAsync.");
                return;
            }
            try
            {
                // If a public wallet address is provided, process it.
                if (!string.IsNullOrEmpty(connectionPacket.PublicWallet))
                {
                    Debug.Log($"Player Wallet Address: {connectionPacket.PublicWallet}");

                    // Update the user's wallet address in the Socket instance.
                    Socket.instance.UserWalletAddress = connectionPacket.PublicWallet;
                }
            }
            // Catch any unexpected errors during packet processing.
            catch (Exception ex)
            {
                Debug.LogError($"Error processing the connection packet in HandleConnectionPacketAsync: {ex.Message}");
            }
        }
        public static async Task HandleBalancePacketAsync(BalancePacket balancePacket)
        {
            // Safeguard against null packets to prevent potential NullReferenceException.
            if (balancePacket == null)
            {
                Debug.LogError("Received a null balance packet in HandleBalancePacketAsync.");
                return;
            }
            try
            {
                // If an Ethereum balance is provided, process it.
                if (!string.IsNullOrEmpty(balancePacket.BalanceOfEth))
                {
                    Debug.Log($"Player's Ethereum Balance: {balancePacket.BalanceOfEth}");
                    // Update the user's Ethereum balance in the Socket instance.
                    Socket.instance.UserBalanceOfEth = balancePacket.BalanceOfEth;
                }
            }
            // Catch any unexpected errors during packet processing.
            catch (Exception ex)
            {
                Debug.LogError($"Error processing the balance packet in HandleBalancePacketAsync: {ex.Message}");
            }
        }
        public static async Task HandleTransactionPacketAsync(TransactionPacket transactionPacket)
        {
            // Safeguard against null packets to prevent potential NullReferenceException.
            if (transactionPacket == null)
            {
                Debug.LogError("Received a null transaction packet in HandleTransactionPacketAsync.");
                return;
            }
            try
            {
                // If a transaction status message is provided, log it.
                if (!string.IsNullOrEmpty(transactionPacket.TransactionStatusMessage))
                {
                    Debug.Log($"Player Transaction Status: {transactionPacket.TransactionStatusMessage}");
                }
            }
            // Catch any unexpected errors during packet processing.
            catch (Exception ex)
            {
                Debug.LogError($"Error processing the transaction packet in HandleTransactionPacketAsync: {ex.Message}");
            }
        }
    }
}
