namespace StarkSharp.Fusion.Sharpion.Dotnet.Handlers
{
    public class Enum
    {
        public enum ClientEnum // Enum listing possible client actions or commands.
        {
            Register = 1,    // Represents client's intent to register.
            Login = 2,    // Represents client's intent to log in.
            Disconnect = 3,    // Represents client's intent to disconnect in.
            Command = 4,    // Represents a general command from the client.
            WalletPack = 5,    // Client is sending or requesting web related Data.
            Balance = 6,    // Client is sending or requesting balance information.
            Transaction = 7     // Client is sending or requesting transaction information.
        }
    }
}
