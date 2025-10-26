# Event Manager PoC

**Proof of Concept (PoC)** for an event management system built with **.NET 9**, **Angular 18**, and **PostgreSQL 17**.

## Overview

This application allows users to:

- Register and log in.
- Browse public events.
- Create and edit their own events.
- Join or leave events.
- View personal events in a calendar interface.

The project is containerized with Docker for easy setup and deployment, but also supports local development without Docker.

## Tech Stack

- **Backend**: .NET 9 (ASP.NET Core)
- **Frontend**: Angular 18
- **Database**: PostgreSQL 17
- **Containerization**: Docker and Docker Compose
- **Authentication**: JWT-based

## Requirements

Before starting, ensure you have the following installed:

| Tool        | Version | Download Link                                                            | Notes                                           |
| ----------- | ------- | ------------------------------------------------------------------------ | ----------------------------------------------- |
| Git         | Latest  | [git-scm.com](https://git-scm.com)                                       | Required for cloning the repository             |
| Docker      | Latest  | [docker.com](https://www.docker.com/get-started)                         | Includes Docker Compose for containerized setup |
| Node.js     | 22.x    | [nodejs.org](https://nodejs.org)                                         | Required for local frontend development         |
| .NET SDK    | 9.0     | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/9.0) | Required for local backend development          |
| npm         | 10.9.2+ | `npm install -g npm@10.9.2`                                              | Recommended for dependency management           |
| Angular CLI | 18.2.21  | `npm install -g @angular/cli@18.2.21`                                     | Required for local frontend development         |
| PostgreSQL  | 17      | [postgresql.org](https://www.postgresql.org/download/)                   | Required for local database setup               |

## Installation and Startup (Docker - Recommended)

### 1. Clone the repository

```bash
git clone https://github.com/Vlad-Mirzoian/Application.git
cd Application
```

### 2. Create an .env file based on the template

- Copy the template:

  - On Linux/macOS:

  ```bash
  cp .env.template .env
  ```

  - On Windows:

  ```bash
  copy .env.template .env
  ```

- Fill in the .env variables

```bash
DB_PASS=your_secure_password
JWT_KEY=your-secure-key-32-chars-long-minimum
API_URL=http://localhost:5250
```

### 3. Start the project using Docker

```bash
docker-compose up --build -d
```

### 4. Access the services:

- **Frontend:** http://localhost:4200
- **Backend:** http://localhost:5250
- **Swagger UI:** http://localhost:5250/swagger
- **PostgreSQL:** localhost:5433

  - Username: postgres
  - Password: Value of DB_PASS from .env
  - Connect using pgAdmin or psql:

```bash
psql -h localhost -p 5433 -U postgres -d eventdb
```

## Local Development (without Docker)

For development or debugging without Docker, set up the backend, frontend, and PostgreSQL manually.

### Prerequisites

Ensure all tools listed in the Requirements section are installed.

### Setup

#### 1. Clone the repository

```bash
git clone https://github.com/Vlad-Mirzoian/Application.git
cd Application
```

#### 2. Create an .env file:

- Copy the template:

  - On Linux/macOS:

  ```bash
  cp .env.template .env
  ```

  - On Windows:

  ```bash
  copy .env.template .env
  ```

- Edit .env with your values:

```bash
  DB_PASS=your_secure_password
  JWT_KEY=your-secure-key-32-chars-long-minimum
  API_URL=http://localhost:5250
```

#### 3. Set up PostgreSQL:

- Install and start PostgreSQL on your machine.
- Create a database named eventdb:

```bash
psql -U postgres -c "CREATE DATABASE eventdb;"
```

- Update the backend connection string in backend/EventApi/appsettings.json:

```bash
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=eventdb;Username=postgres;Password=your_secure_password"
  }
}
```

Replace your_secure_password with the value from DB_PASS in .env.

### Backend

```bash
cd backend/EventApi
dotnet restore
dotnet run
```

- The API will be available at http://localhost:5250.
- Swagger UI will be available at http://localhost:5250/swagger.

### Frontend

```bash
cd frontend/event-manager-frontend
npm install
ng serve
```

- The frontend will be available at http://localhost:4200.

### Notes

- Ensure the backend is running before starting the frontend, as the frontend makes API requests to http://localhost:5250.
- The database (eventdb) will be initialized with tables (Users, Events, Participants) and seed data via Entity Framework Core migrations on the first backend run.
- To verify the database:

```bash
psql -h localhost -p 5432 -U postgres -d eventdb -c "\dt"
```

## Default Credentials

The seeded database contains test users and events.

### Users

| Email             | Password    |
| ----------------- | ----------- |
| user1@example.com | password123 |
| user2@example.com | password123 |

### Events

| Event   | Visibility | Creator | Capacity |
| ------- | ---------- | ------- | -------- |
| Event 1 | Public     | user1   | 10       |
| Event 2 | Public     | user1   | 15       |
| Event 3 | Private    | user2   | 5        |

**Participation:** `user2` is a participant in **Event 1**

## API Endpoints

API documentation is available via **Swagger**:  
👉 [http://localhost:5250/swagger](http://localhost:5250/swagger)

### Main Endpoints

| Method | Endpoint                 | Description                  | Auth |
| ------ | ------------------------ | ---------------------------- | ---- |
| POST   | `/api/auth/register`     | Register a new user          | ❌   |
| POST   | `/api/auth/login`        | User login                   | ❌   |
| GET    | `/api/events`            | Get public events            | ❌   |
| GET    | `/api/events/{id}`       | Get event details            | ❌   |
| POST   | `/api/events`            | Create a new event           | ✅   |
| PATCH  | `/api/events/{id}`       | Edit an event                | ✅   |
| DELETE | `/api/events/{id}`       | Delete an event              | ✅   |
| POST   | `/api/events/{id}/join`  | Join an event                | ✅   |
| POST   | `/api/events/{id}/leave` | Leave an event               | ✅   |
| GET    | `/api/users/me/events`   | Get user’s events (calendar) | ✅   |

## Deployment

For demo or production use, the project can be deployed on **Heroku**, **AWS**, or any containerized environment.  
Currently, it runs via:

```bash
docker-compose up
```

## Development Notes

- Use **Angular DevTools** for frontend debugging
- Use **Swagger** for backend testing
- Test both **desktop** and **mobile** resolutions

## License

This project is licensed under the **MIT License**.
