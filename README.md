# Task Management System

A full-stack Task Management System developed using **ASP.NET Core Web API (.NET 8), Angular 17, SQL Server, and Entity Framework Core**.

The application provides secure authentication, role-based authorization, employee management, and task management functionality.

## Technology Stack

**Backend**
- ASP.NET Core Web API (.NET 8)
- C#
- Entity Framework Core
- SQL Server
- JWT Authentication
- Refresh Tokens
- Swagger / OpenAPI

**Frontend**
- Angular 17 (Standalone Components)
- TypeScript
- HTML and CSS
- Angular Router
- HTTP Client

## Features

### Authentication and Authorization
- User registration and login
- JWT-based authentication
- Refresh token implementation
- Role-based authorization
- Protected API endpoints

### Admin Features
- Create and view users
- Create tasks
- Assign tasks to employees
- View all tasks
- Edit and delete tasks
- Search, filter, and paginate tasks

### Employee Features
- Login and view assigned tasks
- Update assigned task status
- View task details

### Task Management
Each task includes:
- Title
- Description
- Priority
- Status
- Assigned User
- Created Date

**Task Status:** Pending, In Progress, Completed

**Task Priority:** Low, Medium, High

## Project Structure

```text
TaskManagementPro1/
├── TaskManagementPro/            # ASP.NET Core Web API
├── Frontend/
│   └── taskmanagement_11/        # Angular 17 Application
├── TaskManagementPro.sln
├── .gitignore
└── README.md
```

## Prerequisites

Install the following tools before running the application:

- .NET 8 SDK
- Visual Studio 2022 or Visual Studio Code
- SQL Server or SQL Server LocalDB
- Node.js and npm
- Angular CLI compatible with Angular 17

## Backend Setup

1. Clone the repository:

```bash
git clone https://github.com/vikaswork39-ops/TaskManagementPro.git
```

2. Open `TaskManagementPro.sln` in Visual Studio 2022.

3. Configure the SQL Server connection string in the backend `appsettings.json` file.

4. Configure a secure JWT signing key using .NET User Secrets or another secure configuration provider. Do not commit secret keys to GitHub.

5. Restore NuGet packages.

6. Apply Entity Framework Core migrations using Visual Studio Package Manager Console:

```powershell
Update-Database
```

Select the appropriate backend project as the default project before running the command.

7. Run the ASP.NET Core Web API.

8. Open Swagger to explore and test available API endpoints.

## Frontend Setup

1. Open a terminal in the Angular project folder:

```bash
cd Frontend/taskmanagement_11
```

2. Install dependencies:

```bash
npm install
```

3. Ensure the API URLs in the Angular services match the running backend API address.

4. Start the Angular application:

```bash
npx ng serve
```

5. Open the frontend in your browser:

```text
http://localhost:4200
```

## API Endpoints

### Authentication
- `POST /api/auth/register`
- `POST /api/auth/login`

### Task Management
- `GET /api/task/get-all`
- `GET /api/task/get-by-id/{id}`
- `POST /api/task/create`
- `PUT /api/task/update/{id}`
- `DELETE /api/task/delete/{id}`
- `GET /api/task/my-tasks`
- `PUT /api/task/update-status/{id}`
- `GET /api/task/status-options`
- `GET /api/task/priority-options`

API access depends on the authenticated user's role and endpoint authorization rules.

## Security

- JWT authentication for protected APIs
- Role-based access control
- Password hashing
- Refresh token support
- Global exception handling
- Server-side validation

## Database

The application uses SQL Server with Entity Framework Core.

The database contains entities for users, roles, tasks, and refresh tokens.

Entity Framework Core migrations are used to manage the database schema.

## Author

**Vikas Sharma**

GitHub: https://github.com/vikaswork39-ops

## Repository

https://github.com/vikaswork39-ops/TaskManagementPro
