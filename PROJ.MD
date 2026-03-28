
# 📄 VolunteerHub – Requirements Document

**System Name:** VolunteerHub Management System
**Type:** ASP.NET FW Web Forms Application; targetFramework="4.7.2" 
**Database:** Microsoft Access (ACCDB)

---

## 1. 🎯 Purpose

VolunteerHub is a **multi-tenant volunteering management system** that allows organizations (workspaces like schools/universities) to manage volunteering projects and track student participation, hours, and progress.

---

## 2. 👥 User Roles

### 2.1 Super Admin

* Full system control
* Manages all workspaces

### 2.2 Workspace Admin

* Manages a specific workspace (tenant)
* Creates projects and monitors volunteers

### 2.3 Volunteer (Student/User)

* Registers and joins a workspace
* Participates in volunteering activities

---

## 3. 🧩 Core Concepts

### Workspace (Tenant)

Represents an organization:

* University / School / NGO

Each workspace is **isolated logically** (multi-tenant).

---

## 4. ⚙️ Functional Requirements

### 4.1 Super Admin Features

* Create Workspace:

  * Name
  * Unique ID (code)
  * Logo (optional)
  * Location
  * Phone
* Create Workspace Admin:

  * Name
  * Email
  * Password
  * Assign to Workspace
* View all Workspaces
* View all Admins

---

### 4.2 Workspace Admin Features

* Create Volunteering Project:

  * Project Name
  * Project ID (unique per workspace)
  * Start Date
  * End Date
* View all Projects
* View Project Details:

  * Number of volunteers
  * Total hours
* View Volunteers:

  * List of students
  * Each student total hours
  * Remaining required hours (if defined)
* Alerts:

  * Students below required hours
* Dashboard:

  * Graph: hours per project
  * Graph: volunteers per project

---

### 4.3 Volunteer Features

#### Registration

* Name
* Email
* Password
* Workspace ID / Name (join existing workspace)

#### Login

#### After Login:

* View available Projects (only from their workspace)
* Join project(s)

#### Activity Logging

* Create Volunteering Event:

  * Project
  * Start DateTime
  * End DateTime
  * Location
  * Description
* System auto-calculates:

  * Duration (hours)

#### Personal Dashboard

* Total hours completed
* Required hours (optional configurable)
* Remaining hours
* History of events
* Graph: hours over time

---

## 5. 🧮 Business Rules

* Workspace ID must be **unique**
* Email must be **unique per user**
* Volunteer must belong to **one workspace only**
* Project belongs to **one workspace**
* Volunteers can only:

  * See projects in their workspace
* Event duration:

  * Must be **End > Start**
  * Auto-calculated (no manual input)
* No overlapping validation required (optional enhancement)

---

## 6. 🗄️ Data Model (High-Level Tables)

### Workspaces

* Id (PK)
* Name
* Code (Unique)
* LogoPath
* Location
* Phone

### Users

* Id (PK)
* Name
* Email
* Password
* Role (SuperAdmin / Admin / Volunteer)
* WorkspaceId (FK, nullable for SuperAdmin)

### Projects

* Id (PK)
* Name
* Code
* StartDate
* EndDate
* WorkspaceId (FK)

### VolunteerProjects (Join Table)

* Id (PK)
* VolunteerId (FK)
* ProjectId (FK)

### Events (Volunteer Activities)

* Id (PK)
* VolunteerId (FK)
* ProjectId (FK)
* StartDateTime
* EndDateTime
* DurationHours (calculated)
* Location
* Description
* Images (up to 5)

---

## 7. 📊 Non-Functional Requirements

* Simple UI (Web Forms)
* No JWT / advanced auth — simple session-based login
* Password stored (plain or hashed – basic level acceptable for school)
* Responsive (optional, not required now)
* Use async where applicable (DB calls optional)

---

## 8. 📈 Reporting & Visualization

* Graphs (basic):

  * Hours per project
  * Volunteer progress
* Can use:

  * Chart.js or built-in .NET chart controls

---

## 9. 🔐 Authentication & Authorization

* Login via Email + Password
* Session-based authentication
* Role-based access:

  * Super Admin → full access
  * Admin → workspace only
  * Volunteer → personal data only

---

## 10. 🚫 Out of Scope (for now)

* Real payments
* Email verification
* JWT / OAuth
* Mobile app


---

## 11. 🧪 Suggested Milestones

### Phase 1

* Auth (Login/Register)
* Workspace creation
* Basic DB

### Phase 2

* Projects
* Volunteer join

### Phase 3

* Event logging
* Hours calculation

### Phase 4

* Dashboard + graphs
* Admin monitoring

---

## 12. 🧠 Future Enhancements (Optional)

* Notifications system
* Email alerts
* Multi-role per user
* Export reports (PDF/Excel)
* Mobile version

