@echo off
echo SSAP-MAUI 构建脚本
echo ===================

echo.
echo 检查 .NET 8 安装...
dotnet --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo 错误: 未找到 .NET 8 SDK
    echo 请从 https://dotnet.microsoft.com/download 安装 .NET 8 SDK
    pause
    exit /b 1
)

echo .NET SDK 版本:
dotnet --version

echo.
echo 检查 MAUI 工作负载...
dotnet workload list | findstr /i "maui" >nul
if %ERRORLEVEL% neq 0 (
    echo 警告: 未检测到 MAUI 工作负载
    echo 正在尝试安装 MAUI 工作负载...
    dotnet workload install maui
    if %ERRORLEVEL% neq 0 (
        echo 错误: MAUI 工作负载安装失败
        echo 请手动运行: dotnet workload install maui
        pause
        exit /b 1
    )
)

echo.
echo 还原项目依赖...
dotnet restore
if %ERRORLEVEL% neq 0 (
    echo 错误: 依赖还原失败
    pause
    exit /b 1
)

echo.
echo 构建项目...
dotnet build -f net8.0-windows10.0.19041.0 -c Release
if %ERRORLEVEL% neq 0 (
    echo 错误: 项目构建失败
    pause
    exit /b 1
)

echo.
echo 构建成功! 
echo.
echo 要运行应用程序，请执行:
echo dotnet run -f net8.0-windows10.0.19041.0
echo.
echo 或在 Visual Studio 中打开 SSAP-MAUI.csproj 并按 F5 运行
echo.
pause