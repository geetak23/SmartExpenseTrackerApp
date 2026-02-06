# SmartExpenseTracker
SmartExpenseTracker is a project born from a simple idea: managing household expenses shouldn’t feel like detective work. We all have receipts scattered in bags, inboxes, and glove compartments — so this app turns that chaos into clean, structured, meaningful insights.
With a smooth upload experience, automated receipt analysis, and beautiful dashboards, SmartExpenseTracker shows exactly where your money goes and helps you make better spending decisions. It’s built with real-world engineering practices, cloud integration, and testability at its core.

#What You Can Do with It
Upload Receipts Effortlessly
Just drag and drop your receipt (JPEG, PNG, or PDF).
The app stores it securely in Azure Blob Storage and keeps a reference in PostgreSQL — no clutter, no lost files.

#Let the System Read Your Receipt
Behind the scenes, document analysis extracts:
  Store name, Purchase date,Total amount,Every line item (name, quantity, unit price, total)
  The data is normalized into relational tables so it’s clean, searchable, and analytics‑ready.

#Understand Your Spending
The UI brings your expenses to life with:
  Monthly spending trends
  Category‑wise breakdowns
  Top items you spend on

#How the System Is Organized
SmartExpenseTracker
 ──API Layer
      Controllers, DTOs, Swagger, Program.cs 
 ──Core Layer
      Interfaces, Enums, Domain Models 
 ──Infrastructure Layer
      EF Core DbContext, Entities, Repositories
      BlobStorageService, ReceiptAnalysisService 
 ──UI (Blazor Server)
      Pages, Services, Charts, JS/CSS, App.razor

#Data Model at a Glance
A simple, intuitive structure:
  User → Receipt → ReceiptItem  
  (One user can have many receipts, and each receipt can have many items.)

#Tech Stack
  Backend
  ASP.NET Core Web API
  Entity Framework Core
  PostgreSQL
  Azure Blob Storage
  Swagger / OpenAPI

#Frontend
  Blazor Server
  JavaScript (Chart.js)
  Bootstrap

#Cloud & Tools
  Azure Blob Storage
  REST APIs
  Git & GitHub

#Dashboards You’ll See
  Line chart for monthly spending
  Donut chart for category distribution
  Bar chart for top expenses

#Testability & QA Focus
This project is designed with test automation in mind:
  API endpoints ready for Postman / REST Assured
  Clear DTO boundaries
  Database integrity checks

#Supports automation of:
Receipt upload flows
API response validation
Data persistence verification

#Running the Application
Prerequisites:
.NET 7+
PostgreSQL
Azure Blob Storage account

#Steps
> Update appsettings.json with:
> PostgreSQL connection string
> Azure Blob Storage connection string

> Start API project
> Start UI project
> Navigate to:
  API: https://localhost:{port}/swagger
  UI: https://localhost:{port}

#What’s Coming Next - In Progress
  Smarter Receipt Intelligence
  Auto‑categorization using Azure AI / OpenAI
  Duplicate receipt detection
  Anomaly detection for unusual spending

#End‑to‑End Automation
  Playwright UI tests
  API contract validation
  Database persistence checks
