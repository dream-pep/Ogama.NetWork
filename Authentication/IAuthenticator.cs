using System.Threading.Tasks;

namespace Ogama.NetWork.Authentication
{
    /// <summary>
    /// 定义Minecraft账户验证器的基础接口
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// 验证账户凭据
        /// </summary>
        /// <returns>验证结果</returns>
        Task<AuthenticationResult> AuthenticateAsync();

        /// <summary>
        /// 刷新账户凭据
        /// </summary>
        /// <returns>刷新结果</returns>
        Task<AuthenticationResult> RefreshAsync();

        /// <summary>
        /// 验证账户凭据是否有效
        /// </summary>
        /// <returns>是否有效</returns>
        Task<bool> ValidateAsync();
    }

    /// <summary>
    /// 验证结果类
    /// </summary>
    public class AuthenticationResult
    {
        /// <summary>
        /// 获取或设置是否验证成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 获取或设置访问令牌
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// 获取或设置用户名
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// 获取或设置UUID
        /// </summary>
        public string? Uuid { get; set; }

        /// <summary>
        /// 获取或设置错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}