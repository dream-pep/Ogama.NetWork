using System.Threading.Tasks;

namespace Ogama.NetWork.Authentication
{
    /// <summary>
    /// 离线账户验证器实现
    /// </summary>
    public class OfflineAuthenticator : IAuthenticator
    {
        private readonly string _username;
        private readonly string _uuid;
        private readonly string _accessToken;

        /// <summary>
        /// 初始化离线账户验证器
        /// </summary>
        /// <param name="username">用户名</param>
        public OfflineAuthenticator(string username)
        {
            _username = username;
            _uuid = $"OfflinePlayer:{username}";
            _accessToken = "0";
        }

        /// <summary>
        /// 验证离线账户
        /// </summary>
        /// <returns>验证结果</returns>
        public Task<AuthenticationResult> AuthenticateAsync()
        {
            return Task.FromResult(new AuthenticationResult
            {
                IsSuccess = true,
                Username = _username,
                Uuid = _uuid,
                AccessToken = _accessToken
            });
        }

        /// <summary>
        /// 刷新离线账户凭据（离线模式下始终返回成功）
        /// </summary>
        /// <returns>刷新结果</returns>
        public Task<AuthenticationResult> RefreshAsync()
        {
            return AuthenticateAsync();
        }

        /// <summary>
        /// 验证离线账户凭据是否有效（离线模式下始终返回true）
        /// </summary>
        /// <returns>是否有效</returns>
        public Task<bool> ValidateAsync()
        {
            return Task.FromResult(true);
        }
    }
}