# Project Documentation: Planning Poker Web API

## Overview

The **Planning Poker Web API** is a robust and scalable backend solution designed to facilitate real-time collaboration for agile teams during sprint planning sessions. This project leverages modern technologies, architectural principles, and design patterns to ensure maintainability, scalability, and performance. It is built with a focus on **Domain-Driven Design (DDD)**, **Clean Architecture**, and **rich domain modeling**, making it a highly extensible and well-structured application.

---

## Key Features

1. **Real-Time Communication**:
   - Utilizes **SignalR** to enable real-time updates and communication between clients. This ensures seamless collaboration during planning sessions.
   - SignalR is used to broadcast events such as story selection and participant voting updates to all connected clients.

2. **Authentication and Authorization**:
   - Integrates with **Firebase Authentication** for secure and scalable user authentication.
   - Supports JWT-based authentication with token validation and custom claims.

3. **Domain-Driven Design (DDD)**:
   - The project is structured around DDD principles, ensuring that the core business logic is encapsulated within the domain layer.
   - Rich domain models are used to represent business entities and enforce invariants.

4. **Clean Architecture**:
   - Adheres to **Clean Architecture** principles, separating concerns into distinct layers:
     - **Domain Layer**: Contains core business logic and domain models.
     - **Application Layer**: Handles use cases and application-specific logic.
     - **Infrastructure Layer**: Manages data access, external services, and other infrastructure concerns.
     - **Presentation Layer**: Exposes APIs and handles client communication.

5. **Monitoring and Observability**:
   - Integrated with **Application Insights** for monitoring and diagnostics in production environments.
   - Provides detailed telemetry data to track application performance and identify issues.

6. **Extensibility**:
   - Designed to be easily extensible, allowing for the addition of new features and integrations without impacting existing functionality.

7. **Cross-Origin Resource Sharing (CORS)**:
   - Configured to allow secure communication between the backend and the frontend application.

---

## Technologies Used

### Backend
- **C#**: The primary programming language used for development.
- **.NET 9.0**: Provides a modern, high-performance framework for building web APIs.
- **Entity Framework Core**: Used for data access and database interactions.
- **SignalR**: Enables real-time communication between the server and clients.
- **Firebase Admin SDK**: Manages authentication and integrates with Firebase services.
- **Application Insights**: Provides monitoring and telemetry for production environments.

### Database
- **Azure/SQL Server**: Used as the primary relational database for storing application data.

### Authentication
- **Firebase Authentication**: Provides secure and scalable user authentication.

### Configuration
- **User Secrets**: Used to securely store sensitive configuration data during development. -> Use it for Local development to Store API KEYS
- **Environment-Specific Configuration**: Supports `appsettings.json` and `appsettings.Development.json` for environment-specific settings.

---

## Design Patterns

1. **Command Handler Pattern**:
   - Implements the **Command Query Responsibility Segregation (CQRS)** principle.
   - Example: The `SelectStoryCommandHandler` processes commands related to story selection.

2. **Repository Pattern**:
   - While the Repository Pattern is commonly used to abstract data access logic, **Entity Framework Core** already provides a robust implementation of this pattern. This eliminates the need for custom repository classes in most scenarios.

3. **Dependency Injection**:
   - All services and dependencies are injected using the built-in .NET DI container, promoting testability and loose coupling.

4. **Observer Pattern**:
   - SignalR acts as an observer, notifying clients of changes in real-time.

5. **Rich Domain Models**:
   - Encapsulates business logic within domain entities, ensuring that the application adheres to DDD principles.

---

## Project Structure

The project is organized into the following layers:

1. **Domain**:
   - Contains core business logic, domain models, and interfaces.
   - Example: `IApplicationDbContext`, domain entities like `Story` and `Participant`.

2. **Application**:
   - Implements use cases and application-specific logic.
   - Example: Command handlers like `SelectStoryCommandHandler`.

3. **Infrastructure**:
   - Manages data access, external services, and other infrastructure concerns.
   - Example: Database context, Firebase integration.

4. **WebApi**:
   - Exposes APIs and handles client communication.
   - Example: SignalR hubs, controllers.

5. **Firebase.Auth**:
   - Handles Firebase authentication and related services.
   - Example: JWT validation and Firebase Admin SDK integration.

6. **DataAccess**:
   - Contains database-related logic, including migrations and Entity Framework configurations.
   - Example: Database context and migration files.

---

## Configuration

