# SSAP-MAUI - .NET MAUI 课程调度器

这是 SSAP 课程调度器的 .NET MAUI 重构版本，专为 Windows 开发环境设计。

## 系统要求

### Windows 开发机要求
- Windows 10 版本 1903 (内部版本 18362) 或更高版本
- Windows 11 (推荐)
- Visual Studio 2022 17.8 或更高版本
- .NET 8.0 SDK

### .NET MAUI 工作负载安装

在 Windows 开发机上运行以下命令安装必需的工作负载：

```bash
# 安装 .NET MAUI 工作负载
dotnet workload install maui

# 验证安装
dotnet workload list
```

## 项目结构

```
SSAP-MAUI/
├── Models/           # 数据模型 (Course, WeekSchedule, LoginConfig)
├── Services/         # 服务层 (ScheduleService - 后端集成)
├── ViewModels/       # MVVM 视图模型 (LoginViewModel, ScheduleViewModel)
├── Views/            # XAML 视图 (LoginPage, SchedulePage)
├── Resources/        # 资源文件 (字体、图标、启动画面)
├── Platforms/        # 平台特定代码
│   └── Windows/      # Windows 平台代码
└── SSAP-MAUI.csproj  # 项目文件
```

## 功能特性

### ✅ 已实现功能
- **登录界面**: 用户名和密码输入，与原 Flutter 应用样式一致
- **课程显示**: 显示今日课程安排，包含时间、地点、教师信息
- **后端集成**: 自动调用 Backend/main.exe，读取 schedule_grouped.json
- **中文支持**: 完整的中文界面和日期格式化
- **错误处理**: 完善的异常处理和用户提示
- **MVVM 架构**: 使用 CommunityToolkit.Mvvm 实现 MVVM 模式

### 🎨 UI 设计
- **配色方案**: 
  - 主色调: `#6C63FF` (蓝紫色)
  - 次要色: `#FFB870` (橙色)
  - 背景色: `#FDF5E6` (黄白色)
- **字体**: Noto Sans SC (中文优化)
- **设计风格**: 现代化卡片式设计，与原 Flutter 应用保持一致

## 构建和运行

### 在 Windows 开发机上构建

1. **克隆项目**:
```bash
git clone <repository-url>
cd SSAP-Scheduler
```

2. **还原包依赖**:
```bash
cd SSAP-MAUI
dotnet restore
```

3. **构建项目**:
```bash
dotnet build -f net8.0-windows10.0.19041.0
```

4. **运行项目**:
```bash
dotnet run -f net8.0-windows10.0.19041.0
```

### Visual Studio 中运行

1. 使用 Visual Studio 2022 打开 `SSAP-MAUI.csproj`
2. 确保选择 Windows Machine 作为启动目标
3. 按 F5 或点击"启动"运行

## 后端集成

应用程序会自动：

1. **配置保存**: 将用户名和密码保存到 `Backend/config.json`
2. **执行后端**: 运行 `Backend/main.exe` 获取课程数据
3. **数据解析**: 读取 `Backend/schedule_grouped.json` 并解析今日课程
4. **显示课程**: 在界面上显示课程列表

### JSON 数据格式支持

支持后端生成的标准格式：
```json
{
  "Mon": {
    "date": "2024-01-08", 
    "courses": [
      {
        "name": "数据结构与算法",
        "room": "教学楼A101",
        "start": "08:00",
        "end": "09:40", 
        "teacher": "张教授",
        "emoji": "📚"
      }
    ]
  }
}
```

**注意**: `emoji` 字段为可选，如果不存在会使用默认的 📚 图标。

## 技术栈

- **.NET 8.0**: LTS 版本，提供最佳稳定性
- **.NET MAUI**: 跨平台 UI 框架 (当前配置仅支持 Windows)
- **CommunityToolkit.Mvvm**: MVVM 框架
- **System.Text.Json**: JSON 序列化/反序列化
- **Microsoft.Extensions.Logging**: 日志记录

## 故障排除

### 常见问题

1. **MAUI 工作负载未安装**:
```bash
dotnet workload install maui
```

2. **构建失败 - 缺少 Windows SDK**:
   - 确保安装了 Windows 10 SDK (版本 10.0.17763.0 或更高)
   - 通过 Visual Studio Installer 安装

3. **后端 exe 文件未找到**:
   - 确保 `Backend/main.exe` 存在
   - 检查工作目录是否正确设置

4. **字体显示问题**:
   - 确保 `Resources/Fonts/NotoSansSC-Regular.ttf` 文件存在
   - 重新构建项目

### 调试模式

启用详细日志记录：
```csharp
// 在 MauiProgram.cs 中
builder.Services.AddLogging(configure => 
{
    configure.AddDebug();
    configure.SetMinimumLevel(LogLevel.Debug);
});
```

## 与原 Flutter 应用的差异

### 相同功能
- ✅ 用户登录界面
- ✅ 今日课程显示  
- ✅ 后端 exe 调用
- ✅ JSON 数据解析
- ✅ 中文界面
- ✅ 相同的 UI 设计

### MAUI 改进
- 🚀 更好的 Windows 集成
- 🚀 MVVM 架构模式
- 🚀 强类型数据绑定
- 🚀 更好的错误处理
- 🚀 现代化的 C# 语法

## 开发指南

### 添加新功能

1. **新增模型**: 在 `Models/` 目录下创建数据模型类
2. **服务层**: 在 `Services/` 中实现业务逻辑
3. **视图模型**: 在 `ViewModels/` 中创建 MVVM 视图模型
4. **用户界面**: 在 `Views/` 中创建 XAML 视图文件

### 代码风格

- 使用 C# 命名约定
- 视图模型继承自 `ObservableObject`
- 使用 `[ObservableProperty]` 和 `[RelayCommand]` 特性
- XAML 中使用数据绑定而非代码后置

## 许可证

本项目继承原项目的许可证设置。

## 贡献

欢迎提交 Issue 和 Pull Request 来改进此项目。