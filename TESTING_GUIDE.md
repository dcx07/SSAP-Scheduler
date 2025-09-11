# SSAP-Scheduler Backend Integration Test Guide

## Manual Testing Instructions

### Prerequisites
- .NET 9 SDK installed
- Windows environment for full testing
- Backend directory with main.exe

### Test Steps

1. **Build the Application**
   ```bash
   cd src/SSAPSchedulerMaui
   dotnet restore
   dotnet build -c Debug -f net9.0-windows10.0.19041.0
   ```

2. **Test Backend Flow Manually**
   ```bash
   cd Backend
   # Create test credentials
   echo '{"Username":"testuser","Password":"testpass"}' > config.json
   
   # Run backend (use provided main.exe or mock)
   ./main.exe
   
   # Verify schedule output
   cat schedule_grouped.json
   ```

3. **Test Application Flow**
   - Launch the MAUI application
   - Enter username and password in login form
   - Click "登录" button
   - Observe progress indicators and status messages
   - Verify navigation to schedule page with loaded courses

### Expected Behavior

1. **Login Process**:
   - Loading indicator appears
   - Status message shows "正在登录..." then "正在验证凭据..."
   - Cancel button appears during processing
   - On success: "登录成功" message and navigation
   - On failure: Error message with redacted password

2. **Backend Flow**:
   - Credentials written to `Backend/config.json` (UTF-8)
   - `Backend/main.exe` executed with timeout
   - `Backend/schedule_grouped.json` read and parsed
   - Courses displayed on schedule page

3. **Error Handling**:
   - Missing main.exe: "Backend executable (main.exe) not found"
   - Process timeout: "Backend process timed out after 30 seconds"
   - Invalid JSON: "Failed to parse schedule data"
   - Permissions: "Failed to save credentials"

### Cross-Platform Notes
- Windows: Uses `WindowsBackendRunner` with process execution
- Other platforms: Uses `StubBackendRunner` with sample data
- Both implementations follow the same `IBackendRunner` interface

### Architecture Verification
- ✅ MVVM pattern with CommunityToolkit.Mvvm
- ✅ IAsyncRelayCommand for login action
- ✅ Concurrency guards prevent parallel execution
- ✅ Cancellation token support for timeouts
- ✅ Password redaction in error messages
- ✅ AppContext.BaseDirectory for path resolution
- ✅ Backend files copied to output directory