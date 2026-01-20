# Dairy Management System

## Overview
A production-ready desktop business application for managing a small dairy business built with **C# .NET 8**, **WPF (MVVM)**, **SQLite**, and **Entity Framework Core**.

## Business Domain

### Core Operations
- **Milk Collection**: Track milk supplied by customers over 10-day cycles
- **Product Sales**: Sell feed products (oil cake, cotton seed, etc.) to customers
- **Advance Payments**: Provide cash advances to customers during cycles
- **Settlement**: Calculate final payable amount at cycle end
- **Stock Management**: Track product inventory (purchases and sales)
- **Thermal Printing**: Print settlement receipts on ESC/POS thermal printers

### Settlement Formula
```
Final Payable = Milk Amount - Total Product Purchases - Total Advances
```

## Technology Stack

| Component | Technology |
|-----------|-----------|
| **Language** | C# (.NET 8) |
| **UI Framework** | WPF (MVVM Pattern) |
| **Database** | SQLite (Zero-Config) |
| **ORM** | Entity Framework Core |
| **Printer** | ESC/POS Thermal Printer |
| **Architecture** | Clean Architecture |
| **Target Platform** | Windows Desktop |

## Project Structure

```
DairyManagementSystem/
│
├── src/
│   ├── DairyManagement.Domain/          # Domain entities, enums, value objects
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── ValueObjects/
│   │
│   ├── DairyManagement.Application/     # Business logic, services, DTOs
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   └── DTOs/
│   │
│   ├── DairyManagement.Infrastructure/  # Data access, external services
│   │   ├── Persistence/
│   │   │   ├── DbContext/
│   │   │   ├── Configurations/
│   │   │   └── Migrations/
│   │   ├── Repositories/
│   │   └── Printing/
│   │
│   └── DairyManagement.UI/              # WPF user interface (MVVM)
│       ├── Views/
│       ├── ViewModels/
│       ├── Commands/
│       └── Resources/
│
├── docs/
│   ├── UserGuide.md                     # Complete user/admin manual
│   ├── walkthrough.md                   # Final project demonstration
│   ├── testing-strategy.md              # Testing approach
│   └── database-schema.md               # Database structure
│
└── README.md
```

## Database Schema

### Core Tables
1. **customers** - Milk suppliers (customers)
2. **products** - Feed products (oil cake, etc.)
3. **milk_cycles** - 10-day milk collection periods
4. **product_purchases** - Inventory purchases (stock in)
5. **product_sales** - Product sales to customers (stock out)
6. **advance_payments** - Cash advances during cycles
7. **settlements** - Final settlement calculations
8. **settlement_details** - Line-item breakdown for receipts
9. **users** - Application authentication and roles

### Key Relationships
```
customers (1) ----< (N) milk_cycles
customers (1) ----< (N) product_sales
customers (1) ----< (N) advance_payments
customers (1) ----< (N) settlements

products (1) ----< (N) product_purchases
products (1) ----< (N) product_sales

milk_cycles (1) ----< (N) product_sales
milk_cycles (1) ----< (N) advance_payments
milk_cycles (1) ---- (1) settlements

settlements (1) ----< (N) settlement_details
```

## Development Phases

### ✅ PHASE 1 — Database Schema
- [x] SQLite schema with all tables
- [x] Primary & foreign keys
- [x] Indexes for performance
- [x] Constraints for data integrity
- [x] Seed data
- [x] Documentation

### ✅ PHASE 2 — Domain Layer
- [x] Domain entities
- [x] Enums
- [x] Value objects
- [x] Business invariants

### ✅ PHASE 3 — Infrastructure Layer
- [x] DbContext
- [x] EF Core mappings
- [x] Repositories
- [x] Unit of work

### ✅ PHASE 4 — Application Layer
- [x] Services
- [x] Settlement calculation
- [x] Stock management
- [x] Validation logic

### ✅ PHASE 5 — UI (WPF MVVM)
- [x] Customer management
- [x] Product management
- [x] Milk cycle entry
- [x] Advance payment
- [x] Settlement screen
- [x] Reports

### ✅ PHASE 6 — Thermal Printing
- [x] ESC/POS formatting
- [x] Receipt layout
- [x] Print service

