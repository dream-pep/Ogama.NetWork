using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Ogama.NetWork.Authentication
{
    /// <summary>
    /// 外置账户验证器实现
    /// </summary>
    public class ExternalAuthenticator : IAuthenticator, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _serverUrl;
        private readonly string _username;
        private readonly string _password;
        private string? _accessToken;
        private string? _uuid;

        /// <summary>
        /// 初始化外置账户验证器
        /// </summary>
        /// <param name="serverUrl">验证服务器URL</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public ExternalAuthenticator(string serverUrl, string username, string password)
        {
            _serverUrl = serverUrl.TrimEnd('/');
            _username = username;
            _password = password;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 验证外置账户
        /// </summary>
        /// <returns>验证结果</returns>
        public async Task<AuthenticationResult> AuthenticateAsync()
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_serverUrl}/authserver/authenticate", new
                {
                    username = _username,
                    password = _password
                });

                if (!response.IsSuccessStatusCode)
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"验证失败：{response.StatusCode}"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result == null)
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "无法解析服务器响应"
                    };
                }

                _accessToken = result.AccessToken;
                _uuid = result.Uuid;

                return new AuthenticationResult
                {
                    IsSuccess = true,
                    Username = _username,
                    AccessToken = _accessToken,
                    Uuid = _uuid
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"验证过程出错：{ex.Message}"
                };
            }
        }

        /// <summary>
        /// 刷新外置账户凭据
        /// </summary>
        /// <returns>刷新结果</returns>
        public async Task<AuthenticationResult> RefreshAsync()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "没有可用的访问令牌"
                };
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_serverUrl}/authserver/refresh", new
                {
                    accessToken = _accessToken
                });

                if (!response.IsSuccessStatusCode)
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"刷新失败：{response.StatusCode}"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result == null)
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "无法解析服务器响应"
                    };
                }

                _accessToken = result.AccessToken;

                return new AuthenticationResult
                {
                    IsSuccess = true,
                    Username = _username,
                    AccessToken = _accessToken,
                    Uuid = _uuid
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"刷新过程出错：{ex.Message}"
                };
            }
        }

        /// <summary>
        /// 验证外置账户凭据是否有效
        /// </summary>
        /// <returns>是否有效</returns>
        public async Task<bool> ValidateAsync()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                return false;
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_serverUrl}/authserver/validate", new
                {
                    accessToken = _accessToken
                });

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
        }

        private class AuthResponse
        {
            public string? AccessToken { get; set; }
            public string? Uuid { get; set; }
        }
    }
}