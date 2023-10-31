using StarkSharp.Connectors.Components;
using System;
using System.Numerics;
using static StarkSharp.Fusion.Sharpion.Dotnet.Handlers.Enum;

namespace StarkSharp.Fusion.Sharpion.Dotnet.Handlers
{
    public class Packs
    {
        private static System.Random random = new System.Random();

        // Creates and returns a LoginPacket with provided information.
        public static LoginPacket CreateLoginPack(bool durum, bool auth)
        {
            LoginPacket packetlogin = new LoginPacket(); // Initialize a new instance of LoginPacket.
            // Generate a random packet ID between 0 and 99999999.
            packetlogin.packetid = random.Next(0, 99999999).ToString();
            packetlogin.type = (int)ClientEnum.Login;
            packetlogin.islog = durum;
            packetlogin.wo = null; // Work order indicates the source or usage purpose.
            return packetlogin; // Return the fully formed login packet.
        }

        // Creates and returns a DisconnectPacket with provided information.
        public static DisconnectPacket CreateDisconnectPack(int socketid, bool durum, bool auth)
        {
            DisconnectPacket packetdisconnect = new DisconnectPacket(); // Initialize a new instance of LoginPacket.
            // Generate a random packet ID between 0 and 99999999.
            packetdisconnect.packetid = random.Next(0, 99999999).ToString();
            packetdisconnect.SocketID = socketid;
            packetdisconnect.type = (int)ClientEnum.Disconnect;
            packetdisconnect.disconnect = durum;
            return packetdisconnect; // Return the fully formed login packet.
        }

        // Creates and returns a BalancePacket packet with provided information.
        public static BalancePacket CreateBalanceOfPack(int socketid, string walletad)
        {
            BalancePacket packetbalance = new BalancePacket(); // Initialize a new instance of BalancePacket.
            // Generate a random packet ID between 0 and 99999999.
            packetbalance.packetid = random.Next(0, 99999999).ToString();
            // Set the type of the packet using the ClientEnum.
            packetbalance.type = (int)ClientEnum.Balance;
            // Set the public wallet address with the provided wallet address.
            packetbalance.WalletAdress = walletad;
            return packetbalance; // Return the fully formed packet.
        }
        // Creates and returns a basic TransactionPacket.
        public static TransactionPacket CreateTransactionPack(int socketid, TransactionInteraction interaction)
        {
            TransactionPacket packettransaction = new TransactionPacket(); // Initialize a new instance of TransactionPacket.
            // Generate a random packet ID between 0 and 99999999.
            packettransaction.packetid = random.Next(0, 99999999).ToString();
            packettransaction.ContractPack = interaction;

            // Set the type of the packet using the ClientEnum.
            packettransaction.type = (int)ClientEnum.Transaction;
            return packettransaction; // Return the fully formed transaction packet.
        }
        // Base packet class to represent the structure of Data exchanged between server and client.
        public class Packet
        {
            public string packetid;     // Unique identifier for the packet.
            public int SocketID;           // General identifier, could be user ID or another relevant ID.
            public int type;         // Type of packet, referring to ClientEnum or ServerEnum.
            public string status;       // Status of the request or response (e.g., success, failed).
            public string message;      // General Message or Data from server or client.
            public string wo;           // Work order or specific instruction related to the packet.
        }

        // Packet specifically for exchanging web related information.
        public class ConnectionWalletPack : Packet
        {
            public string PublicWallet;  // Public wallet address
            public string WalletBalance;  // Public wallet balance
        }

        // Packet specifically for balance related information.
        public class BalancePacket : Packet
        {
            public string WalletAdress; // Public wallet address of the client.
            public string BalanceOfEth;       // Balance of Ethereum in the client's wallet.
        }

        // Packet used for login actions.
        public class LoginPacket : Packet
        {
            public bool islog;       // Indicates if the client is currently logged in.
            public bool auth;        // Indicates if the client has authenticated successfully.
        }

        public class DisconnectPacket : Packet
        {
            public bool disconnect;  // client has disconnect
        }

        // Packet used for registration actions.
        public class Register : Packet
        {
            public string MacAdress; // MAC address of the client's device.
        }
        // Packet specifically for transaction related information.
        public class TransactionPacket : Packet
        {
            public string TransactionStatusMessage;  // Message or status about a specific transaction.
            public object ContractPack;
        }
    }
}
