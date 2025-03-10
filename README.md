# Ogama.NetWork

一个全面的Minecraft .NET 8组件库，提供游戏安装、身份验证和网络功能。

## 功能特性

- 游戏安装
  - 支持原版Minecraft安装
  - 支持Forge模组加载器安装
  - 支持Fabric模组加载器安装
  - 支持整合包安装
  - 可扩展的安装器接口设计

- 身份验证
  - 支持Microsoft账户登录
  - 支持离线模式登录
  - 支持外部验证服务器登录
  - 可扩展的验证器接口设计

- 游戏运行时
  - 游戏进程管理
  - 日志记录和分析
  - 游戏状态监控

- 网络功能
  - P2P联机支持
  - 网络状态管理
  - 可扩展的网络接口设计

## 安装

通过NuGet包管理器安装：

```powershell
Install-Package Ogama.NetWork
```

或者使用.NET CLI：

```bash
dotnet add package Ogama.NetWork
```

## 快速开始

### 游戏安装

```csharp
// 创建原版安装器实例
var installer = new VanillaInstaller();

// 执行安装
var result = await installer.InstallAsync(new InstallOptions
{
    GameVersion = "1.19.2",
    InstallPath = "path/to/minecraft"
});
```

### 身份验证

```csharp
// 创建Microsoft验证器实例
var authenticator = new MicrosoftAuthenticator();

// 执行登录
var result = await authenticator.AuthenticateAsync("your-access-token");
```

### 网络功能

```csharp
// 创建网络管理器实例
var networkManager = new NetworkManager();

// 连接到P2P网络
await networkManager.ConnectAsync("network-id");
```

## 文档

详细文档请参考：

- [安装模块](docs/installation.md)
- [身份验证](docs/authentication.md)
- [运行时](docs/runtime.md)

## 许可证

本项目采用GPL-3.0协议开源，详见[LICENSE](LICENSE)文件。

## 贡献

欢迎提交Issue和Pull Request！

## 更新日志

### 0.1.0

- 初始版本发布
- 实现基础游戏安装功能
- 实现基础身份验证功能
- 实现基础网络功能