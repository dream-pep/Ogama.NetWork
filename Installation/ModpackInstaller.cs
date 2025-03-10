using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ogama.NetWork.Installation
{
    /// <summary>
    /// 整合包安装器实现
    /// </summary>
    public class ModpackInstaller : IInstaller
    {
        private readonly string _modpackPath;
        private readonly string _installPath;
        private readonly string _gameVersion;
        private readonly HttpClient _httpClient;
        private double _progress;

        /// <summary>
        /// 初始化整合包安装器
        /// </summary>
        /// <param name="modpackPath">整合包文件路径</param>
        /// <param name="installPath">安装路径</param>
        /// <param name="gameVersion">游戏版本</param>
        public ModpackInstaller(string modpackPath, string installPath, string gameVersion)
        {
            _modpackPath = modpackPath;
            _installPath = installPath;
            _gameVersion = gameVersion;
            _httpClient = new HttpClient();
            _progress = 0;
        }

        /// <summary>
        /// 获取安装器名称
        /// </summary>
        public string Name => "Modpack";

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
                // 验证整合包文件
                if (!File.Exists(_modpackPath))
                {
                    return new InstallationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "未找到整合包文件"
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
                // 整合包已在本地，无需下载
                return new InstallationResult
                {
                    IsSuccess = true,
                    InstallationPath = _modpackPath
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
                var validateResult = await ValidateEnvironmentAsync();
                if (!validateResult.IsSuccess)
                {
                    return validateResult;
                }

                // 解压整合包
                using (var archive = ZipFile.OpenRead(_modpackPath))
                {
                    var totalEntries = archive.Entries.Count;
                    var processedEntries = 0;

                    foreach (var entry in archive.Entries)
                    {
                        var targetPath = Path.Combine(_installPath, entry.FullName);
                        var targetDir = Path.GetDirectoryName(targetPath);

                        if (!string.IsNullOrEmpty(targetDir))
                        {
                            Directory.CreateDirectory(targetDir);
                        }

                        if (!string.IsNullOrEmpty(entry.Name))
                        {
                            entry.ExtractToFile(targetPath, true);
                        }

                        processedEntries++;
                        _progress = (double)processedEntries / totalEntries * 100;
                    }
                }

                return new InstallationResult
                {
                    IsSuccess = true,
                    InstallationPath = _installPath
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