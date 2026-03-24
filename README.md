🧠 Technical Documentation
# 💰 SmartExpenseTracker  
A clean, modern, and intelligent way to understand where your money goes.

SmartExpenseTracker is a full‑stack application that turns messy purchase receipts into structured insights.
Upload a receipt, let the system analyze it, and instantly explore your spending through interactive dashboards.
It’s built with real‑world engineering practices — cloud integration, clean architecture, and testability baked in from day one.
## ✨ Features

### 📤 Receipt Upload & Secure Storage
- Upload JPEG, PNG, or PDF receipts from the UI  
- Files are stored safely in **Azure Blob Storage**  
- PostgreSQL stores metadata + blob references  

### 🧾 Automated Receipt Analysis
Extracts and normalizes:
- Store name  
- Purchase date  
- Total amount  
- Line items (name, quantity, unit price, total)

All data is structured into relational tables for clean analytics.

### 📊 Expense Analytics & Dashboards
- Monthly spending trends  
- Category‑wise breakdowns  
- Top items you spend on  
- API‑powered charts ready for dashboards

## 🗂️ Data Model

**User → Receipt → ReceiptItem**  
A simple 1‑to‑many chain that mirrors real‑world spending.

## 🛠️ Tech Stack

### Backend
- ASP.NET Core Web API  
- Entity Framework Core  
- PostgreSQL  
- Azure Blob Storage  
- Swagger / OpenAPI  

### Frontend
- Blazor Server  
- Chart.js  
- Bootstrap  

### Cloud & Tooling
- Azure  
- REST APIs  
- Git & GitHub  

## 📈 Sample Dashboards
- **Monthly Spend Trend** (Line Chart)  
- **Category‑wise Expense Split** (Donut Chart)  
- **Top Expense Items** (Bar Chart)  

## 🧪 Testability & QA Focus
Designed from the ground up for automation:
- DTO boundaries for clean validation  
- API endpoints ready for Postman / REST Assured  
- Database integrity checks  
- Automated flows for upload → analysis → persistence

## 🚀 Running the Application

### Prerequisites
- .NET 7+  
- PostgreSQL  
- Azure Blob Storage account  

### Setup
1. Update `appsettings.json` with:
   - PostgreSQL connection string  
   - Azure Blob Storage connection string  

2. Start the **API** project  
3. Start the **UI** project  
4. Visit:  
   - API: `https://localhost:{port}/swagger`  
   - UI: `https://localhost:{port}`
   
## 🔮 Upcoming Enhancements (In Progress)

### 🤖 AI‑Powered Receipt Intelligence
- Duplicate receipt detection  
- Spending anomaly detection
- Auto‑categorization using Azure AI / OpenAI  

