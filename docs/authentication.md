# 身份验证模块使用说明

## 概述
身份验证模块提供了多种Minecraft账户验证方式，包括Microsoft账户验证、离线验证等。通过该模块，您可以轻松实现游戏账户的登录和身份验证功能。

## 功能特性
- 支持Microsoft账户验证
- 支持离线模式验证
- 可扩展的验证器接口设计

## 使用方法

### Microsoft账户验证
```csharp
// 创建Microsoft验证器实例
var authenticator = new MicrosoftAuthenticator();

// 执行验证
var result = await authenticator.AuthenticateAsync();
if (result.IsSuccess)
{
    // 验证成功，获取玩家信息
    var playerName = result.PlayerName;
    var uuid = result.UUID;
    var accessToken = result.AccessToken;
}
```

### 离线模式验证
```csharp
// 创建离线验证器实例
var authenticator = new OfflineAuthenticator("PlayerName");

// 执行验证
var result = await authenticator.AuthenticateAsync();
if (result.IsSuccess)
{
    // 验证成功，获取玩家信息
    var playerName = result.PlayerName;
    var uuid = result.UUID;
}
```

### 自定义验证器
您可以通过实现`IAuthenticator`接口来创建自定义的验证器：
```csharp
public class CustomAuthenticator : IAuthenticator
{
    public async Task<AuthenticationResult> AuthenticateAsync()
    {
        // 实现自定义验证逻辑
    }
}
```

## 注意事项
1. Microsoft账户验证需要网络连接
2. 离线模式验证仅适用于支持离线模式的服务器
3. 验证成功后请妥善保存身份凭证

## API参考

### IAuthenticator 接口
```csharp
public interface IAuthenticator
{
    Task<AuthenticationResult> AuthenticateAsync();
}
```

### AuthenticationResult 类
```csharp
public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? PlayerName { get; set; }
    public string? UUID { get; set; }
    public string? AccessToken { get; set; }
}
```

## 错误处理
验证过程中可能出现的错误：
- 网络连接失败
- 账户凭证无效
- 验证服务器无响应

请通过检查`AuthenticationResult.IsSuccess`和`AuthenticationResult.ErrorMessage`来处理可能的错误情况。