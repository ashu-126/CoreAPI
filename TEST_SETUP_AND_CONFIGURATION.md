# Test Setup and Configuration Guide

## Overview

Complete guide for setting up and running test cases for the CoreAPI project.

---

## 📋 Prerequisites

### Required Tools
- ✅ .NET 10 SDK
- ✅ Visual Studio 2026 or Visual Studio Code
- ✅ Git
- ✅ PowerShell or Command Prompt

### Verify Installation
```powershell
dotnet --version  # Should show 10.x.x
dotnet test --version
```

---

## 🚀 Quick Start

### 1. Build the Test Project
```powershell
# Navigate to solution root
cd C:\Users\asprajapati\source\repos\CoreAPI

# Build solution
dotnet build

# Build test project specifically
dotnet build CoreAPI.Tests\CoreAPI.Tests.csproj
```

### 2. Run All Tests
```powershell
# Run all tests with summary
dotnet test

# Run with verbose output
dotnet test --verbosity detailed

# Run with no build (faster)
dotnet test --no-build
```

### 3. Run Specific Test Categories
```powershell
# Run only unit tests
dotnet test --filter "FullyQualifiedName~HomeControllerTests"

# Run only authorization tests
dotnet test --filter "FullyQualifiedName~AuthorizationAttributeTests"

# Run only response tests
dotnet test --filter "FullyQualifiedName~ResponseValidationTests"

# Run tests with specific trait
dotnet test --filter "Category=Authorization"
dotnet test --filter "Category=Response"
dotnet test --filter "Endpoint=AdminOnly"
```

---

## 📊 Test Project Structure

```
CoreAPI.Tests/
├── bin/                          # Build output
├── obj/                          # Build intermediate
├── Controllers/
│   └── HomeControllerTests.cs    # 20+ unit tests
├── Authorization/
│   └── AuthorizationAttributeTests.cs  # 10+ tests
├── Integration/
│   └── HomeControllerIntegrationTests.cs # 12+ tests
├── Response/
│   └── ResponseValidationTests.cs # 15+ tests
└── CoreAPI.Tests.csproj          # Test project file
```

---

## 🧪 Test Categories

### Unit Tests
**File:** `HomeControllerTests.cs`
```
Purpose: Test controller methods directly
Type: Fast, isolated tests
Coverage: ~95%
Count: 20+
```

**Run Command:**
```powershell
dotnet test --filter "FullyQualifiedName~HomeControllerTests"
```

### Authorization Tests
**File:** `AuthorizationAttributeTests.cs`
```
Purpose: Verify authorization attributes
Type: Reflection-based tests
Coverage: ~100%
Count: 10+
```

**Run Command:**
```powershell
dotnet test --filter "FullyQualifiedName~AuthorizationAttributeTests"
```

### Integration Tests
**File:** `HomeControllerIntegrationTests.cs`
```
Purpose: Test full HTTP request/response cycle
Type: Integration tests (currently skipped)
Coverage: ~80%
Count: 12+
Note: Currently marked as "Skip" - requires running server
```

**Run Command (with server running):**
```powershell
# Remove Skip attribute and run
dotnet test --filter "FullyQualifiedName~HomeControllerIntegrationTests"
```

### Response Validation Tests
**File:** `ResponseValidationTests.cs`
```
Purpose: Validate response structure and content
Type: Assertion-based tests
Coverage: ~90%
Count: 15+
```

**Run Command:**
```powershell
dotnet test --filter "FullyQualifiedName~ResponseValidationTests"
```

---

## 💾 Test Execution Options

### Basic Test Run
```powershell
dotnet test
```
Output:
```
Starting test execution, please wait...
Total tests: 57
Passed: 57
Failed: 0
```

### Detailed Output
```powershell
dotnet test --verbosity detailed
```

### Very Detailed Output
```powershell
dotnet test --verbosity diagnostic
```

### Single Test Execution
```powershell
# By fully qualified name
dotnet test --filter "FullyQualifiedName=CoreAPI.Tests.Controllers.HomeControllerTests.AdminEndpoint_WhenCalled_ReturnsOkResult"

# By trait
dotnet test --filter "Category=Authorization&Endpoint=AdminOnly"
```

---

## 📊 Code Coverage

### Install Coverage Tools
```powershell
# Install ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Or update if already installed
dotnet tool update -g dotnet-reportgenerator-globaltool
```

### Generate Coverage Report
```powershell
# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura /p:ExcludeByAttribute=GeneratedCode

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:coverage -reporttypes:Html

# Open report
start coverage\index.html  # Windows
open coverage/index.html   # macOS
```

### Coverage Goals
- **Overall:** 90%+
- **Critical paths:** 95%+
- **Controllers:** 95%+
- **Services:** 90%+

---

## 🔧 Visual Studio Integration

### In Visual Studio 2026

1. **Open Test Explorer**
   - Menu: Test → Test Explorer
   - Keyboard: Ctrl + E, T

2. **Run Tests**
   - Click "Run All Tests"
   - Or run specific test by clicking play button

