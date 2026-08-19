# 🎓 CodeGrade (SmartCodeReviewer)

An automated, AI-powered code submission and grading platform built for computer science educators and students. 

CodeGrade bridges the gap between manual code review and instant feedback by utilizing a robust ASP.NET Core microservices architecture, automated n8n webhooks, and the Google Gemini AI. Students submit code in a browser-based IDE, and teachers receive instant, AI-evaluated grades and comprehensive feedback directly on their dashboards.

---

## ✨ Key Features

### 👨‍🏫 Teacher Portal
* **Classroom Management:** Create and manage distinct classrooms for different courses.
* **Assignment Creation:** Post assignments with specific instructions and language constraints (e.g., C#, Python, C++, or "Any").
* **Submissions Dashboard:** View all student submissions at a glance, filter by status, and see AI-generated grades.
* **Smart Code Review:** Open a read-only Monaco Editor to safely review student code alongside the AI's detailed feedback.

### 👩‍💻 Student Portal
* **Live Code Editor:** Write and format code directly in the browser using the integrated Microsoft Monaco Editor.
* **Automated Feedback:** Submit code and receive an engaging "AI is Analyzing" UI experience.
* **Instant Results:** Review grades and read detailed, constructive feedback from the AI to learn from mistakes immediately.

---

## 🛠️ Tech Stack

* **Frontend:** ASP.NET Core MVC, Razor Pages, Bootstrap 5, SweetAlert2, Monaco Editor
* **Backend:** ASP.NET Core Web API (RESTful architecture)
* **Database:** Microsoft SQL Server, Entity Framework Core (Code-First Migrations)
* **Automation:** n8n (Self-hosted workflow automation)
* **Artificial Intelligence:** Google Gemini API

---

## 🚀 Local Setup & Installation

### Prerequisites
* Visual Studio 2022 (with ASP.NET and web development workload)
* Microsoft SQL Server & SQL Server Management Studio (SSMS)
* Node.js & npm (for n8n)
* A free Google Gemini API Key

### 1. Database Setup
1. Open the solution in Visual Studio.
2. Open `appsettings.json` in the **CodeReviewer.Api** project and update the `DefaultConnection` string to point to your local SQL Server.
3. Open the Package Manager Console, set the default project to `CodeReviewer.Api`, and run:
   ```powershell
   Update-Database
   ```

### 2. n8n Automation Setup
1. Open a terminal/command prompt and start n8n:
    ```Bash
    n8n
    ```
2. Open `http://localhost:5678` in your browser.
3. Import or recreate the 3-node workflow: `Webhook` -> `Google Gemini` -> `HTTP Request`.
4. Ensure the Gemini node is configured with your API key and strict JSON-output prompt instructions.
5. Set the workflow to **Published**.

### 3. Application Startup
1. In Visual Studio, right-click the Solution and select **Configure Startup Projects**.
2. Set it to **Multiple startup projects** and set both `CodeReviewer.Api` and `CodeReviewer.Mvc` to **Start**.
3. Run the application.
4. Register a Teacher account, create a class, and post assignments!
5. Register a Student acount, join a class via code, and write & submit codes!


---


## 👨‍💻 Author

1. **Aloukik Das**
    * **Role:** Lead Architect, DevOps & AI
    * **GitHub:** [https://github.com/aloukikdas](https://github.com/aloukikdas)

2. **Elfa Monali**
    * **Role:** Database Engineer
    * **GitHub:** [https://github.com/elfamonali](https://github.com/elfamonali)

3. **Sanjana Krishnan**
    * **Role:** Backend API Developer
    * **GitHub:** [https://github.com/SanKrishnan](https://github.com/SanKrishnan)

4. **Subrat Das**
    * **Role:** Auth & Security
    * **GitHub:** [https://github.com/imsubratdas](https://github.com/imsubratdas)

5. **Arjun Vashishtha**
    * **Role:** Frontend: Teacher Portal & Classroom
    * **GitHub:** [https://github.com/arjundroid12](https://github.com/arjundroid12)

6. **Arnav Sharma**
    * **Role:** Frontend: Student Portal
    * **GitHub:** [https://github.com/ARNAV04x](https://github.com/ARNAV04x)

7. **Rohit Mondal**
    * **Role:** UI/UX & Polish
    * **GitHub:** [https://github.com/rohit23bce10575](https://github.com/rohit23bce10575)