### ✅ PHASE 7 — Production Readiness
- [x] Error handling
- [x] Logging
- [x] Backup strategy
- [x] Deployment checklist
- [x] GitHub Actions CI/CD (Windows Builder)

## Installation & Setup

One of the best features of this system is **Zero-Configuration**. No database installation is required.

### Prerequisites
- .NET 8 Runtime (Standard on Windows 10/11)
- Windows 10/11
- ESC/POS thermal printer (for receipt printing)

### How to Run
1. **Unzip** the application folder.
2. **Double-click** `DairyManagement.UI.exe`.
3. **Activate**: On the first run, the app will ask for a Product Key.
   - Send your **Computer ID** to the administrator.
   - Enter the **Product Key** provided to unlock the software forever.
4. **Login**:
   - Default Username: `admin`
   - Default Password: `admin`

## Key Features

### 1. Customer Management
- Add/edit/view customers
- Track active/inactive status
- Customer search and filtering

### 2. Product Management
- Manage feed products
- Track stock levels
- Reorder alerts
- Price management

### 3. Milk Cycle Management
- Create 10-day cycles
- Enter total milk amount (₹)
- Track cycle status
- View pending settlements

### 4. Product Sales
- Sell products to customers
- Link sales to milk cycles
- Automatic stock deduction
- Sales history

### 5. Advance Payments
- Record cash advances
- Link to milk cycles
- Multiple payment modes
- Payment tracking

### 6. Settlement Processing
- Automatic calculation
- Detailed breakdown
- Thermal receipt printing
- Payment recording

### 7. Reports
- Pending settlements
- Customer outstanding
- Product stock status
- Settlement history

## Business Rules

### Settlement Rules
- One settlement per milk cycle
- Settlement formula: Milk - Products - Advances
- Negative balance allowed (customer owes money)
- Once settled, cycle is immutable

### Stock Rules
- Stock must be >= 0
- Cannot sell more than available stock
- Purchases increase stock
- Sales decrease stock

### Cycle Rules
- End date must be >= start date
- One customer cannot have duplicate cycles on same start date
- Cannot modify settled cycles

## Architecture Principles

### Clean Architecture
- **Domain Layer**: Business entities and rules (no dependencies)
- **Application Layer**: Use cases and business logic
- **Infrastructure Layer**: Data access and external services
- **UI Layer**: Presentation (WPF MVVM)

### SOLID Principles
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Subtypes must be substitutable
- **Interface Segregation**: Many specific interfaces over one general
- **Dependency Inversion**: Depend on abstractions, not concretions

### Design Patterns
- **MVVM**: Model-View-ViewModel for UI
- **Repository**: Data access abstraction
- **Unit of Work**: Transaction management
- **Dependency Injection**: Loose coupling
- **Command Pattern**: UI actions

## Documentation

- **[User & Admin Guide](docs/UserGuide.md)** - Complete user/admin manual
- **[Final Walkthrough](docs/walkthrough.md)** - Demonstration of all features
- **[Database Schema](docs/database-schema.md)** - SQLite database structure
- **[Business Rules](docs/business-rules.md)** - Core business domain rules

## Security & Licensing
The system is protected with **Hardware-Locked Activation**. Each computer generates a unique ID, and a matching product key is required to unlock the software. This ensures the software cannot be redistributed without your permission.

**For Administrator**: 
- **Tool**: Use the `KeyGenerator.cs` tool in the `AdminTools` folder.
- **Tracking**: The tool automatically logs every sale into `client_registry.csv` for your records.
- **Fast Terminal Command**:
    ```bash
    python3 -c "import hashlib; print(hashlib.sha256('COMPUTER_ID_HEREACTIVATION_SECRET'.encode()).hexdigest().upper()[:16])"
    ```

## 🚀 How You Use This (Step-by-Step)

1. **Commit workflow**
   ```bash
   git add .github/workflows/dotnet-build.yml
   git commit -m "Add Windows CI build for WPF"
   git push
   ```

2. **Create a release tag**
   ```bash
   git tag v1.1.0
   git push origin v1.1.0
   ```

## License
Proprietary - All rights reserved

---

**Version**: 1.1.0  
**Last Updated**: January 19, 2026  
**Status**: Project Complete - Production Ready
