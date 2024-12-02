# PeakLimsApi

## Getting Started
1. Run `docker-compose up --build` from your `.sln` directory to spin up your database(s) and other supporting 
infrastructure depending on your configuration (e.g. RabbitMQ, Keycloak, Jaeger, etc.).
2. Setup the Keycloak auth server with Pulumi:
    1. [Install the pulumi CLI](https://www.pulumi.com/docs/get-started/)
    1. `cd` to your scaffolded Pulumi project
    1. Run `pulumi login --local` to use Pulumi locally
    1. Run `pulumi up` to start the scaffolding process
       > Note: The stack name must match the extension on your yaml config file (e.g. `Pulumi.dev.yaml`) would have a stack of `dev`.
    1. Select yes to apply the configuration to your local Keycloak instance.
       > If you want to reset your pulumi configuration, run `pulumi destroy` to remove all the resources and then `pulumi up` again to start fresh.
3. If running a BFF:
    1. Make sure you have [`pnpm` installed](https://pnpm.io/installation)
    1. Run the project with `dotnet run` or your IDE
4. If you want to use the corresponding UI, clone down [the UI repo](https://github.com/peaklims/peaklims-ui) and follow the instructions in the README.

### Running Your Project(s)
Once you have your database(s) running, you can run your API(s), BFF, and Auth Server by using 
the `dotnet run` command or running your projects from your IDE of choice.   

### Migrations
Migrations should be applied for you automatically on startup, but if you have any any issues, you can do the following:
    1. Make sure you have a migrations in your boundary project (there should be a `Migrations` directory in the project directory). 
    If there isn't see [Running Migrations](#running-migrations) below.
    2. Confirm your environment (`ASPNETCORE_ENVIRONMENT`) is set to `Development` using 
    `$Env:ASPNETCORE_ENVIRONMENT = "Development"` for powershell or `export ASPNETCORE_ENVIRONMENT=Development` for bash.
    3. `cd` to the boundary project root (e.g. `cd PeakLims/src/PeakLims`)
    4. Run your project and your migrations should be applied automatically. Alternatively, you can run `dotnet ef database update` to apply your migrations manually.

    > You can also stay in the `sln` root and run something like `dotnet ef database update --project PeakLims/src/PeakLims`

## Running Integration Tests
To run integration tests:

1. Ensure that you have docker installed.
2. Go to your src directory for the bounded context that you want to test.
3. Confirm that you have migrations in your infrastructure project. If you need to add them, see the [instructions below](#running-migrations).
4. Run the tests

> ⏳ If you don't have the database image pulled down to your machine, they will take some time on the first run.