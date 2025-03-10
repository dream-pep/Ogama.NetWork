using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Ogama.NetWork.Network
{
    /// <summary>
    /// 网络管理器实现
    /// </summary>
    public class NetworkManager : INetworkManager
    {
        private readonly string _zerotierPath;
        private readonly string _networkId;
        private Process? _zerotierProcess;
        private NetworkStatus _status = NetworkStatus.Uninitialized;
        private string? _localNodeId;
        private int _latency;

        public NetworkStatus Status => _status;
        public string LocalNodeId => _localNodeId ?? string.Empty;
        public int Latency => _latency;

        /// <summary>
        /// 初始化网络管理器
        /// </summary>
        /// <param name="zerotierPath">Zerotier可执行文件路径</param>
        /// <param name="networkId">Zerotier网络ID</param>
        public NetworkManager(string zerotierPath, string networkId)
        {
            _zerotierPath = zerotierPath;
            _networkId = networkId;
        }

        /// <summary>
        /// 获取网络状态
        /// </summary>
        public bool IsConnected => _zerotierProcess != null && !_zerotierProcess.HasExited;

        /// <summary>
        /// 获取网络地址
        /// </summary>
        public string? NetworkAddress { get; private set; }

        /// <summary>
        /// 连接到网络
        /// </summary>
        /// <returns>连接结果</returns>
        public async Task<NetworkResult> InitializeAsync()
        {
            try
            {
                _status = NetworkStatus.Initialized;
                return new NetworkResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                _status = NetworkStatus.Error;
                return new NetworkResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"初始化失败：{ex.Message}"
                };
            }
        }

        public async Task<NetworkResult> JoinAsync(string networkId)
        {
            try
            {
                if (IsConnected)
                {
                    return new NetworkResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "已连接到网络"
                    };
                }

                // 验证Zerotier程序
                if (!File.Exists(_zerotierPath))
                {
                    return new NetworkResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "未找到Zerotier程序"
                    };
                }

                // 启动Zerotier
                _zerotierProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _zerotierPath,
                        Arguments = $"join {_networkId}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                _zerotierProcess.Start();
                await Task.Delay(5000); // 等待网络连接

                // 获取网络地址
                var addressProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _zerotierPath,
                        Arguments = "listnetworks",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                addressProcess.Start();
                var output = await addressProcess.StandardOutput.ReadToEndAsync();
                await addressProcess.WaitForExitAsync();

                if (output.Contains(_networkId))
                {
                    var addressStart = output.IndexOf(_networkId) + _networkId.Length;
                    var addressEnd = output.IndexOf("\n", addressStart);
                    NetworkAddress = output.Substring(addressStart, addressEnd - addressStart).Trim();

                    return new NetworkResult { IsSuccess = true };
                }

                return new NetworkResult
                {
                    IsSuccess = false,
                    ErrorMessage = "无法获取网络地址"
                };
            }
            catch (Exception ex)
            {
                return new NetworkResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"连接网络失败：{ex.Message}"
                };
            }
        }

        /// <summary>
        /// 断开网络连接
        /// </summary>
        /// <returns>断开结果</returns>
        public async Task<NetworkResult> DisconnectAsync()
        {
            try
            {
                if (!IsConnected)
                {
                    return new NetworkResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "未连接到网络"
                    };
                }

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _zerotierPath,
                        Arguments = $"leave {_networkId}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                if (_zerotierProcess != null && !_zerotierProcess.HasExited)
                {
                    _zerotierProcess.Kill();
                    _zerotierProcess.Dispose();
                    _zerotierProcess = null;
                }

                NetworkAddress = null;

                return new NetworkResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new NetworkResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"断开网络失败：{ex.Message}"
                };
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_zerotierProcess != null)
            {
                if (!_zerotierProcess.HasExited)
                {
                    _zerotierProcess.Kill();
                }
                _zerotierProcess.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }

    public async Task<NetworkResult> SendAsync(byte[] data, string targetNodeId)
    {
        if (_status != NetworkStatus.Connected)
        {
            return new NetworkResult
            {
                IsSuccess = false,
                ErrorMessage = "未连接到网络"
            };
        }

        // TODO: 实现P2P数据发送逻辑
        return new NetworkResult { IsSuccess = true };
    }

    public async Task<NetworkResult> BroadcastAsync(byte[] data)
    {
        if (_status != NetworkStatus.Connected)
        {
            return new NetworkResult
            {
                IsSuccess = false,
                ErrorMessage = "未连接到网络"
            };
        }

        // TODO: 实现P2P广播逻辑
        return new NetworkResult { IsSuccess = true };
    }

    public async Task<NetworkResult> LeaveAsync()
    {
        return await DisconnectAsync();
    }
        /// <summary>
        /// 获取或设置是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 获取或设置错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}