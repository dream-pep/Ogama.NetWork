using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ogama.NetWork.Runtime
{
    /// <summary>
    /// 游戏日志收集器
    /// </summary>
    public class GameLogger : IDisposable
    {
        private readonly string _logDirectory;
        private readonly string _gameVersion;
        private readonly StringBuilder _logBuffer;
        private StreamWriter? _logWriter;
        private bool _isDisposed;

        /// <summary>
        /// 初始化日志收集器
        /// </summary>
        /// <param name="logDirectory">日志目录</param>
        /// <param name="gameVersion">游戏版本</param>
        public GameLogger(string logDirectory, string gameVersion)
        {
            _logDirectory = logDirectory;
            _gameVersion = gameVersion;
            _logBuffer = new StringBuilder();
            _isDisposed = false;

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        /// <summary>
        /// 开始记录日志
        /// </summary>
        public void StartLogging()
        {
            var logPath = Path.Combine(_logDirectory, $"game-{_gameVersion}-{DateTime.Now:yyyyMMdd-HHmmss}.log");
            _logWriter = new StreamWriter(logPath, false, Encoding.UTF8);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="level">日志级别</param>
        public async Task LogAsync(string message, LogLevel level = LogLevel.Info)
        {
            if (_isDisposed || _logWriter == null)
            {
                return;
            }

            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            _logBuffer.AppendLine(logMessage);

            await _logWriter.WriteLineAsync(logMessage);
            await _logWriter.FlushAsync();

            if (_logBuffer.Length > 1024 * 1024) // 限制缓冲区大小为1MB
            {
                _logBuffer.Clear();
            }
        }

        /// <summary>
        /// 分析崩溃日志
        /// </summary>
        /// <returns>崩溃分析结果</returns>
        public CrashAnalysisResult AnalyzeCrash()
        {
            var result = new CrashAnalysisResult();
            var logs = _logBuffer.ToString();

            // 检查常见的崩溃原因
            if (logs.Contains("OutOfMemoryError"))
            {
                result.CrashType = CrashType.OutOfMemory;
                result.Description = "游戏内存不足";
                result.Solution = "增加游戏内存分配或减少资源包数量";
            }
            else if (logs.Contains("java.lang.ClassNotFoundException"))
            {
                result.CrashType = CrashType.MissingClass;
                result.Description = "缺少必要的类文件";
                result.Solution = "重新安装游戏或检查Mod完整性";
            }
            else if (logs.Contains("java.lang.NoClassDefFoundError"))
            {
                result.CrashType = CrashType.MissingDependency;
                result.Description = "缺少依赖项";
                result.Solution = "安装缺失的Mod或更新现有Mod";
            }
            else
            {
                result.CrashType = CrashType.Unknown;
                result.Description = "未知错误";
                result.Solution = "请查看完整日志以获取详细信息";
            }

            return result;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _logWriter?.Dispose();
                _logBuffer.Clear();
                _isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 日志级别枚举
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 调试信息
        /// </summary>
        Debug,

        /// <summary>
        /// 一般信息
        /// </summary>
        Info,

        /// <summary>
        /// 警告信息
        /// </summary>
        Warning,

        /// <summary>
        /// 错误信息
        /// </summary>
        Error
    }

    /// <summary>
    /// 崩溃类型枚举
    /// </summary>
    public enum CrashType
    {
        /// <summary>
        /// 内存不足
        /// </summary>
        OutOfMemory,

        /// <summary>
        /// 缺少类
        /// </summary>
        MissingClass,

        /// <summary>
        /// 缺少依赖
        /// </summary>
        MissingDependency,

        /// <summary>
        /// 未知错误
        /// </summary>
        Unknown
    }

    /// <summary>
    /// 崩溃分析结果类
    /// </summary>
    public class CrashAnalysisResult
    {
        /// <summary>
        /// 获取或设置崩溃类型
        /// </summary>
        public CrashType CrashType { get; set; }

        /// <summary>
        /// 获取或设置崩溃描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 获取或设置解决方案
        /// </summary>
        public string? Solution { get; set; }
    }
}