# Web Frontend

This is the frontend for the Planning Poker application, built with React, TypeScript, and Vite. It uses several modern libraries and tools, including Material-UI, Zustand, and Firebase. The application also leverages **SignalR** for real-time communication, ensuring seamless updates during collaborative sessions.

## Prerequisites

Before building the project, ensure you have the following installed:

- **Node.js** (version 18 or higher is recommended)
- **npm** (comes with Node.js)
- **Azure Static Web Apps CLI** (`@azure/static-web-apps-cli`) for deployment (optional)

## Installation

1. Clone the repository to your local machine:

   ```bash
   git clone <repository-url>
   cd web-frontend
   ```

2. Install the dependencies. Note that `@react-spring/web` does not yet officially support React 19, so you may need to use the `--force` flag to bypass peer dependency warnings:

   ```bash
   npm install --force
   ```

3. Start the development server:

   ```bash
   npm run dev
   ```

## Building the Project

To build the project for production, run:

```bash
npm run build
```

## Previewing the Build

To preview the production build locally, run:

```bash
npm run preview
```

## Linting

To lint the codebase, run:

```bash
npm run lint
```

## Deployment to Azure

To deploy the application to Azure Static Web Apps using the `@azure/static-web-apps-cli`, follow these steps:

1. Obtain a deployment token for your Azure Static Web App:

   - Go to the [Azure Portal](https://portal.azure.com/).
   - Navigate to your Static Web App resource.
   - Under the **Settings** section, select **Deployment Token**.
   - Copy the token provided.

2. Build the project for production:

   ```bash
   npm run build
   ```

3. Use `npx` to run the Azure Static Web Apps CLI and deploy the application. Replace `<deployment-token>` with your Azure deployment token:

   ```bash
   npx @azure/static-web-apps-cli deploy ./dist --app-name <app-name> --deployment-token <deployment-token>
   ```

   - `./dist`: The directory containing the production build.
   - `<app-name>`: The name of your Azure Static Web App.
   - `<deployment-token>`: Your Azure deployment token.

4. Verify the deployment by visiting the URL provided by Azure after the deployment completes.

For more details on Azure Static Web Apps, refer to the [official documentation](https://learn.microsoft.com/en-us/azure/static-web-apps/).
