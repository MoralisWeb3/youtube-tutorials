# WalletConnectSharp Core

WalletConnectSharp is an implementation of the [WalletConnect](https://walletconnect.org/) protocol (currently only v1) using .NET and (optinoally) NEthereum. This library implements the [WalletConnect Technical Specification](https://docs.walletconnect.org/tech-spec) in .NET to allow C# dApps makers to add support for the open [WalletConnect](https://walletconnect.org/) protocol. This branch of the repo includes only the core library, which can be cloned as a submodule in other projects

#### :warning: **This is beta software**: This software is currently in beta and under development. Please proceed with caution, and open a new issue if you encounter a bug :warning:

## Usage

First you must define the `ClientMeta` you would like to send along with your connect request. This is what is shown in the Wallet UI

```csharp
var metadata = new ClientMeta()
{
    Description = "This is a test of the Nethereum.WalletConnect feature",
    Icons = new[] {"https://app.warriders.com/favicon.ico"},
    Name = "WalletConnect Test",
    URL = "https://app.warriders.com"
};    
```

Once you have the metadata, you can create the `WalletConnect` object

```csharp
var walletConnect = new WalletConnect(metadata);
Console.WriteLine(walletConnect.URI);
```

This will print the `wc` connect code into the console. You can transform this text into a QR code or use it for deep linking. Once you have the `wc` link displayed to the user, you can then call `Connect()`. The `Connect()` function will block until either a successful or rejected session response

```csharp
var walletConnectData = await walletConnect.Connect();
```

This function returns a `Task<WCSessionData>` object, so it can be awaited if your using async/await. The `WCSessionData` has data about the current session (accounts, chainId, etc..)

```csharp
Console.WriteLine(walletConnectData.accounts[0]);
Console.WriteLine(walletConnectData.chainId);
```

## Connecting with NEthereum

With the above, you have enough to use the base WalletConnect protocol. However, this library comes with an NEthereum provider implementation. To use it, you simply invoke `CreateProvider(url)` or `CreateProvider(IClient)`. You are required to specify an additional RPC URL or a custom `IClient` because the `WalletConnect` protocol does not perform read operations (`eth_call`, `eth_estimateGas`, etc..), so you must provide either an `Infura Project ID`, a node's HTTP url for `HttpProvider` or a custom `IClient`.

Here is an example
```csharp
var web3 = new Web3(walletConnect.CreateProvider(new Uri("https://mainnet.infura.io/v3/<infruaId>"));
```
