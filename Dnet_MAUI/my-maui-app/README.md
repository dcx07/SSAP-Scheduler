# My MAUI App

## Overview
This is a .NET MAUI application designed to manage and display course schedules. The app features a login page and a schedule page, allowing users to authenticate and view their courses for the day.

## Project Structure
- **Platforms**: Contains platform-specific implementations for Android, iOS, MacCatalyst, and Windows.
- **Resources**: Includes application resources such as AppIcon, Fonts, Images, Raw files, and Styles.
- **Models**: Contains the `Course.cs` file, which defines the Course model with properties like Name, Start, End, Room, Teacher, and Emoji.
- **Services**: Contains the `ScheduleService.cs` file, which handles fetching and processing schedule data.
- **ViewModels**: Includes `LoginViewModel.cs` for managing login logic and `ScheduleViewModel.cs` for managing schedule data.
- **Views**: Contains XAML files for the UI layout, including `LoginPage.xaml` and `SchedulePage.xaml`, along with their respective code-behind files.

## Features
- User authentication through a login page.
- Display of today's courses on the schedule page.
- Platform-specific implementations for a seamless user experience across devices.

## Getting Started
1. Clone the repository.
2. Open the project in your preferred IDE.
3. Restore the dependencies and build the project.
4. Run the application on your desired platform.

## Contributing
Contributions are welcome! Please submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for details.