# Screenr

## Install pre-requisites
You'll need to install the following pre-requisites in order to build SAFE applications

* The [.NET Core SDK](https://www.microsoft.com/net/download) 3.1 or higher.
* [npm](https://nodejs.org/en/download/) package manager.
* [Node LTS](https://nodejs.org/en/download/).

Before you run the project **for the first time only** you must install dotnet "local tools" with this command:

```bash
dotnet tool restore
```

## Recommended IDE

[Jetbrains Rider](https://www.jetbrains.com/rider/)

## Running the application

To concurrently run the server and the client components in watch mode use the following command:

```bash
dotnet fake build -t watch
```

Then open `http://localhost:8080` in your browser.

## Debugging the Application
Use Rider's debugger to debug the server. Be sure to set up your configuration to bundle the front end before starting.

To debug the front end, use the browser's built in debugger, or try the method explained [here](https://safe-stack.github.io/docs/recipes/developing-and-testing/debug-safe-app/).

## Testing the application
To run all tests:

```bash
dotnet fake build -t runtests
```

## Deploying to Azure
You'll need to get permissions to deploy to the Azure resource.

```bash
dotnet fake build -t azure
```
