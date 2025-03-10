# 运行时管理模块使用说明

## 概述
运行时管理模块提供了Minecraft游戏进程的管理功能，包括进程启动、停止、重启以及日志收集等功能。通过该模块，您可以轻松控制游戏进程的生命周期并监控其运行状态。

## 功能特性
- 游戏进程生命周期管理
- 进程状态监控
- 内存使用量监控
- 游戏日志收集
- 可扩展的进程管理接口设计

## 使用方法

### 游戏进程管理
```csharp
// 创建游戏进程管理器实例
var gameProcess = new GameProcess(
    javaPath: "path/to/java.exe",
    workingDirectory: "path/to/minecraft",
    arguments: new[] { "-Xmx2G", "-jar", "minecraft.jar" }
);

// 启动游戏
var startResult = await gameProcess.StartAsync();
if (startResult.IsSuccess)
{
    // 游戏启动成功
    Console.WriteLine($"进程ID：{gameProcess.ProcessId}");
    Console.WriteLine($"内存使用：{gameProcess.MemoryUsage:F2} MB");
}

// 停止游戏
var stopResult = await gameProcess.StopAsync();
if (stopResult.IsSuccess)
{
    Console.WriteLine($"游戏已停止，退出代码：{stopResult.ExitCode}");
}

// 重启游戏
var restartResult = await gameProcess.RestartAsync();
if (restartResult.IsSuccess)
{
    Console.WriteLine("游戏已重启");
}
```

### 游戏日志收集
```csharp
// 创建日志收集器实例
var logger = new GameLogger(gameProcess);

// 订阅日志事件
logger.OnLogReceived += (sender, log) =>
{
    Console.WriteLine($"[{log.Level}] {log.Message}");
};

// 开始收集日志
logger.Start();

// 停止收集日志
logger.Stop();
```

### 自定义进程管理器
您可以通过实现`IGameProcess`接口来创建自定义的进程管理器：
```csharp
public class CustomGameProcess : IGameProcess
{
    public int ProcessId => // 实现进程ID获取
    public string WorkingDirectory => // 实现工作目录获取
    public bool IsRunning => // 实现运行状态获取
    public double MemoryUsage => // 实现内存使用量获取

    public Task<GameProcessResult> StartAsync()
    {
        // 实现启动逻辑
    }

    public Task<GameProcessResult> StopAsync()
    {
        // 实现停止逻辑
    }

    public Task<GameProcessResult> RestartAsync()
    {
        // 实现重启逻辑
    }

    public void Dispose()
    {
        // 实现资源释放
    }
}
```

## 注意事项
1. 确保Java路径正确且版本兼容
2. 合理设置内存参数避免内存溢出
3. 正确处理进程的异常退出情况
4. 及时释放不再使用的进程资源

## API参考

### IGameProcess 接口
```csharp
public interface IGameProcess : IDisposable
{
    int ProcessId { get; }
    string WorkingDirectory { get; }
    bool IsRunning { get; }
    double MemoryUsage { get; }

    Task<GameProcessResult> StartAsync();
    Task<GameProcessResult> StopAsync();
    Task<GameProcessResult> RestartAsync();
}
```

### GameProcessResult 类
```csharp
public class GameProcessResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public int? ExitCode { get; set; }
}
```

### GameLogger 类
```csharp
public class GameLogger
{
    public event EventHandler<GameLogEventArgs> OnLogReceived;
    
    public void Start();
    public void Stop();
}
```

## 错误处理
运行时可能出现的错误：
- Java路径无效
- 内存分配失败
- 进程异常退出
- 文件访问权限不足
- 端口被占用

请通过检查`GameProcessResult.IsSuccess`和`GameProcessResult.ErrorMessage`来处理可能的错误情况。