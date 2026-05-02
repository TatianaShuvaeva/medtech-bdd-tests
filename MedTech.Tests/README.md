# medtech-bdd-tests
BDD test automation project for a MedTech system using .NET, Reqnroll, NUnit, and FluentAssertions.

## Debugging all tests in VS Code

This workspace includes a debug configuration for a full test run.

1. Set a breakpoint in a step definition or service, for example in StepDefinitions/PatientenakteSteps.cs or Infrastructure/PatientenakteService.cs.
2. Start Run and Debug and select Debug all tests.
3. When VS Code shows the process picker, choose the waiting testhost or dotnet test process.

The pre-launch task runs the full test suite with VSTEST_HOST_DEBUG=1 so the test host waits for the debugger:

dotnet test MedTech.Tests.csproj --environment VSTEST_HOST_DEBUG=1

dotnet test .\MedTech.Tests.csproj --filter "Category=sicherheit"