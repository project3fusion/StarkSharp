<h1>StarkSharp Basics</h1>

Starksharp is built for helping developers to connect their apps and games to Starknet easily.

<h4>1. Create Platform</h4>

First of all you have to create a platform.

What is a platform?

Platform is the target platform you will make your app or game to work. For example DotNet can be easily used with a windows forms application while Unity can be used for Unity games.

Most important point in here is, Unity for example have three different options: WebGL, Sharpion and RPC. You can define the type with PlatformType enum.

```
Platform myNewPlatform = UnityPlatform.New(PlatformConnectorType.WebGL);
```

[Supported Platforms](../Platforms/README.md)

<h4>2. Create Connector</h4>

After setting the platform, you have to create a connector which will use that platform to connect your app to the starknet.

```
Connector connector = new Connector(myNewPlatform);
```

<h4>3. Connect Wallet</h4>

Now we are ready to connect our wallet.

ArgentX

```
connector.ConnectWallet(WalletType.ArgentX,
    (successMessage) => OnWalletConnectionSuccess(successMessage),
    (errorMessage) => OnWalletConnectionError(errorMessage));
```

Braavos

```
connector.ConnectWallet(WalletType.Braavos,
    (successMessage) => OnWalletConnectionSuccess(successMessage),
    (errorMessage) => OnWalletConnectionError(errorMessage));
```

Supported Wallet List

<h4>4. Create Actions</h4>

We have to create actions for success or error situations,

```
public void OnWalletConnectionSuccess(string message)
{
    connector.DebugMessage("On Wallet Connection Success: " + message);
}

public void OnWalletConnectionError(string message)
{
    connector.DebugMessage("On Wallet Connection Error: " + message);
}
```

<h4>5. Send Transaction</h4>

```
connector.SendTransaction(
    ERC20Standart.TransferToken(sendTransactionContractAddress, sendTransactionRecipientAddress, amount),
    (successMessage) => OnSendTransactionSuccess(successMessage),
    (errorMessage) => OnSendTransactionError(errorMessage));
```

If you are wondering how to send a custom transaction, here is how you can do it:

```
connector.SendTransaction(
    new List<string> { contractAddress, entryPoint, callDataJson },
    (successMessage) => OnSendTransactionSuccess(successMessage),
    (errorMessage) => OnSendTransactionError(errorMessage));
```

Here contractAddress is the address of your contract, entry point is the function and call data json is the callData you have in json format.

Please keep in mind that, StarkSharp overall uses Newtonsoft.Json (as stated in the setup) so please install Newtonsoft.Json library.

<h4>6. Send Queries</h4>

Okey, let's talk about how to send queries. In order to send queries you can use RPC connectors.

Let's define an RPC connector.

```
Platform myNewPlatform = DotNetPlatform.New(PlatformConnectorType.RPC);
```

Dotnet, Unity and Godot uses different api approaches. Why? Because Normal DotNet HTTP Requests don't work on Unity or Godot. They have their specialized functions, that's why StarkSharp needs you to define which platform you are using.

After setting your platform, you can send your queries:

```
connector.CallContract(
    ERC20Standart.BalanceOf(callContractContractAddress, callOtherUserWalletAddress),
    (successMessage) => OnCallContractSuccess(successMessage),
    (errorMessage) => OnCallContractError(errorMessage));
```

If you want to send a custom query, here is the code

```
ContractInteraction myContract = new ContractInteraction(_ContractAdress, _EntryPoint, _CallData);
connector.CallContract(
       myContract,
       (successMessage) => OnCallContractSuccess(successMessage),
       (errorMessage) => OnCallContractError(errorMessage)
    );
```

<h4>7. You are Ready to Go!</h4>

If you are looking for examples, check [examples](../StarkSharp/StarkSharp.Examples/).
