# StarkSharp Exception Handling System

Kapsamlı ve merkezi hata yakalama sistemi. Tüm hataları custom hata kodlarına dönüştürür ve standart bir yapıda sunar.

## Özellikler

- ✅ **Custom Exception Sınıfları**: Tüm hatalar için özel exception sınıfları
- ✅ **Hata Kodları**: 100+ önceden tanımlı hata kodu
- ✅ **Otomatik Dönüştürme**: Standart .NET exception'ları otomatik olarak StarkSharpException'a dönüştürür
- ✅ **Kategori Sistemi**: Hatalar kategorilere ayrılmış (Network, RPC, Wallet, Transaction, vb.)
- ✅ **Extension Metodları**: Kolay kullanım için extension metodları
- ✅ **Error Response**: JSON formatında standart hata yanıtı
- ✅ **Safe Execute**: Güvenli çalıştırma metodları

## Hata Kodları

### Genel Hatalar (1000-1999)
- `UnknownError` (1000)
- `InvalidParameter` (1001)
- `NullReference` (1002)
- `InvalidOperation` (1003)
- `Timeout` (1004)
- `Cancelled` (1005)

### Network Hataları (2000-2999)
- `NetworkError` (2000)
- `ConnectionFailed` (2001)
- `ConnectionTimeout` (2002)
- `RequestFailed` (2003)
- `ServerError` (2005)
- `ClientError` (2006)

### RPC Hataları (3000-3999)
- `RpcError` (3000)
- `RpcInvalidRequest` (3001)
- `RpcMethodNotFound` (3002)
- `RpcInvalidParams` (3003)
- `RpcInternalError` (3004)
- `RpcTimeout` (3007)

### Wallet Hataları (4000-4999)
- `WalletError` (4000)
- `WalletNotConnected` (4001)
- `WalletConnectionFailed` (4002)
- `WalletRejected` (4005)

### Transaction Hataları (5000-5999)
- `TransactionError` (5000)
- `TransactionFailed` (5001)
- `TransactionRejected` (5002)
- `TransactionReverted` (5003)
- `InsufficientFee` (5007)

### Contract Hataları (6000-6999)
- `ContractError` (6000)
- `ContractNotFound` (6001)
- `ContractCallFailed` (6002)
- `InvalidContractAddress` (6003)

## Kullanım

### Temel Kullanım

```csharp
try
{
    // Risky operation
    DoSomething();
}
catch (Exception ex)
{
    var starkSharpException = ex.ToStarkSharpException();
    Console.WriteLine($"Error Code: {starkSharpException.ErrorCode}");
    Console.WriteLine($"Category: {starkSharpException.ErrorCategory}");
    Console.WriteLine($"Message: {starkSharpException.Message}");
}
```

### SafeExecute Kullanımı

```csharp
var result = ErrorHandler.SafeExecute(() =>
{
    return RiskyOperation();
}, StarkSharpErrorCode.UnknownError);
```

### TryExecute Kullanımı

```csharp
var (success, result, error) = ErrorHandler.TryExecute(() =>
{
    return RiskyOperation();
});

if (success)
{
    Console.WriteLine($"Result: {result}");
}
else
{
    Console.WriteLine($"Error: {error.ErrorCode} - {error.Message}");
}
```

### RPC Hata Yönetimi

```csharp
var response = new JsonRpcResponse
{
    error = new JsonRpcError { code = -32601, message = "Method not found" }
};

var exception = ErrorHandler.HandleJsonRpcError(response);
```

### HTTP Hata Yönetimi

```csharp
var exception = ErrorHandler.HandleHttpStatusCode(
    HttpStatusCode.NotFound,
    "Resource not found"
);
```

### Extension Metodları

```csharp
try
{
    throw new TimeoutException();
}
catch (Exception ex)
{
    // Log exception
    ex.LogException(NotificationPlatform.Console);
    
    // Get user-friendly message
    var friendlyMessage = ex.GetUserFriendlyMessage();
    
    // Check error code
    if (ex.IsErrorCode(StarkSharpErrorCode.Timeout))
    {
        // Handle timeout
    }
    
    // Check category
    if (ex.IsErrorCategory("General"))
    {
        // Handle general error
    }
}
```

### ErrorResponse Oluşturma

```csharp
try
{
    throw new ArgumentException("Invalid argument");
}
catch (Exception ex)
{
    var errorResponse = ErrorResponse.FromException(ex);
    var json = errorResponse.ToJson();
    // Send JSON response to client
}
```

### Async Kullanım

```csharp
try
{
    var result = await ErrorHandler.SafeExecuteAsync(async () =>
    {
        await Task.Delay(100);
        return "Result";
    });
}
catch (StarkSharpException ex)
{
    Console.WriteLine($"Error: {ex.ErrorCode}");
}
```

## Hata Kategorileri

- **General**: Genel hatalar
- **Network**: Ağ hataları
- **RPC**: RPC hataları
- **Wallet**: Cüzdan hataları
- **Transaction**: İşlem hataları
- **Contract**: Kontrat hataları
- **Account**: Hesap hataları
- **Cryptographic**: Kriptografik hatalar
- **Platform**: Platform hataları
- **Configuration**: Yapılandırma hataları
- **Validation**: Doğrulama hataları
- **Serialization**: Serileştirme hataları

## Özelleştirme

### Yeni Hata Kodu Ekleme

1. `StarkSharpErrorCode` enum'ına yeni kod ekleyin
2. `ErrorCodeHelper`'a kategori ve mesaj ekleyin
3. `ErrorHandler.HandleException` metoduna mapping ekleyin (gerekirse)

### Custom Exception Oluşturma

```csharp
throw new StarkSharpException(
    StarkSharpErrorCode.CustomError,
    "Custom error message",
    additionalData: new { CustomField = "value" }
);
```

## Best Practices

1. **Her zaman StarkSharpException kullanın**: Tüm hataları StarkSharpException'a dönüştürün
2. **ErrorResponse kullanın**: API yanıtları için ErrorResponse kullanın
3. **Kategorilere göre gruplayın**: Hataları kategorilere göre işleyin
4. **Logging**: Tüm hataları loglayın
5. **User-friendly messages**: Kullanıcıya anlaşılır mesajlar gösterin

## Örnekler

Detaylı örnekler için `Examples/ErrorHandlingExamples.cs` dosyasına bakın.

