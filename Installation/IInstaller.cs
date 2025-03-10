using System.Threading.Tasks;

namespace Ogama.NetWork.Installation
{
    /// <summary>
    /// 定义Minecraft游戏安装器的基础接口
    /// </summary>
    public interface IInstaller
    {
        /// <summary>
        /// 获取安装器名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取游戏版本
        /// </summary>
        string GameVersion { get; }

        /// <summary>
        /// 获取安装进度
        /// </summary>
        double Progress { get; }

        /// <summary>
        /// 验证安装环境
        /// </summary>
        /// <returns>验证结果</returns>
        Task<InstallationResult> ValidateEnvironmentAsync();

        /// <summary>
        /// 下载所需文件
        /// </summary>
        /// <returns>下载结果</returns>
        Task<InstallationResult> DownloadFilesAsync();

        /// <summary>
        /// 安装游戏
        /// </summary>
        /// <returns>安装结果</returns>
        Task<InstallationResult> InstallAsync();
    }

    /// <summary>
    /// 安装结果类
    /// </summary>
    public class InstallationResult
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
        /// 获取或设置安装路径
        /// </summary>
        public string? InstallationPath { get; set; }
    }
}