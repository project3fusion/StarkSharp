namespace StarkSharp.Core.Interfaces
{
    /// <summary>
    /// Configuration interface for StarkSharp settings
    /// </summary>
    public interface IStarkSharpConfiguration
    {
        string RpcUrl { get; set; }
        string ChainId { get; set; }
        PlatformName PlatformName { get; set; }
        PlatformConnectorType ConnectorType { get; set; }
        int DefaultMaxRetries { get; set; }
        float DefaultCheckInterval { get; set; }
    }
}

