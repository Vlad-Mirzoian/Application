# Event Manager PoC

**Proof of Concept (PoC)** for an event management system built with **.NET 9**, **Angular 18**, and **PostgreSQL 16**.

## Overview

This application allows users to register, log in, browse public events, create/edit their own events, join or leave events, and view their personal events in a calendar.

---

## Requirements

Before starting, make sure you have the following installed:

- [Git](https://git-scm.com/)
- [Docker](https://www.docker.com/) and **docker-compose**
- [Node.js 18+](https://nodejs.org/) (for local frontend development)
- [.NET SDK 9+](https://dotnet.microsoft.com/download) (for local backend development)

---

## Installation and Startup

### 1. Clone the repository

```bash
git clone https://github.com/your-username/event-manager.git
cd event-manager
```

### 2. Create an .env file based on the template

```bash
   cp .env.template .env
```

### 3. Fill in the .env variables

(see example below)

### 4. Start the project using Docker

```bash
   docker-compose up --build
```

### Services

- **Backend:** [http://localhost:5000](http://localhost:5000)
- **Swagger UI:** [http://localhost:5000/swagger](http://localhost:5000/swagger)
- **Frontend:** [http://localhost:4200](http://localhost:4200)
- **PostgreSQL:** `localhost:5432`

---

## Local Development (without Docker)

### Backend

```bash
cd backend/EventApi
dotnet restore
dotnet run
```

### Frontend

```bash
cd frontend/event-manager-frontend
npm install
ng serve
```

## Configuration (`.env`)

```bash
DB_PASS=your_secure_password
JWT_KEY=your-secure-key-32-chars-long-minimum
API_URL=http://localhost:5250/api
```

---

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
| Event 2 | Public     | user1   | —        |
| Event 3 | Private    | user2   | —        |

**Participation:** `user2` is a participant in **Event 1**

---

## API Endpoints

API documentation is available via **Swagger**:  
👉 [http://localhost:5250/swagger](http://localhost:5000/swagger)

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

---

## Deployment

For demo or production use, the project can be deployed on **Heroku**, **AWS**, or any containerized environment.  
Currently, it runs via:

```bash
docker-compose up
```

---

## Development Notes

- Use **Angular DevTools** for frontend debugging
- Use **Swagger** for backend testing
- Test both **desktop** and **mobile** resolutions

---

## License

This project is licensed under the **MIT License**.
