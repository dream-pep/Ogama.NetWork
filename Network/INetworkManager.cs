using System;
using System.Threading.Tasks;

namespace Ogama.NetWork.Network
{
    /// <summary>
    /// 定义P2P网络管理的基础接口
    /// </summary>
    public interface INetworkManager : IDisposable
    {
        /// <summary>
        /// 获取网络状态
        /// </summary>
        NetworkStatus Status { get; }

        /// <summary>
        /// 获取本地节点ID
        /// </summary>
        string LocalNodeId { get; }

        /// <summary>
        /// 获取网络延迟（毫秒）
        /// </summary>
        int Latency { get; }

        /// <summary>
        /// 初始化网络
        /// </summary>
        /// <returns>初始化结果</returns>
        Task<NetworkResult> InitializeAsync();

        /// <summary>
        /// 加入网络
        /// </summary>
        /// <param name="networkId">网络ID</param>
        /// <returns>加入结果</returns>
        Task<NetworkResult> JoinAsync(string networkId);

        /// <summary>
        /// 离开网络
        /// </summary>
        /// <returns>离开结果</returns>
        Task<NetworkResult> LeaveAsync();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据内容</param>
        /// <param name="targetNodeId">目标节点ID</param>
        /// <returns>发送结果</returns>
        Task<NetworkResult> SendAsync(byte[] data, string targetNodeId);

        /// <summary>
        /// 广播数据
        /// </summary>
        /// <param name="data">数据内容</param>
        /// <returns>广播结果</returns>
        Task<NetworkResult> BroadcastAsync(byte[] data);
    }

    /// <summary>
    /// 网络状态枚举
    /// </summary>
    public enum NetworkStatus
    {
        /// <summary>
        /// 未初始化
        /// </summary>
        Uninitialized,

        /// <summary>
        /// 已初始化
        /// </summary>
        Initialized,

        /// <summary>
        /// 已连接
        /// </summary>
        Connected,

        /// <summary>
        /// 已断开
        /// </summary>
        Disconnected,

        /// <summary>
        /// 错误
        /// </summary>
        Error
    }

    /// <summary>
    /// 网络操作结果类
    /// </summary>
    public class NetworkResult
    {
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