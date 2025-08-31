# SSAP Scheduler - Windows MAUI App

## Overview
This is a .NET MAUI application designed specifically for Windows to manage and display course schedules. The app features a login page and a schedule page, allowing users to authenticate and view their courses for the day.

**Note: This project has been configured for Windows-only deployment to ensure compatibility with VS 2022.**

## Project Structure
- **Platforms**: Contains Windows-specific implementation only.
- **Resources**: Includes application resources such as AppIcon, Splash screen, and Styles.
- **Models**: Contains the `Course.cs` file, which defines the Course model with properties like Name, Start, End, Room, Teacher, and Emoji.
- **Services**: Contains the `ScheduleService.cs` file, which handles fetching and processing schedule data from the backend.
- **ViewModels**: Includes `LoginViewModel.cs` for managing login logic and `ScheduleViewModel.cs` for managing schedule data using MVVM pattern.
- **Views**: Contains XAML files for the UI layout, including `LoginPage.xaml` and `SchedulePage.xaml`, along with their respective code-behind files.
- **Converters**: Contains value converters like `InvertedBoolConverter.cs` for UI data binding.

## Features
- User authentication through a login page with validation.
- Display of today's courses on the schedule page with real-time loading.
- Integration with Python/executable backend for fetching course data.
- Modern UI design with Chinese language support.
- MVVM architecture using CommunityToolkit.Mvvm.

## System Requirements
- Windows 10 version 1903 or higher
- Visual Studio 2022 with .NET MAUI workload installed
- .NET 8.0 or higher

## Getting Started
1. Clone the repository.
2. Open `my-maui-app.csproj` in Visual Studio 2022.
3. Ensure you have the .NET MAUI workload installed.
4. Restore NuGet packages.
5. Build and run the project (Windows target).

## Configuration
The app expects a backend directory with either:
- `main.py` (Python script)
- `main.exe` (compiled executable)

The backend should process `config.json` and generate `schedule_grouped.json`.

## Contributing
Contributions are welcome! Please submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for details.