### Database Connection
The database connection string is configured in `appsettings.json` and `secrets.json`. Example:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Initial Catalog=planningpoker;User ID=sa;Password=yourStrong(!)Password;"
}
```

### Firebase Configuration
Firebase settings are defined in `appsettings.json`:
```json
"Firebase": {
  "Auth": {
    "TokenAuthorityUrl": "https://securetoken.google.com/{projectId}",
    "ProjectId": "{projectId}",
    "ValidIssuerUrl": "https://securetoken.google.com/{projectId}"
  },
  "CredentialPath": "/pathToYourHome/.google/{credential-file-name}.json",
  "UseJson": "False"
}
```

### CORS
CORS is configured to allow requests from the frontend application:
```json
"Cors": {
  "Origins": "http://localhost:5173"
}
```

---

## How It Works

1. **Story Selection**:
   - The `SelectStoryCommandHandler` checks if the story exists in the database.
   - If found, it broadcasts the story selection to all clients in the match group using SignalR.

2. **Real-Time Voting**:
   - Participants' votes are broadcasted in real-time using SignalR.
   - SignalR ensures that all connected clients receive updates instantly.

3. **Authentication**:
   - Firebase Authentication validates JWT tokens for secure access to the API.

4. **Monitoring**:
   - Application Insights collects telemetry data, providing insights into application performance and errors.

---

## Extensibility

The project is designed to be easily extensible. New features can be added by:
- Creating new domain models and use cases.
- Adding new SignalR hubs for additional real-time functionality.
- Extending the database schema and updating the Entity Framework context.

---

## Build and Deployment

### Building the Project

1. **Install Prerequisites**:
   - Ensure you have the following installed:
     - [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
     - [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
     - [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (optional for local testing, you can use docker to which i recommend)

2. **Clone the Repository**:
   ```bash
   git clone https://github.com/GDavij/planning-poker
   cd planning-poker/services
   ```

3. **Restore Dependencies**:
   ```bash
   dotnet restore .
   ```

4. **Build the Solution**:
   ```bash
   dotnet build .
   ```

5. **Run the Application Locally**:
   - Update the `appsettings.Development.json` file with your local configuration.
   - Start the application:
     ```bash
     dotnet run --project ./src/WebApi.csproj
     ```

6. **Test the Application**:
   - Use tools like Postman or cURL to test the API endpoints.
   - Ensure the database is running and accessible.

7. **Update Database Migrations**:
     1. Open a terminal in the project directory.
     2. Run the following command, replacing `<connection-string>` with your custom database connection string:
        ```bash
        dotnet ef database update --connection "<connection-string>" --project ./src/DataAccess/DataAccess.csproj --startup-project ./src/WebApi/WebApi.csproj
        ```
     3. Verify that the migrations have been applied successfully by checking the database schema.

---

### Deploying to Azure Web App

1. **Create an Azure Web App**:
   - Log in to Azure:
     ```bash
     az login
     ```
   - Create a resource group:
     ```bash
     az group create --name PlanningPokerRG --location <region>
     ```
   - Create an Azure Web App:
     ```bash
     az webapp create --resource-group PlanningPokerRG --plan <app-service-plan> --name <web-app-name> --runtime "DOTNET|9.0"
     ```

2. **Configure the Database**:
   - Set up an Azure SQL Database and update the connection string in the Azure Web App settings:
     ```bash
     az webapp config connection-string set --name <web-app-name> --resource-group PlanningPokerRG --settings DefaultConnection="Server=<server-name>.database.windows.net;Database=<db-name>;User ID=<username>;Password=<password>;" --connection-string-type SQLAzure
     ```

3. **Publish the Application**:
   - Publish the project to Azure:
     ```bash
     dotnet publish -c Release -o ./publish
     az webapp deploy --resource-group PlanningPokerRG --name <web-app-name> --src-path ./publish
     ```

4. **Set Environment Variables**:
   - Configure Firebase and other environment-specific settings:
     ```bash
     az webapp config appsettings set --name <web-app-name> --resource-group PlanningPokerRG --settings Firebase__Auth__TokenAuthorityUrl=<value> Firebase__Auth__ProjectId=<value> Firebase__Auth__ValidIssuerUrl=<value> Firebase__CredentialPath=<value>
     ```

5. **Enable CORS**:
   - Allow frontend communication:
     ```bash
     az webapp cors add --name <web-app-name> --resource-group PlanningPokerRG --allowed-origins http://localhost:5173
     ```

6. **Monitor the Application**:
   - Use Application Insights for monitoring:
     - Link Application Insights to the Web App:
       ```bash
       az monitor app-insights component create --app <app-insights-name> --location <region> --resource-group PlanningPokerRG
       az webapp config appsettings set --name <web-app-name> --resource-group PlanningPokerRG --settings APPINSIGHTS_INSTRUMENTATIONKEY=<instrumentation-key>
       ```

7. **Verify Deployment**:
   - Access the deployed application at `https://<web-app-name>.azurewebsites.net`.
   - Test the API endpoints to ensure everything is working as expected.

---

## Conclusion

The **Planning Poker Web API** is a modern, scalable, and maintainable solution for agile teams. By leveraging **SignalR** for real-time communication, **Firebase** for authentication, and **Application Insights** for monitoring, it provides a seamless and secure experience for users. Its adherence to **DDD**, **Clean Architecture**, and rich domain modeling ensures that the application is well-structured and ready for future growth.