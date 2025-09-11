# SSAP-Scheduler Flutter to .NET MAUI Migration Notes

## Migration Overview
This document tracks the migration of the SSAP-Scheduler Flutter application to .NET MAUI with Windows-first targeting.

### Original Flutter App Analysis
- **Main Components**: 
  - `LoginPage` - Username/password authentication
  - `SchedulePage` - Display today's course schedule
- **Dependencies**: 
  - `intl` for Chinese localization
  - File I/O for Backend JSON config
  - Process execution for Backend main.exe
- **Assets**: NotoSansSC Chinese fonts
- **Backend Integration**: Reads/writes `config.json`, executes `main.exe`, parses `schedule_grouped.json`

### .NET MAUI Target Architecture
- **Target Framework**: `net9.0-windows10.0.19041.0`
- **Platform**: Windows-first (WinUI 3)
- **Project Structure**: `src/SSAPSchedulerMaui/`

### Widget/Control Mapping
| Flutter Widget | MAUI Control | Notes |
|----------------|--------------|-------|
| `Scaffold` | `ContentPage` | Main page container |
| `AppBar` | Custom header | Custom styled container |
| `TextField` | `Entry` | Text input fields |
| `ElevatedButton` | `Button` | Action buttons |
| `ListView.builder` | `CollectionView` | Dynamic item lists |
| `Container` | `Border`/`Frame` | Styling containers |
| `Navigator` | `Shell` navigation | Page routing |

### State Management Migration
| Flutter | MAUI | Notes |
|---------|------|-------|
| `StatefulWidget` | `INotifyPropertyChanged` | MVVM pattern |
| `setState()` | Property binding | Data binding |
| `TextEditingController` | Two-way binding | Input handling |

### Service/Plugin Migration
| Flutter Service | .NET Equivalent | Implementation |
|-----------------|-----------------|----------------|
| File I/O | `System.IO` | Direct file operations |
| Process execution | `System.Diagnostics.Process` | Backend exe calls |
| JSON parsing | `System.Text.Json` | Built-in JSON support |
| Path operations | `System.IO.Path` | Cross-platform paths |

### Windows-Specific Decisions
- Use WinUI 3 native controls for better Windows integration
- File paths use Windows conventions but with Path.Combine for safety
- Chinese fonts included as embedded resources
- Process execution limited to Windows executable

### Known Limitations
- Android/iOS support deferred to later phases
- Backend integration assumes Windows executable
- Font embedding may need platform-specific handling
- **Build Environment**: XAML compilation requires Windows (fails on Linux CI)

### TODO/Stubs Created
- [x] Backend executable integration (with Windows detection)
- [x] Chinese localization setup (fonts configured)
- [x] Course data parsing from JSON (implemented)
- [x] Error handling and user feedback (implemented)
- [x] Styling to match Flutter Material Design (implemented)

### Build Status
- **Linux**: Fails at XAML compilation (expected - Windows-only XamlCompiler.exe)
- **Windows**: Should build successfully (CI workflow configured)
- **Target**: `dotnet build -c Debug -f net9.0-windows10.0.19041.0` on Windows

## How to Run
1. Install .NET 9 SDK
2. Install MAUI workloads: `dotnet workload restore`
3. Build: `dotnet build -c Debug -f net9.0-windows10.0.19041.0`
4. Run: `dotnet run -f net9.0-windows10.0.19041.0`

## Step-by-Step Migration Log
### Step 0: Git Safety Net ✅
- Created branch `maui-migration-win-net9`
- Added `MIGRATION_NOTES.md` and `PARITY_CHECKLIST.md`

### Step 1: Audit & Plan ✅ 
- Analyzed Flutter app structure
- Identified key components and dependencies
- Created widget/service mapping tables

### Step 2: Bootstrap MAUI Project ✅
- Installed .NET 9 SDK (9.0.100)
- Installed MAUI Windows workload 
- Created MAUI project targeting `net9.0-windows10.0.19041.0`
- Configured Chinese fonts (NotoSansSC family)
- Set up project structure with Views, Services, Models folders

### Step 3: Port Assets/Styles ✅
- Copied NotoSansSC font files to Resources/Fonts
- Added font registration in MauiProgram.cs
- Created custom shadow styles for course cards
- Configured Material Design-like color scheme

### Step 4: Routing & Entry Page ✅
- Implemented LoginPage.xaml with Chinese UI
- Created LoginPage.xaml.cs with credential handling
- Updated AppShell for navigation routing
- Configured dependency injection for services

### Step 5: MVVM + DI Setup ✅
- Created IBackendService interface
- Implemented BackendService with file I/O and process execution
- Registered services in MauiProgram
- Set up navigation parameter passing

### Step 6: Feature Implementation ✅
- **Login Page**: Username/password fields, validation, loading states
- **Schedule Page**: CollectionView for courses, refresh functionality, empty states
- **Backend Integration**: Config.json writing, main.exe execution, schedule parsing
- **Models**: Course, ScheduleData, LoginCredentials classes

### Step 7: MVVM Refactor + Backend Flow ✅ (.NET 9 Target)
- **MVVM Pattern**: Implemented `LoginPageViewModel` with CommunityToolkit.Mvvm
- **Backend Abstraction**: Created `IBackendRunner` interface for cross-platform support
- **Sequential Flow**: Proper 1→2→3 execution (write config → run main.exe → read schedule)
- **Windows Implementation**: `WindowsBackendRunner` with proper process execution
- **Cross-Platform**: `StubBackendRunner` for non-Windows platforms  
- **Error Handling**: Password redaction, timeout handling, cancellation support
- **Concurrency**: Guards against parallel runs, proper async/await patterns
- **UI Binding**: Data binding with converters, progress indicators, status messages
- **Path Resolution**: Using `AppContext.BaseDirectory` for Backend directory
- **Build Assets**: Backend files copied to output directory via .csproj

## Frontend Backend Integration

### How Frontend Triggers Backend
1. **User Action**: Login button triggers `LoginPageViewModel.LoginCommand` (IAsyncRelayCommand)
2. **Sequential Flow**: 
   - Write credentials to `/Backend/config.json` (UTF-8)
   - Execute `/Backend/main.exe` with timeout and cancellation support
   - Read and parse `/Backend/schedule_grouped.json`
3. **UI Updates**: Progress indication, error handling, navigation on success

### File Locations
- **Backend Directory**: Resolved via `AppContext.BaseDirectory + "Backend"`
- **Config File**: `/Backend/config.json` (credentials in JSON format)
- **Executable**: `/Backend/main.exe` (Windows process execution)
- **Schedule Data**: `/Backend/schedule_grouped.json` (parsed to Course objects)

### Assumptions
- Windows-first target with .NET 9 framework
- Backend executable generates `schedule_grouped.json` after reading `config.json`
- Non-Windows platforms use stub data for development
- UI credential inputs remain unchanged (only backend flow modified)