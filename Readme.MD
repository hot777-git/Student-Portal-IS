# Hawassa University – Student Information System

A role-based desktop application built with **C# Windows Forms (.NET 8)** for managing students, teachers, courses, grades, and academic schedules at Hawassa University, Sidama Region, Ethiopia.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [Portals & Roles](#portals--roles)
- [Security](#security)

---

## Overview

The system provides four distinct role-based portals accessible from a single landing screen:

| Portal | Role | Access Color |
|---|---|---|
| Student Portal | Enrolled students | Blue |
| Teacher Portal | Teaching staff | Green |
| Head Teacher Portal | Department heads | Purple |
| Registration Portal | Registrar office | Red |

Each portal requires authentication. The Registrar portal uses a separate login layer before granting access.

---

## Features

### Student Portal
- View enrolled courses and semester schedule
- View grades and CGPA
- Check payment history
- View and update personal profile

### Teacher Portal
- View assigned courses
- Enter and update student grades
- View class schedule

### Head Teacher Portal (Department Head)
- Add and manage courses (code, name, credit hours, year/semester)
- Register and manage teachers within the department
- View enrolled students (read-only)
- Assign courses to teachers
- Assign courses to students
- Manage department schedule

### Registration Portal (Registrar)
- Register new students and generate student IDs (e.g. `HwU/0001/16`)
- Register new Head Teachers (new or promote existing teacher)
- View and manage all users (students, teachers, heads)
- Reset passwords for any user type
- View audit logs
- Manage academic calendar
- View rules and regulations

---

## Tech Stack

| Component | Technology |
|---|---|
| Language | C# |
| Framework | .NET 8 (`net8.0-windows`) |
| UI | Windows Forms |
| Database | Microsoft SQL Server (Express) |
| Data access | `System.Data.SqlClient` v4.8.6 |
| Password hashing | SHA-256 (built-in `System.Security.Cryptography`) |

---

## Project Structure

```
student portalzip/
│
├── Program.CS                    # App entry point; tests DB connection on startup
├── HawassaUniversity.csproj      # Project definition (net8.0-windows, WinForms)
├── Hawassauniversity.SQL         # Full database schema + seed script
│
├── Databasehelper.CS             # Shared DB helpers, auth methods, Session class
│
├── Frmmain.CS                    # Landing screen with 4 portal cards
│
├── Frmstudentforms.CS            # Student login / registration form
├── Frmstudentdashboard.CS        # Student dashboard (courses, grades, schedule, profile)
│
├── Frmteacherforms.CS            # Teacher login form
├── Frmteacherdashboard.cs        # Teacher dashboard (courses, grade entry)
│
├── Frmheadteacherforms.CS        # Head Teacher login form
├── Frmheaddashboard.cs           # Head Teacher dashboard (courses, teachers, assignments)
│
├── frmRegistrarLogin.cs          # Registrar authentication gate
├── frmRegistrationPortal.cs      # Full registrar management portal
│
├── frmGradeManagement.cs         # Grade entry interface for teachers
├── frmAcademicCalendar.cs        # Academic calendar viewer
├── frmRulesAndRegulations.cs     # University rules viewer
│
└── bin/Debug/net8.0-windows/
    └── HawassaUniversity.exe     # Compiled executable
```

---

## Prerequisites

- **Windows** (required – WinForms is Windows-only)
- [**.NET 8 SDK**](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or the .NET 8 Desktop Runtime
- **SQL Server Express** (or any SQL Server edition with a named instance `SQLEXPRESS`)  
  Download: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- **SQL Server Management Studio (SSMS)** – recommended for running the setup script

---

## Database Setup

1. Open **SSMS** and connect to your `localhost\SQLEXPRESS` instance.
2. Open the file `Hawassauniversity.SQL` from this project.
3. Run the entire script. It will:
   - Drop and recreate `HawassaUniversityDB`
   - Create all tables (`Students`, `Teachers`, `HeadCredentials`, `Courses`, `Enrollments`, `Grades`, etc.)
   - Insert any seed data

> **Warning:** The script drops the database if it already exists. Back up any existing data before re-running.

---

## Configuration

The connection string is defined in `Databasehelper.CS`:

```csharp
private const string ConnectionString =
    "Server=localhost\\SQLEXPRESS;Database=HawassaUniversityDB;" +
    "Integrated Security=True;TrustServerCertificate=True;Encrypt=False;";
```

If your SQL Server instance name or authentication method differs, update this string before building.

---

## Running the Application

### Option A – Run the pre-built executable

Navigate to:
```
bin/Debug/net8.0-windows/HawassaUniversity.exe
```
Double-click to run. The app tests the database connection on startup and shows an error dialog with instructions if the connection fails.

### Option B – Build from source

```bash
dotnet build HawassaUniversity.csproj
dotnet run --project HawassaUniversity.csproj
```

Or open the project folder in **Visual Studio 2022** and press `F5`.

---

## Portals & Roles

### Registrar
The registrar has a hardcoded or pre-seeded admin credential. After logging in via the Registration Portal gate, the registrar can create all other user accounts.

### Students
Student IDs follow the format `HwU/XXXX/YY` and are generated by the registrar. Students log in with a username and password set during registration.

### Teachers & Head Teachers
Accounts are created by either the Registrar or the Head Teacher of a department. Head Teacher credentials are managed separately in the `HeadCredentials` table.

---

## Security

- All passwords are hashed with **SHA-256** before being stored in the database — plain-text passwords are never persisted.
- A static `Session` class tracks the currently logged-in user (role, ID, name, department) for the duration of the session and is cleared on logout.
- Global exception handling in `Program.CS` catches unhandled errors and writes them to `error_log.txt` alongside the executable, preventing silent crashes.
- Email validation is enforced on registration (checks format, domain, and character rules).

---

## Contact

**IT Department – Hawassa University**  
📧 abrehamengida71@gmaii.com
📍 Hawassa, Sidama Region, Ethiopia