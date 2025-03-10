using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace Ogama.NetWork.Installation
{
    /// <summary>
    /// Fabric安装器实现
    /// </summary>
    public class FabricInstaller : IInstaller
    {
        private readonly string _gameVersion;
        private readonly string _fabricVersion;
        private readonly string _installPath;
        private readonly HttpClient _httpClient;
        private double _progress;

        /// <summary>
        /// 初始化Fabric安装器
        /// </summary>
        /// <param name="gameVersion">游戏版本</param>
        /// <param name="fabricVersion">Fabric版本</param>
        /// <param name="installPath">安装路径</param>
        public FabricInstaller(string gameVersion, string fabricVersion, string installPath)
        {
            _gameVersion = gameVersion;
            _fabricVersion = fabricVersion;
            _installPath = installPath;
            _httpClient = new HttpClient();
            _progress = 0;
        }

        /// <summary>
        /// 获取安装器名称
        /// </summary>
        public string Name => "Fabric";

        /// <summary>
        /// 获取游戏版本
        /// </summary>
        public string GameVersion => _gameVersion;

        /// <summary>
        /// 获取安装进度
        /// </summary>
        public double Progress => _progress;

        /// <summary>
        /// 验证安装环境
        /// </summary>
        /// <returns>验证结果</returns>
        public async Task<InstallationResult> ValidateEnvironmentAsync()
        {
            try
            {
                // 验证Java环境
                if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Java", "bin", "java.exe")))
                {
                    return new InstallationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "未找到Java运行环境"
                    };
                }

                // 验证安装目录
                if (!Directory.Exists(_installPath))
                {
                    Directory.CreateDirectory(_installPath);
                }

                return new InstallationResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new InstallationResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"验证环境失败：{ex.Message}"
                };
            }
        }

        /// <summary>
        /// 下载所需文件
        /// </summary>
        /// <returns>下载结果</returns>
        public async Task<InstallationResult> DownloadFilesAsync()
        {
            try
            {
                // 构建Fabric下载URL
                string fabricUrl = $"https://meta.fabricmc.net/v2/versions/loader/{_gameVersion}/{_fabricVersion}/profile/json";
                string installerPath = Path.Combine(_installPath, "fabric-installer.json");

                // 下载Fabric安装配置
                var response = await _httpClient.GetAsync(fabricUrl);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                // 保存安装配置
                await File.WriteAllTextAsync(installerPath, content);

                return new InstallationResult
                {
                    IsSuccess = true,
                    InstallationPath = installerPath
                };
            }
            catch (Exception ex)
            {
                return new InstallationResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"下载文件失败：{ex.Message}"
                };
            }
        }

        /// <summary>
        /// 安装游戏
        /// </summary>
        /// <returns>安装结果</returns>
        public async Task<InstallationResult> InstallAsync()
        {
            try
            {
                var downloadResult = await DownloadFilesAsync();
                if (!downloadResult.IsSuccess)
                {
                    return downloadResult;
                }

                // 读取安装配置
                var installerConfig = await File.ReadAllTextAsync(downloadResult.InstallationPath!);
                var profileJson = JsonDocument.Parse(installerConfig);

                // 创建版本目录
                var versionDir = Path.Combine(_installPath, "versions", $"{_gameVersion}-fabric-{_fabricVersion}");
                Directory.CreateDirectory(versionDir);

                // 保存版本配置
                await File.WriteAllTextAsync(
                    Path.Combine(versionDir, $"{_gameVersion}-fabric-{_fabricVersion}.json"),
                    installerConfig
                );

                // 清理安装文件
                File.Delete(downloadResult.InstallationPath!);

                return new InstallationResult
                {
                    IsSuccess = true,
                    InstallationPath = versionDir
                };
            }
            catch (Exception ex)
            {
                return new InstallationResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"安装失败：{ex.Message}"
                };
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
    }
}