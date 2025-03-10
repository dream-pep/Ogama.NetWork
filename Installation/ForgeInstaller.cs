using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ogama.NetWork.Installation
{
    /// <summary>
    /// Forge安装器实现
    /// </summary>
    public class ForgeInstaller : IInstaller
    {
        private readonly string _gameVersion;
        private readonly string _forgeVersion;
        private readonly string _installPath;
        private readonly HttpClient _httpClient;
        private double _progress;

        /// <summary>
        /// 初始化Forge安装器
        /// </summary>
        /// <param name="gameVersion">游戏版本</param>
        /// <param name="forgeVersion">Forge版本</param>
        /// <param name="installPath">安装路径</param>
        public ForgeInstaller(string gameVersion, string forgeVersion, string installPath)
        {
            _gameVersion = gameVersion;
            _forgeVersion = forgeVersion;
            _installPath = installPath;
            _httpClient = new HttpClient();
            _progress = 0;
        }

        /// <summary>
        /// 获取安装器名称
        /// </summary>
        public string Name => "Forge";

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
                // 构建Forge下载URL
                string forgeUrl = $"https://maven.minecraftforge.net/net/minecraftforge/forge/{_gameVersion}-{_forgeVersion}/forge-{_gameVersion}-{_forgeVersion}-installer.jar";
                string installerPath = Path.Combine(_installPath, $"forge-{_gameVersion}-{_forgeVersion}-installer.jar");

                // 下载Forge安装器
                using (var response = await _httpClient.GetAsync(forgeUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    var totalBytes = response.Content.Headers.ContentLength ?? 0;
                    var readBytes = 0L;

                    using (var fileStream = File.Create(installerPath))
                    using (var downloadStream = await response.Content.ReadAsStreamAsync())
                    {
                        var buffer = new byte[8192];
                        int bytesRead;

                        while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            readBytes += bytesRead;
                            _progress = (double)readBytes / totalBytes * 100;
                        }
                    }
                }

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

                // 执行Forge安装器
                var javaPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Java", "bin", "java.exe");
                var installerPath = downloadResult.InstallationPath;

                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = javaPath,
                        Arguments = $"-jar {installerPath} --installClient",
                        WorkingDirectory = _installPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    return new InstallationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Forge安装失败，退出代码：{process.ExitCode}"
                    };
                }

                // 清理安装文件
                if (installerPath != null)
                {
                    File.Delete(installerPath);
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