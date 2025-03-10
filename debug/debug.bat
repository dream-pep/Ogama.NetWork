@echo off
setlocal enabledelayedexpansion

:: 设置控制台标题
title Ogama.NET Debug Console

:: 显示ASCII艺术图标
echo.
echo    ____                                   _   _ ______ _______ 
echo   / __ \                                 | \ | |  ____|__   __|
echo  | |  | | __ _  __ _ _ __ ___   __ _    |  \| | |__     | |   
echo  | |  | |/ _` |/ _` | '_ ` _ \ / _` |   | . ` |  __|    | |   
echo  | |__| | (_| | (_| | | | | | | (_| |   | |\  | |____   | |   
echo   \____/ \__, |\__,_|_| |_| |_|\__,_|   |_| \_|______|  |_|   
echo           __/ |                                                 
echo          |___/                                                  
echo.
echo Ogama.NET Debug Console v1.0
echo.

:menu
echo 可用命令：
echo 1. check-env   - 检查运行环境
echo 2. test-net    - 测试网络连接
echo 3. view-log    - 查看日志
echo 4. list-inst   - 列出已安装内容
echo 5. exit        - 退出调试控制台
echo.

set /p choice=请输入命令: 

:: 使用独立线程执行命令并进行错误处理
start /b cmd /c (
if "%choice%"=="check-env" (
    call :checkEnvironment
) else if "%choice%"=="test-net" (
    call :testNetwork
) else if "%choice%"=="view-log" (
    call :viewLog
) else if "%choice%"=="list-inst" (
    call :listInstallations
) else if "%choice%"=="exit" (
    exit
) else (
    echo 无效的命令
)
)

goto menu

:checkEnvironment
echo 检查运行环境...
if exist "%ProgramFiles%\Java\bin\java.exe" (
    echo [√] Java 已安装
) else (
    echo [×] Java 未安装
)
echo 检查完成
exit /b 0

:testNetwork
echo 测试网络连接...
ping -n 2 maven.minecraftforge.net >nul
if errorlevel 1 (
    echo [×] 无法连接到 Forge 服务器
) else (
    echo [√] Forge 服务器连接正常
)
ping -n 2 fabricmc.net >nul
if errorlevel 1 (
    echo [×] 无法连接到 Fabric 服务器
) else (
    echo [√] Fabric 服务器连接正常
)
exit /b 0

:viewLog
if exist "..\bin\Debug\net8.0\logs\latest.log" (
    type "..\bin\Debug\net8.0\logs\latest.log"
) else (
    echo 未找到日志文件
)
exit /b 0

:listInstallations
echo 已安装的内容：
if exist "..\bin\Debug\net8.0\installations" (
    dir /b "..\bin\Debug\net8.0\installations"
) else (
    echo 未找到安装目录
)
exit /b 0