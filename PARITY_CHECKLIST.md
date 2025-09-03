# SSAP-Scheduler MAUI Migration Parity Checklist

## Core Functionality Parity

### Authentication & Login
- [ ] ⚠️ Username text input field
- [ ] ⚠️ Password text input field  
- [ ] ⚠️ Login button with loading state
- [ ] ⚠️ Form validation and error handling
- [ ] ⚠️ Config.json file writing
- [ ] ⚠️ Navigation to Schedule page after login

**Status**: Not implemented yet
**Notes**: Needs MAUI Entry controls and file I/O

### Schedule Display
- [ ] ⚠️ Today's course list display
- [ ] ⚠️ Course tile with emoji, name, time, location
- [ ] ⚠️ Dynamic list rendering
- [ ] ⚠️ Backend integration (main.exe execution)
- [ ] ⚠️ JSON parsing for schedule data
- [ ] ⚠️ Loading states and error handling
- [ ] ⚠️ Refresh functionality

**Status**: Not implemented yet  
**Notes**: Needs CollectionView and Backend process integration

### Backend Integration
- [ ] ⚠️ File system access for Backend directory
- [ ] ⚠️ Config.json read/write operations
- [ ] ⚠️ Process execution for main.exe
- [ ] ⚠️ Schedule_grouped.json parsing
- [ ] ⚠️ Error handling for Backend failures

**Status**: Not implemented yet
**Notes**: Use System.IO and System.Diagnostics.Process

### UI/UX Features
- [ ] ⚠️ Chinese font rendering (NotoSansSC)
- [ ] ⚠️ Material Design-like styling
- [ ] ⚠️ Color scheme matching Flutter version
- [ ] ⚠️ Responsive layout
- [ ] ⚠️ Loading indicators
- [ ] ⚠️ Error messages and snackbars

**Status**: Not implemented yet
**Notes**: Needs font embedding and XAML styling

### Localization & Internationalization  
- [ ] ⚠️ Chinese text display
- [ ] ⚠️ Date/time formatting for Chinese locale
- [ ] ⚠️ RTL/LTR text handling
- [ ] ⚠️ Locale-specific number formatting

**Status**: Not implemented yet
**Notes**: Needs resource files and CultureInfo setup

## Windows-Specific Features

### Platform Integration
- [ ] ⚠️ Windows file path handling
- [ ] ⚠️ Windows process execution
- [ ] ⚠️ Windows native controls (WinUI 3)
- [ ] ⚠️ Windows app packaging (MSIX)

**Status**: Not implemented yet
**Notes**: Windows-first approach

### Performance & Compatibility
- [ ] ⚠️ Fast startup time
- [ ] ⚠️ Memory efficiency
- [ ] ⚠️ Windows 10/11 compatibility
- [ ] ⚠️ Proper window sizing and state

**Status**: Not implemented yet

## Build & Deployment

### Development Build
- [ ] ⚠️ `dotnet build` succeeds for Windows target
- [ ] ⚠️ `dotnet run` launches app window
- [ ] ⚠️ No runtime exceptions on startup
- [ ] ⚠️ Basic navigation works

**Status**: Not started
**Target**: Debug build for net9.0-windows10.0.19041.0

### CI/CD Integration
- [ ] ⚠️ GitHub Actions workflow for Windows
- [ ] ⚠️ Automated build verification
- [ ] ⚠️ Unit test execution
- [ ] ⚠️ MSIX packaging pipeline

**Status**: Not implemented yet

## Testing & Quality Assurance

### Manual Testing
- [ ] ⚠️ Login flow end-to-end
- [ ] ⚠️ Schedule display with sample data
- [ ] ⚠️ Backend integration with test executable
- [ ] ⚠️ Error scenarios handling
- [ ] ⚠️ UI responsiveness and styling

**Status**: Not started

### Automated Testing
- [ ] ⚠️ Unit tests for view models
- [ ] ⚠️ Integration tests for Backend calls
- [ ] ⚠️ UI tests for critical user flows

**Status**: Not planned for initial migration

## Migration Progress Legend
- ✅ **Completed**: Feature fully implemented and tested
- 🔄 **In Progress**: Currently being worked on  
- ⚠️ **Pending**: Not yet started but planned
- ❌ **Blocked**: Cannot proceed due to dependencies
- 🚫 **Deferred**: Moved to later phase

## Overall Progress: 0% Complete
**Next Steps**: Bootstrap .NET 9 MAUI project and begin Login page implementation