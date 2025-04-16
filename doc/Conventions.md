# Using Conventions in AdaptiveFlow Projects

To ensure consistency, clarity, and maintainability in your project when integrating **AdaptiveFlow**, it's important to follow well-defined organization and naming conventions. This document outlines best practices for structuring your project and naming your classes, interfaces, and files.

> With AdaptiveFlow, you get tasks done efficiently, flexibly, and in perfect harmony—just like in a well-managed factory.

---

## **1. Project Organization**

A clean directory structure helps separate responsibilities and makes your project more manageable. Here's an example structure for an API project that uses AdaptiveFlow:

```text
/MyProject
├── /Controllers       # Handles API requests and routes
├── /Models            # Represents data structures and domain models
├── /Services          # Contains business logic
├── /Repositories      # Manages data access
├── /Interfaces        # Defines contracts for services and repositories
├── /Steps             # Implements AdaptiveFlow workflow steps
├── /Utilities         # Contains helper or utility classes
└── /Tests             # Includes unit and integration tests
```

## Explanation of Directories

- **Controllers**: Where API endpoints are defined. These interact with the Services layer to handle requests and return appropriate responses.
- **Models**: Represents the business domain, such as user or product information, usually corresponding to database entities or DTOs.
- **Services**: Contains the application's core logic and orchestrates operations between repositories and external components.
- **Repositories**: Abstracts and encapsulates interactions with databases or other storage mechanisms.
- **Interfaces**: Defines service and repository contracts, making the application extensible and testable.
- **Steps**: Stores AdaptiveFlow step implementations that define independent units of the workflow.
- **Utilities**: Stores reusable helper classes, such as for JSON parsing or date formatting.
- **Tests**: Ensures reliability and robustness through unit and integration tests.

## 2. Naming Conventions
Consistent naming helps in quickly identifying the purpose of a class or interface. Below are guidelines for naming conventions:

### Class Naming
- Use meaningful names that reflect the class's purpose.
- **Suffixes**: Include suffixes that describe the role of the class:
    - **For services**: `UserService`, `OrderService`
    - **For repositories**: `UserRepository`, `ProductRepository`
    - **For steps**: `LogStep`, `ComputeStep`

### Interface Naming
- Prefix interface names with an `I` (following C# conventions).
    - Examples: `IUserService`, `IProductRepository`

### File Naming
- Match file names to class names to ensure clarity:
    ```text
    ├── /Controllers
    │   └── HomeController.cs
    ├── /Models
    │   ├── UserModel.cs
    │   └── ProductModel.cs
    ├── /Services
    │   └── UserService.cs
    ├── /Repositories
    │   └── ProductRepository.cs
    ├── /Steps
    │   ├── FlowLogStep.cs
    │   └── FlowComputeStep.cs    
    ```

### Tests
- Include Tests as a suffix to the test file name, matching the class being tested:
    ```text
    └── /Tests
        ├── UserServiceTests.cs
        ├── FlowManagerTests.cs
        └── FlowStepWrapperTests.cs
    ```

## 3. Example: AdaptiveFlow Step Implementation
- Below is an example of how to create and name an AdaptiveFlow step class:
    
    **LogStep.cs**

    ```csharp
    public class LogStep : IFlowStep
    {
      public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default)
      {
          var logMessage = context.Get<string>("logMessage");
          Console.WriteLine($"Log: {logMessage}");
          await Task.CompletedTask;
      }
    }
    ```

This step would belong in the `/Steps` directory, clearly representing its responsibility in the workflow.

## 4. Example: AdaptiveFlow Test
- A corresponding test for the `LogStep` class could follow this pattern:

    **LogStepTests.cs**

    ```csharp
    public class LogStepTests
    {
        [Fact]
        public async Task ExecuteAsync_Should_Log_Message()
        {
            // Arrange
            var logStep = new LogStep();
            var context = new FlowContext();
            context.Set("logMessage", "Test message");

            // Act
            await logStep.ExecuteAsync(context);

            // Assert
            // Assuming you mock or spy on Console.WriteLine for verification
        }
    }
    ```
    
This ensures that the step performs its intended function and fits seamlessly into workflows.

## 5. Benefits of Adhering to Conventions

1. **Improved Readability:** A consistent structure helps developers quickly locate and understand the code.
2. **Enhanced Collaboration:** Team members can follow a common standard, reducing confusion and onboarding time.
3. **Maintainability:** Organized and clearly named components make it easier to debug and extend the application.
4. **Testability:** Separation of concerns facilitates the creation of reliable unit and integration tests.


