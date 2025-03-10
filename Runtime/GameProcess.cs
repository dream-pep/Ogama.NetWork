using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ogama.NetWork.Runtime
{
    /// <summary>
    /// Minecraft游戏进程管理实现
    /// </summary>
    public class GameProcess : IGameProcess
    {
        private readonly string _javaPath;
        private readonly string _workingDirectory;
        private readonly string[] _arguments;
        private Process? _process;

        /// <summary>
        /// 初始化游戏进程管理器
        /// </summary>
        /// <param name="javaPath">Java可执行文件路径</param>
        /// <param name="workingDirectory">游戏工作目录</param>
        /// <param name="arguments">启动参数</param>
        public GameProcess(string javaPath, string workingDirectory, string[] arguments)
        {
            _javaPath = javaPath;
            _workingDirectory = workingDirectory;
            _arguments = arguments;
        }

        /// <summary>
        /// 获取游戏进程ID
        /// </summary>
        public int ProcessId => _process?.Id ?? -1;

        /// <summary>
        /// 获取游戏工作目录
        /// </summary>
        public string WorkingDirectory => _workingDirectory;

        /// <summary>
        /// 获取游戏是否正在运行
        /// </summary>
        public bool IsRunning => _process != null && !_process.HasExited;

        /// <summary>
        /// 获取游戏内存使用量（MB）
        /// </summary>
        public double MemoryUsage => _process?.WorkingSet64 / 1024.0 / 1024.0 ?? 0;

        /// <summary>
        /// 启动游戏
        /// </summary>
        /// <returns>启动结果</returns>
        public Task<GameProcessResult> StartAsync()
        {
            try
            {
                if (IsRunning)
                {
                    return Task.FromResult(new GameProcessResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "游戏进程已在运行"
                    });
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = _javaPath,
                    WorkingDirectory = _workingDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = false
                };

                foreach (var argument in _arguments)
                {
                    startInfo.ArgumentList.Add(argument);
                }

                _process = new Process { StartInfo = startInfo };
                _process.Start();

                return Task.FromResult(new GameProcessResult
                {
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new GameProcessResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"启动游戏失败：{ex.Message}"
                });
            }
        }

        /// <summary>
        /// 停止游戏
        /// </summary>
        /// <returns>停止结果</returns>
        public async Task<GameProcessResult> StopAsync()
        {
            try
            {
                if (!IsRunning)
                {
                    return new GameProcessResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "游戏进程未在运行"
                    };
                }

                _process!.CloseMainWindow();
                await Task.Delay(5000); // 等待游戏正常关闭

                if (!_process.HasExited)
                {
                    _process.Kill(); // 强制结束进程
                }

                return new GameProcessResult
                {
                    IsSuccess = true,
                    ExitCode = _process.ExitCode
                };
            }
            catch (Exception ex)
            {
                return new GameProcessResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"停止游戏失败：{ex.Message}"
                };
            }
        }

        /// <summary>
        /// 重启游戏
        /// </summary>
        /// <returns>重启结果</returns>
        public async Task<GameProcessResult> RestartAsync()
        {
            var stopResult = await StopAsync();
            if (!stopResult.IsSuccess)
            {
                return stopResult;
            }

            return await StartAsync();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (IsRunning)
            {
                _process!.Kill();
                _process.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}