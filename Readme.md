# About CatsStealer

This project's goal is to allow stealing cats data from the [Cats API](https://thecatapi.com/).

## Requirements

- .NET 8.0 or superior
- SQL Server Express 2016 or superior
- Optional (but mandatory for the actual purpose of the project) an API key from the [Cats API](https://thecatapi.com/)
- Optional, docker: for one-click run

## Set up

### VS/Terminal

1. Clone this project running `git clone https://github.com/amoraitis/steal-all-the-cats.git` in your terminal. If you haven't git installed can simply download it and unzip it.

2. Add mandatory environment variables

**Windows only examples**

*Terminal (as admin)*

```bash
setx CatsStealer_ConnectionString "value" /M
setx CatsStealer_CatsApiSettings:ApiKey "YourApiKey" /M
```

*Powershell (as admin)*

```bash
[System.Environment]::SetEnvironmentVariable("CatsStealer_ConnectionString", "value", [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("CatsStealer_CatsApiSettings:ApiKey", "YourApiKey", [System.EnvironmentVariableTarget]::Machine)
```

3. Go to the `src/CatsStealer.WebApi` folder by running the command `cd src/CatsStealer.WebApi` or manually navigating into the file system.

4. Run the command `dotnet restore` to install all the dependencies.

5. Run the command `dotnet build` to compile the project.

6. Run the command `dotnet run` to start serving the project.

7. That it's, your application is running in `http://localhost:5079/swagger`.

### Docker

1. Clone this project running `git clone` in your terminal. If you haven't git installed can simply download it and unzip it.
2. Go to the project folder by running the command `cd steal-all-the-cats` or manually navigating into the file system.
3. Add mandatory environment variables in a `.env` file in the root of the project. See example env file below. (Run `touch .env` in the terminal to create a new file)

```env
DB_CONNECTION_STRING=Server=YourConnectionString
DB_PASSWORD=YourPassword
CATS_API_KEY=YOUR_API_KEY
ASPNETCORE_ENVIRONMENT=Development
```

4. Run the command `docker-compose up -d` to start serving the project.
5. That's it, the application is running in `http://localhost:8000/swagger`.
