# Organization and Naming Conventions

To integrate ***AdaptiveFlow*** into your project, follow a clear structure and consistent naming conventions.

## **1. Organization**
Structure your project to separate responsibilities. Here is an example for an API:

    ```text
    /MyProject
    ├── /Controllers   # API Controllers
    ├── /Models        # Data Models
    ├── /Services      # Business Logic
    ├── /Repositories  # Data Access
    ├── /Interfaces    # Service and Repository Interfaces
    └── /Steps         # AdaptiveFlow Steps
    ```

## **2. Naming Conventions**

Adopt meaningful and standardized names:
- **Suffixes:** Use suffixes like `Service`, `Repository`, or `Step` to indicate the class purpose.
- **Interfaces:** Prefix with an uppercase `I` (C# convention), e.g., `IMyScopeService`.

    ```text
    /MyProject
    ├── /Controllers
    │   └── HomeController.cs
    ├── /Models
    │   └── UserModel.cs
    ├── /Services
    │   └── UserService.cs
    ├── /Repositories
    │   └── UserRepository.cs
    ├── /Interfaces
    │   └── IUserService.cs
    └── /Steps
        ├── LogStep.cs
        └── ComputeStep.cs
    ```