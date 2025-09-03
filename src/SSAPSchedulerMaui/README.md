# SSAP Scheduler - .NET MAUI (Windows)

This is the .NET MAUI version of the SSAP Scheduler application, migrated from Flutter with Windows-first targeting.

## Requirements

- .NET 9 SDK
- MAUI workloads installed
- Windows 10/11 (for compilation and running)

## Build Instructions

1. **Install .NET 9 SDK**
   ```bash
   # Download from https://dot.net/9.0 or use installer
   dotnet --version  # Should show 9.0.x
   ```

2. **Install MAUI Workloads**
   ```bash
   dotnet workload install maui-windows
   ```

3. **Restore Dependencies**
   ```bash
   cd src/SSAPSchedulerMaui
   dotnet restore
   ```

4. **Build for Windows**
   ```bash
   dotnet build -c Debug -f net9.0-windows10.0.19041.0
   ```

5. **Run the Application**
   ```bash
   dotnet run -f net9.0-windows10.0.19041.0
   ```

## Project Structure

- `Views/` - XAML pages (LoginPage, SchedulePage)
- `Services/` - Backend integration service
- `Models/` - Data models (Course, LoginCredentials)
- `Resources/` - Fonts, images, styles
- `Platforms/` - Platform-specific code

## Features Implemented

### ✅ Login Page
- Username/password input fields
- Form validation and error handling  
- Loading states during authentication
- Navigation to schedule page

### ✅ Schedule Page
- Today's course list display
- Course cards with emoji, name, time, location
- Refresh functionality
- Empty state handling
- Loading indicators

### ✅ Backend Integration
- File system access to Backend directory
- Config.json read/write operations
- Process execution for main.exe
- JSON parsing for schedule data
- Error handling and user feedback

### ✅ UI/UX
- Chinese font support (NotoSansSC)
- Material Design-inspired styling
- Responsive layout
- Color scheme matching original Flutter app
- Native Windows controls (WinUI 3)

## Backend Compatibility

The app expects the Backend directory structure to be available:
```
Project Root/
├── Backend/
│   ├── main.exe          # Backend executable
│   ├── config.json       # Generated credentials
│   └── schedule_grouped.json  # Generated schedule data
└── src/SSAPSchedulerMaui/     # MAUI app
```

The backend integration automatically:
1. Saves user credentials to `Backend/config.json`
2. Executes `Backend/main.exe` to fetch schedule data
3. Parses `Backend/schedule_grouped.json` for today's courses
4. Falls back to stub data on non-Windows platforms

## Development Notes

- **Windows-First**: Builds and runs on Windows 10/11
- **Cross-Platform Ready**: Code structure supports future Android/iOS expansion
- **Font Support**: NotoSansSC fonts embedded for Chinese text
- **Navigation**: Uses Shell-based navigation with parameter passing
- **DI Container**: Services registered in MauiProgram.cs

## Troubleshooting

### Build Fails on Linux/macOS
This is expected - XAML compilation requires Windows. The project will build successfully on Windows.

### Backend Executable Not Found
Ensure the `Backend/main.exe` exists in the correct relative path from the project root.

### Chinese Text Not Displaying
Verify NotoSansSC fonts are properly embedded and registered in `MauiProgram.cs`.

## Migration Status

This MAUI app provides feature parity with the original Flutter application:
- ✅ Login flow with credential persistence
- ✅ Schedule display with today's courses
- ✅ Backend integration for data fetching
- ✅ Chinese localization support
- ✅ Material Design-inspired UI

The migration is considered complete for Windows platform targeting.