# Test Execution Quick Reference

## 🚀 Quick Commands

### Run All Tests
```powershell
cd C:\Users\asprajapati\source\repos\CoreAPI
dotnet test
```

### Run with Verbose Output
```powershell
dotnet test --verbosity detailed
```

### Run Specific Test File
```powershell
# Unit Tests
dotnet test --filter "FullyQualifiedName~HomeControllerTests"

# Authorization Tests
dotnet test --filter "FullyQualifiedName~AuthorizationAttributeTests"

# Response Validation Tests
dotnet test --filter "FullyQualifiedName~ResponseValidationTests"

# Integration Tests
dotnet test --filter "FullyQualifiedName~HomeControllerIntegrationTests"
```

---

## 🏷️ Filter by Traits

### By Category
```powershell
# Authorization tests
dotnet test --filter "Category=Authorization"

# Response validation tests
dotnet test --filter "Category=Response"

# HTTP method tests
dotnet test --filter "Category=HTTP"

# Validation tests
dotnet test --filter "Category=Validation"

# Integration tests
dotnet test --filter "Category=Integration"
```

### By Endpoint
```powershell
# AdminOnly endpoint tests
dotnet test --filter "Endpoint=AdminOnly"

# ManagerAndAdmin endpoint tests
dotnet test --filter "Endpoint=ManagerAndAdmin"

# GetProducts endpoint tests
dotnet test --filter "Endpoint=GetProducts"

# CreateProduct endpoint tests
dotnet test --filter "Endpoint=CreateProduct"

# DeleteProduct endpoint tests
dotnet test --filter "Endpoint=DeleteProduct"
```

### By Permission
```powershell
# products.read permission tests
dotnet test --filter "Permission=products.read"

# products.write permission tests
dotnet test --filter "Permission=products.write"

# products.delete permission tests
dotnet test --filter "Permission=products.delete"
```

### By Security
```powershell
# Authorization security tests
dotnet test --filter "Security=Authorization"
```

---

## 🔍 Complex Filters

### Multiple Conditions (AND)
```powershell
# Authorization tests for AdminOnly endpoint
dotnet test --filter "Category=Authorization&Endpoint=AdminOnly"

# Response tests for GetProducts
dotnet test --filter "Category=Response&Endpoint=GetProducts"

# Permission tests for DeleteProduct
dotnet test --filter "Permission=products.delete&Endpoint=DeleteProduct"
```

### Exclude Tests (NOT)
```powershell
# All tests except integration
dotnet test --filter "FullyQualifiedName!~Integration"

# All tests except response validation
dotnet test --filter "FullyQualifiedName!~ResponseValidation"
```

---

## 📊 Code Coverage

### Generate Coverage Report
```powershell
# Collect coverage data
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:coverage -reporttypes:Html

# Open report
start coverage\index.html
```

### Coverage by File
```powershell
# With file-level coverage
dotnet test /p:CollectCoverage=true \
           /p:CoverageFormat=cobertura \
           /p:ExcludeByAttribute=GeneratedCode
```

---

## ⚡ Performance

### Run Without Build
```powershell
dotnet test --no-build
```

### Run in Release Mode
```powershell
dotnet test -c Release
```

### Run with Minimal Output
```powershell
dotnet test --verbosity quiet
```

### Run Specific Number of Tests
```powershell
# Run only first 5 tests
dotnet test --filter "Category=Authorization" | head -5
```

---

## 🔧 Development Mode

### Watch Mode (Continuous Testing)
```powershell
# Run tests continuously on file changes
dotnet watch test

# Or use third-party tool
dotnet tool install -g dotnet-script
dotnet script watch.csx
```

### Debug Single Test
```powershell
# Run in debug mode
dotnet test --filter "AdminEndpoint_WhenCalled_ReturnsOkResult" --logger console
```

### Collect Diagnostics
```powershell
dotnet test --collect:"XPlat Code Coverage" --diag:diagnostics.log
```

---

## 📋 Test Summary Reports

### Generate JUnit Report
```powershell
dotnet test --logger "junit;LogFileName=test-results.xml"
```

### Generate HTML Report
```powershell
dotnet test --logger "html;LogFileName=test-results.html"
```

### Generate Trx Report
```powershell
dotnet test --logger "trx;LogFileName=test-results.trx"
```

---

## 🎯 CI/CD Commands

