# 游戏安装模块使用说明

## 概述
游戏安装模块提供了多种Minecraft游戏版本的安装功能，包括原版（Vanilla）、Forge、Fabric以及整合包的安装。通过该模块，您可以轻松管理不同版本的游戏客户端安装。

## 功能特性
- 支持原版Minecraft安装
- 支持Forge模组加载器安装
- 支持Fabric模组加载器安装
- 支持整合包安装
- 可扩展的安装器接口设计

## 使用方法

### 原版安装
```csharp
// 创建原版安装器实例
var installer = new VanillaInstaller();

// 执行安装
var result = await installer.InstallAsync(new InstallOptions
{
    GameVersion = "1.19.2",
    InstallPath = "path/to/minecraft"
});

if (result.IsSuccess)
{
    // 安装成功
    Console.WriteLine($"安装完成：{result.InstalledVersion}");
}
```

### Forge安装
```csharp
// 创建Forge安装器实例
var installer = new ForgeInstaller();

// 执行安装
var result = await installer.InstallAsync(new InstallOptions
{
    GameVersion = "1.19.2",
    ForgeVersion = "43.2.0",
    InstallPath = "path/to/minecraft"
});

if (result.IsSuccess)
{
    // 安装成功
    Console.WriteLine($"安装完成：{result.InstalledVersion}");
}
```

### Fabric安装
```csharp
// 创建Fabric安装器实例
var installer = new FabricInstaller();

// 执行安装
var result = await installer.InstallAsync(new InstallOptions
{
    GameVersion = "1.19.2",
    FabricVersion = "0.14.21",
    InstallPath = "path/to/minecraft"
});

if (result.IsSuccess)
{
    // 安装成功
    Console.WriteLine($"安装完成：{result.InstalledVersion}");
}
```

### 整合包安装
```csharp
// 创建整合包安装器实例
var installer = new ModpackInstaller();

// 执行安装
var result = await installer.InstallAsync(new InstallOptions
{
    ModpackPath = "path/to/modpack.zip",
    InstallPath = "path/to/minecraft"
});

if (result.IsSuccess)
{
    // 安装成功
    Console.WriteLine($"安装完成：{result.InstalledVersion}");
}
```

### 自定义安装器
您可以通过实现`IInstaller`接口来创建自定义的安装器：
```csharp
public class CustomInstaller : IInstaller
{
    public async Task<InstallResult> InstallAsync(InstallOptions options)
    {
        // 实现自定义安装逻辑
    }
}
```

## 注意事项
1. 安装过程需要网络连接
2. 确保有足够的磁盘空间
3. 安装路径必须具有写入权限
4. 建议在安装前备份重要数据

## API参考

### IInstaller 接口
```csharp
public interface IInstaller
{
    Task<InstallResult> InstallAsync(InstallOptions options);
}
```

### InstallOptions 类
```csharp
public class InstallOptions
{
    public string? GameVersion { get; set; }
    public string? ForgeVersion { get; set; }
    public string? FabricVersion { get; set; }
    public string? ModpackPath { get; set; }
    public string InstallPath { get; set; }
}
```

### InstallResult 类
```csharp
public class InstallResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? InstalledVersion { get; set; }
}
```

## 错误处理
安装过程中可能出现的错误：
- 网络连接失败
- 文件下载错误
- 版本不兼容
- 磁盘空间不足
- 文件权限不足

请通过检查`InstallResult.IsSuccess`和`InstallResult.ErrorMessage`来处理可能的错误情况。