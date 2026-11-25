## 🛠️ Step 1: Create the File
 Open your Developer PowerShell (make sure you are in the root folder: .

## Run this command to create the file:

powershell
New-Item README.md
Open the newly created README.md in Visual Studio.

Copy and paste the entire block of Markdown below into the file and save it.

📄 The README.md Content
markdown
# 🛡️ CommunityGuard

**CommunityGuard** is an enterprise-grade Neighbourhood Safety and Resource Coordination Platform. Designed with a Windows-first approach using **.NET MAUI**, it enables neighbourhood watch coordinators ("Controllers") and field responders to report incidents, manage resource dispatch, and track response times in real-time.

This project was built to demonstrate professional software engineering principles, moving beyond basic CRUD applications to showcase **Domain-Driven Design (DDD), Test-Driven Development (TDD), and Clean Architecture**.

---

## 🚀 Key Features

- **Role-Based Access Control (RBAC):** Secure JWT authentication with distinct roles. *Controllers* have full administrative access, while *Responders* only see incidents assigned to them.
- **Incident Lifecycle State Machine:** Incidents cannot jump randomly between statuses (e.g., Reported -> Acknowledged -> Dispatched -> Resolved -> Closed). The Domain layer strictly enforces these transitions.
- **Resource Dispatch & Double-Booking Prevention:** Assign patrol vehicles or personnel to incidents. The system uses domain rules to prevent allocating the same resource if it is already booked for that time window.
- **Immutable Audit Trail:** Every status change is recorded with a timestamp, user ID, and optional note for complete accountability.
- **Split-Screen Desktop UX:** A modern Master-Detail layout specifically designed for desktop monitors, allowing users to browse the incident list while viewing details and history simultaneously.
- **Offline-First Local Database:** Powered by SQLite, making the backend fully portable without the need for a heavy SQL Server installation.

---

## 🏗️ Architecture & Tech Stack

The solution strictly follows **Orthogonality** and the **DRY (Don't Repeat Yourself)** principle. It is divided into 6 projects to ensure a strict separation of concerns:

- **`CommunityGuard.Domain`**: The pure business logic. Contains entities, state machines, and domain exceptions. *(Zero external dependencies).*
- **`CommunityGuard.Shared`**: Data Transfer Objects (DTOs) shared between the API and the MAUI client.
- **`CommunityGuard.Api`**: ASP.NET Core Web API. Handles HTTP requests, JWT generation, and communicates with the database via Entity Framework Core.
- **`CommunityGuard.Maui`**: The .NET MAUI Windows Desktop application. Built using the **MVVM pattern** with `CommunityToolkit.Mvvm`.
- **`CommunityGuard.Domain.Tests`**: Fast unit tests covering the state machine and business rules.
- **`CommunityGuard.Api.Tests`**: Integration tests verifying database persistence and API security.

**Tech Stack:** .NET 9, C# 13, .NET MAUI, ASP.NET Core Identity, Entity Framework Core (Code-First), SQLite, xUnit, CommunityToolkit.Mvvm.

---

## ⚙️ How to Run This Project Locally

### Prerequisites
- **Visual Studio 2022** (v17.12 or later) with the **.NET Multi-platform App UI development** workload installed.
- **.NET 9 SDK**.

### Step-by-Step Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Manaleng222/CommunityGuard.git
   cd CommunityGuard
   
## Set Startup Projects:

Right-click the Solution in Visual Studio and select Configure Startup Projects...

Select Multiple startup projects.

Set CommunityGuard.Api and CommunityGuard.Maui to Start.

Run the Application:

Press F5. Visual Studio will start the API in the background and launch the Windows Desktop App.

Note: On the first run, the API will automatically generate the SQLite database (CommunityGuardDb.db) and seed the default admin user.

🔐 Default Login Credentials
Email: admin@communityguard.com

Password: Admin123!

(You can use this account to access the Controller features like Resource Dispatch and Status Updates).

🧪 Running the Tests
This project was built using Test-Driven Development (TDD). To run the test suite:

Open the Test Explorer in Visual Studio (Test -> Test Explorer).

Click Run All Tests.

You will see over 35 passing tests covering Domain Rules, Persistence, Auth, and API integration.

🔮 Future Roadmap
□ Expand target frameworks to include Android and iOS.
□ Implement real-time push notifications using SignalR.
□ Add interactive map integration for incident locations.
□ Generate MSIX installer for seamless enterprise deployment.
Developed as a professional portfolio project to demonstrate enterprise software development capabilities in the South African tech market.

text

---

### 🚀 Step 2: Push to GitHub

Now that your `README.md` is created and saved, open your Developer PowerShell and push it to your repository:

```powershell
git add .
git commit -m "docs: add comprehensive README with architecture overview"
git push origin main
