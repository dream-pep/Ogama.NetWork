using System;
using System.Threading.Tasks;

namespace Ogama.NetWork.Runtime
{
    /// <summary>
    /// 定义Minecraft游戏进程管理的基础接口
    /// </summary>
    public interface IGameProcess : IDisposable
    {
        /// <summary>
        /// 获取游戏进程ID
        /// </summary>
        int ProcessId { get; }

        /// <summary>
        /// 获取游戏工作目录
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        /// 获取游戏是否正在运行
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// 获取游戏内存使用量（MB）
        /// </summary>
        double MemoryUsage { get; }

        /// <summary>
        /// 启动游戏
        /// </summary>
        /// <returns>启动结果</returns>
        Task<GameProcessResult> StartAsync();

        /// <summary>
        /// 停止游戏
        /// </summary>
        /// <returns>停止结果</returns>
        Task<GameProcessResult> StopAsync();

        /// <summary>
        /// 重启游戏
        /// </summary>
        /// <returns>重启结果</returns>
        Task<GameProcessResult> RestartAsync();
    }

    /// <summary>
    /// 游戏进程操作结果类
    /// </summary>
    public class GameProcessResult
    {
        /// <summary>
        /// 获取或设置是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 获取或设置错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 获取或设置退出代码
        /// </summary>
        public int? ExitCode { get; set; }
    }
}