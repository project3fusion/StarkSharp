using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static StarkSharp.Fusion.Sharpion.Dotnet.Handlers.Enum;
using static StarkSharp.Fusion.Sharpion.Dotnet.Handlers.Packs;

namespace StarkSharp.Fusion.Sharpion.Dotnet.Handlers
{
    public class Handler
    {
        public static async Task HandShake(string datahandjson)
        {
            try
            {
                Console.WriteLine("Data From Server (HandShake): " + datahandjson);

                Packet handshakepacket = JsonConvert.DeserializeObject<Packet>(datahandjson);

                switch (handshakepacket.type)
                {
                    case (int)ClientEnum.Login:
                        await HandleLoginPacketAsync(JsonConvert.DeserializeObject<LoginPacket>(datahandjson));
                        break;
                    case (int)ClientEnum.WalletPack:
                        await HandleConnectionPacketAsync(JsonConvert.DeserializeObject<ConnectionWalletPack>(datahandjson));
                        break;
                    case (int)ClientEnum.Balance:
                        await HandleBalancePacketAsync(JsonConvert.DeserializeObject<BalancePacket>(datahandjson));
                        break;
                    case (int)ClientEnum.Transaction:
                        await HandleTransactionPacketAsync(JsonConvert.DeserializeObject<TransactionPacket>(datahandjson));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in HandShake: " + ex.Message);
            }
        }
        public static async Task HandleLoginPacketAsync(LoginPacket loginPacket)
        {
            // Check for null packets to avoid NullReferenceException.
            if (loginPacket == null)
            {
                Console.WriteLine("Received a null login packet in HandleLoginPacketAsync.");
                return;
            }
            try
            {
                // Check if user is logged in.
                if (loginPacket.islog)
                {
                    Console.WriteLine($"Login Data From Server: {loginPacket.message}");
                    // Additional logic for logged-in user can be placed here.
                }
                // Check if user is authenticated (e.g., connected their wallet).
                else if (loginPacket.auth)
                {
                    Console.WriteLine($"Login Data From Server (User Connected Wallet): {loginPacket.message}");
                    // Additional logic for authenticated user can be placed here.
                }
                // Handle other cases.
                else
                {
                    Console.WriteLine($"Login Data From Server: {loginPacket.message}");
                    return;
                }
            }
            // Catch any unexpected errors during processing.
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing the login packet in HandleLoginPacketAsync: {ex.Message}");
            }
        }
        public static async Task HandleConnectionPacketAsync(ConnectionWalletPack connectionPacket)
        {
            // Safeguard against null packets to prevent potential NullReferenceException.
            if (connectionPacket == null)
            {
                Console.WriteLine("Received a null connection packet in HandleConnectionPacketAsync.");
                return;
            }
            try
            {
                // If a public wallet address is provided, process it.
                if (!string.IsNullOrEmpty(connectionPacket.PublicWallet))
                {
                    Console.WriteLine($"Player Wallet Address: {connectionPacket.PublicWallet}");

                    // Update the user's wallet address in the Socket instance.
                    Client.instance.UserWalletAddress = connectionPacket.PublicWallet;
                }
            }
            // Catch any unexpected errors during packet processing.
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing the connection packet in HandleConnectionPacketAsync: {ex.Message}");
            }
        }
        public static async Task HandleBalancePacketAsync(BalancePacket balancePacket)
        {
            // Safeguard against null packets to prevent potential NullReferenceException.
            if (balancePacket == null)
            {
                Console.WriteLine("Received a null balance packet in HandleBalancePacketAsync.");
                return;
            }
            try
            {
                // If an Ethereum balance is provided, process it.
                if (!string.IsNullOrEmpty(balancePacket.BalanceOfEth))
                {
                    Console.WriteLine($"Player's Ethereum Balance: {balancePacket.BalanceOfEth}");
                    // Update the user's Ethereum balance in the Socket instance.
                    Client.instance.UserBalanceOfEth = balancePacket.BalanceOfEth;
                }
            }
            // Catch any unexpected errors during packet processing.
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing the balance packet in HandleBalancePacketAsync: {ex.Message}");
            }
        }
        public static async Task HandleTransactionPacketAsync(TransactionPacket transactionPacket)
        {
            // Safeguard against null packets to prevent potential NullReferenceException.
            if (transactionPacket == null)
            {
                Console.WriteLine("Received a null transaction packet in HandleTransactionPacketAsync.");
                return;
            }
            try
            {
                // If a transaction status message is provided, log it.
                if (!string.IsNullOrEmpty(transactionPacket.TransactionStatusMessage))
                {
                    Console.WriteLine($"Player Transaction Status: {transactionPacket.TransactionStatusMessage}");
                }
            }
            // Catch any unexpected errors during packet processing.
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing the transaction packet in HandleTransactionPacketAsync: {ex.Message}");
            }
        }
    }
}
