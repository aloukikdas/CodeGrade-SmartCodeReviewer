# 🏗️ System Architecture

The CodeGrade (SmartCodeReviewer) platform utilizes a decoupled, event-driven microservices architecture to securely handle user authentication, data storage, and external AI processing.

---

## 📂 Project Directory Structure

The repository is organized into two distinct ASP.NET Core projects within a single solution, ensuring a clean decoupling of the frontend UI and the backend database.

```text
📦 CodeGrade-SmartCodeReviewer
 ┣ 📂 App                       # Main Application Source Code
 ┃ ┣ 📂 CodeReviewer.Api        # Backend REST API Project
 ┃ ┃ ┣ 📂 Controllers           # API Endpoints (Classrooms, Submissions, etc.)
 ┃ ┃ ┣ 📂 Data                  # Entity Framework DbContext
 ┃ ┃ ┣ 📂 DTOs                  # DTOs for secure payload mapping
 ┃ ┃ ┣ 📂 Migrations            # EF Core Database Migrations
 ┃ ┃ ┣ 📂 Models                # Database Entities (User, Assignment, etc.)
 ┃ ┃ ┣ 📜 appsettings.json      # Database Connection Strings
 ┃ ┃ ┗ 📜 Program.cs            # API Configuration & Swagger setup
 ┃ ┃
 ┃ ┣ 📂 CodeReviewer.Mvc        # Frontend Web Application
 ┃ ┃ ┣ 📂 Controllers           # UI Routing (Auth, Teacher, etc.)
 ┃ ┃ ┣ 📂 Models                # ViewModels for UI data binding
 ┃ ┃ ┣ 📂 Services              # N8nService.cs & API HttpClient integration
 ┃ ┃ ┣ 📂 Views                 # Razor Pages (.cshtml)
 ┃ ┃ ┃ ┣ 📂 Auth                # Login/Register UI
 ┃ ┃ ┃ ┣ 📂 Classroom           # Class management UI
 ┃ ┃ ┃ ┣ 📂 Home                # Landing Page UI
 ┃ ┃ ┃ ┣ 📂 Student             # Student Dashboard & Monaco Editor UI
 ┃ ┃ ┃ ┣ 📂 Teacher             # Teacher Dashboard & AI Review UI
 ┃ ┃ ┃ ┗ 📂 Shared              # _Layout.cshtml & Global styling
 ┃ ┃ ┣ 📂 wwwroot               # Static assets (CSS, JS, SweetAlert2)
 ┃ ┃ ┣ 📜 appsettings.json      # API Base URLs
 ┃ ┃ ┗ 📜 Program.cs            # MVC Configuration & HttpClient injection
 ┃ ┃
 ┗ ┗ 📜 App.slnx                # Visual Studio Solution File
```
---

## 🧩 High-Level Components

### 1. Frontend Client (`CodeReviewer.Mvc`)
The user-facing application built with ASP.NET Core MVC. It handles user sessions, renders Razor Views, and provides the interactive UI (Monaco Editor, SweetAlert2). **Crucially, this project never talks directly to the database.** It routes all data requests through the Web API via an `HttpClientFactory`.

### 2. Backend Service (`CodeReviewer.Api`)
A secure RESTful ASP.NET Core Web API. This service acts as the single source of truth and the gatekeeper to the database. It exposes endpoints for creating users, posting assignments, fetching submissions, and receiving AI grades.

### 3. Automation Engine (`n8n`)
A local, node-based automation server that acts as the "middleware" between the C# application and Google's servers. It listens for webhooks from the MVC app, structures the payload, communicates with the Gemini API, and POSTs the result back to the Web API.

### 4. Database (`SQL Server`)
Relational database managed via Entity Framework Core. Contains tables for `Users`, `Classrooms`, `Enrollments`, `Assignments`, and `Submissions`.

---

## 🔄 The AI Feedback Loop (Data Flow)

The core innovation of this platform is the automated grading loop. Here is the lifecycle of a single code submission:

1. **Trigger:** A student clicks "Submit" on the MVC frontend. 
2. **Database Save (Pending):** The MVC app sends the raw code to the Web API, which saves a new `Submission` record in SQL Server with a `null` grade.
3. **Webhook Dispatch:** The MVC app packages the Student ID, Assignment ID, and Code into a JSON payload and fires it to the n8n Webhook URL (`N8nService.cs`).
4. **AI Processing:** n8n intercepts the payload and forwards the code to the Google Gemini API with a strict system prompt instructing it to act as a harsh computer science professor and return a JSON object containing a `grade` and `feedback`.
5. **API Intercept:** n8n catches the AI's response, parses the JSON, and fires an `HTTP POST` request to the Web API's `/api/Submissions/grade` endpoint.
6. **Database Update:** The Web API finds the matching submission in the database and updates the previously null `Grade` and `AiFeedback` columns.
7. **UI Update:** The student and teacher dashboards dynamically pull this new data, allowing them to review the AI's assessment.

---

## 🗄️ Database Schema Overview

*   **Users:** ID, Name, Email, PasswordHash, Role (Teacher/Student)
*   **Classrooms:** ID, Name, Description, TeacherId
*   **Enrollments:** ID, StudentId, ClassroomId
*   **Assignments:** ID, Title, Description, AllowedLanguage, ClassroomId
*   **Submissions:** ID, CodeText, Language, SubmissionDate, Grade (Nullable), AiFeedback (Nullable), StudentId, AssignmentId