### Full Pipeline
```powershell
# Build
dotnet build -c Release

# Run tests
dotnet test -c Release --no-build

# Generate coverage
dotnet test -c Release --no-build /p:CollectCoverage=true /p:CoverageFormat=cobertura

# SonarQube analysis
dotnet sonarscanner begin /k:"CoreAPI" /d:sonar.host.url="http://localhost:9000"
dotnet build
dotnet sonarscanner end
```

### GitHub Actions
```yaml
- name: Run tests
  run: dotnet test --verbosity normal
```

---

## 🐛 Troubleshooting Commands

### Clean and Rebuild
```powershell
dotnet clean
dotnet build
dotnet test
```

### Reset Cache
```powershell
Remove-Item -Recurse -Force bin
Remove-Item -Recurse -Force obj
dotnet build
dotnet test
```

### Check Test Discovery
```powershell
dotnet test --collect:"XPlat Code Coverage" --logger console
```

---

## 📊 Test Count Verification

### List All Tests
```powershell
dotnet test --verbosity quiet --filter "" | findstr "Total tests"
```

### Count by Category
```powershell
# Count authorization tests
dotnet test --filter "Category=Authorization" --verbosity quiet

# Count response tests
dotnet test --filter "Category=Response" --verbosity quiet

# Count endpoint tests
dotnet test --filter "Endpoint=AdminOnly" --verbosity quiet
```

---

## 🔄 Continuous Execution

### Run Tests on Every Save
```powershell
# Using PowerShell FSW (File System Watcher)
while ($true) {
    dotnet test --no-build
    Write-Host "Press Ctrl+C to stop"
    Start-Sleep -Seconds 5
}
```

### Run Tests in Background
```powershell
# Start background job
Start-Job -ScriptBlock { cd C:\Users\asprajapati\source\repos\CoreAPI; dotnet test }

# List jobs
Get-Job

# View output
Get-Job | Receive-Job

# Stop job
Stop-Job -Id 1
```

---

## 📈 Test Metrics

### Get Test Statistics
```powershell
# Count total tests
$output = dotnet test --verbosity quiet
$output | Select-String "Total tests"

# Count passed tests
$output | Select-String "Passed"

# Count failed tests
$output | Select-String "Failed"
```

### Measure Execution Time
```powershell
# Time full test run
Measure-Command { dotnet test }

# Time specific category
Measure-Command { dotnet test --filter "Category=Authorization" }
```

---

## 🚀 One-Liners

### Quick Test
```powershell
cd C:\Users\asprajapati\source\repos\CoreAPI && dotnet test && echo "✅ Tests Passed!"
```

### Test with Coverage
```powershell
dotnet test /p:CollectCoverage=true && reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:coverage -reporttypes:Html && start coverage\index.html
```

### Run Only Failed Tests
```powershell
dotnet test --filter "Category=Authorization" --logger console
```

### Clean and Test
```powershell
dotnet clean && dotnet build && dotnet test --verbosity detailed
```

---

## 📱 Visual Studio Commands

### Run from VS
- **View** → **Test Explorer** (Ctrl+E, T)
- Click **Run All** or right-click specific test
- **Debug** → **Debug All Tests** (Ctrl+R, A)

### Command Line from VS
```
View → Terminal
Run: dotnet test
```

---

## 🎯 Common Scenarios

### "I want to run all admin tests"
```powershell
dotnet test --filter "Endpoint=AdminOnly"
```

### "I want to run response validation only"
```powershell
dotnet test --filter "Category=Response"
```

### "I want to skip integration tests"
```powershell
dotnet test --filter "Category!=Integration"
```

### "I want to run one specific test"
```powershell
dotnet test --filter "FullyQualifiedName=CoreAPI.Tests.Controllers.HomeControllerTests.AdminEndpoint_WhenCalled_ReturnsOkResult"
```

### "I want to see detailed output"
```powershell
dotnet test --verbosity detailed
```

### "I want coverage report"
```powershell
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:coverage -reporttypes:Html
```

---

## 📝 Notes

- Tests are organized with traits for easy filtering
- Integration tests are skipped by default (marked with `[Fact(Skip = "...")]`)
- To enable integration tests, remove the Skip attribute and ensure API is running
- Coverage reports are generated in `coverage/` directory
- Test results can be exported to various formats (XML, HTML, Trx)

---

**Happy Testing! 🧪**