3. **Filter Tests**
   - Use search box in Test Explorer
   - Type test name or trait

4. **View Results**
   - Double-click test to see details
   - Check output window for logs

5. **Code Coverage**
   - Right-click test → Analyze Code Coverage
   - View coverage percentage

---

## 🏃 Continuous Integration

### GitHub Actions

Create `.github/workflows/tests.yml`:

```yaml
name: Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --configuration Release --no-restore

    - name: Run tests
      run: dotnet test --configuration Release --no-build --verbosity normal

    - name: Generate coverage
      run: |
        dotnet test /p:CollectCoverage=true \
                   /p:CoverageFormat=cobertura \
                   /p:ExcludeByAttribute=GeneratedCode

    - name: Upload coverage
      uses: codecov/codecov-action@v3
      with:
        files: ./coverage.cobertura.xml
```

---

## 📈 Performance Benchmarking

### Run Tests with Timing
```powershell
dotnet test --logger "console;verbosity=detailed"
```

### Expected Performance
| Test Type | Avg Time | Count | Total |
|-----------|----------|-------|-------|
| Unit Tests | ~10ms | 20 | ~200ms |
| Authorization Tests | ~5ms | 10 | ~50ms |
| Response Tests | ~8ms | 15 | ~120ms |
| Integration | ~500ms | 12 | ~6000ms* |
| **Total** | - | ~57 | **~370ms** |

*Integration tests skipped by default

---

## 🐛 Debugging Tests

### In Visual Studio

1. **Set Breakpoint**
   - Click line number in test

2. **Run Test in Debug Mode**
   - Right-click test → Debug

3. **Inspect Values**
   - Hover over variables
   - Use Watch window

### Using Command Line

```powershell
# Run single test with debug output
dotnet test --filter "AdminEndpoint_WhenCalled_ReturnsOkResult" --verbosity diagnostic
```

---

## ✅ Test Verification Checklist

### Before Committing Code

- [ ] Run all tests: `dotnet test`
- [ ] Check code coverage: `>90%`
- [ ] Run SonarQube analysis
- [ ] No test failures
- [ ] No warnings in output

### Before Pushing to Repository

- [ ] All tests passing locally
- [ ] Code coverage acceptable
- [ ] No breaking changes
- [ ] Documentation updated

---

## 🔍 Common Issues & Solutions

### Issue: Tests Not Found
**Solution:**
```powershell
# Rebuild solution
dotnet clean
dotnet build

# Run tests
dotnet test
```

### Issue: Connection Timeout (Integration Tests)
**Solution:**
- Ensure API is running on correct port
- Check firewall settings
- Verify connection string

### Issue: Test Timeout
**Solution:**
```powershell
# Increase timeout
dotnet test --logger "console;verbosity=detailed" -- RunConfiguration.TargetPlatform=x64
```

### Issue: Coverage Not Generated
**Solution:**
```powershell
# Install package
dotnet add package coverlet.collector

# Run with explicit settings
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura /p:UseSourceLink=true
```

---

## 📝 Test Traits Reference

### Available Traits

```csharp
[Trait("Category", "Authorization")]  // Authorization tests
[Trait("Category", "Response")]       // Response validation
[Trait("Category", "HTTP")]           // HTTP method tests
[Trait("Category", "Validation")]     // Input validation
[Trait("Category", "Integration")]    // Integration tests

[Trait("Endpoint", "AdminOnly")]      // Specific endpoint
[Trait("Endpoint", "ManagerAndAdmin")]
[Trait("Endpoint", "GetProducts")]
[Trait("Endpoint", "CreateProduct")]
[Trait("Endpoint", "DeleteProduct")]

[Trait("Permission", "products.read")]
[Trait("Permission", "products.write")]
[Trait("Permission", "products.delete")]

[Trait("Security", "Authorization")]  // Security tests
```

### Filter Examples

```powershell
# All authorization tests
dotnet test --filter "Category=Authorization"

# Admin endpoint tests
dotnet test --filter "Endpoint=AdminOnly"

# Products permission tests
dotnet test --filter "Permission=products.read"

# Multiple filters (AND)
dotnet test --filter "Category=Response&Endpoint=GetProducts"

# Not filter
dotnet test --filter "FullyQualifiedName!~Integration"
```

---

## 🎯 Next Steps

1. **Run Tests**
   ```powershell
   dotnet test
   ```

2. **Check Coverage**
   ```powershell
   dotnet test /p:CollectCoverage=true
   ```

3. **Setup CI/CD**
   - Add GitHub Actions workflow
   - Configure SonarQube

4. **Monitor Quality**
   - Track test coverage trends
   - Monitor test execution time
   - Review failed tests

---

## 📚 Useful Resources

- **xUnit Documentation:** https://xunit.net/
- **FluentAssertions:** https://fluentassertions.com/
- **SonarQube Docs:** https://docs.sonarqube.org/
- **dotnet test docs:** https://docs.microsoft.com/dotnet/core/tools/dotnet-test

---

**Happy Testing! 🧪**
