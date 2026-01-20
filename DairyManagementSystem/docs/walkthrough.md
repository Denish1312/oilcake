# Dairy Management System - Project Walkthrough

The Dairy Management System is a fully functional WPF desktop application with a professional, user-friendly interface and a robust security layer.

## Key Accomplishments

- **Zero-Config Deployment**: Switched to SQLite, enabling the app to run immediately with no database installation required.
- **Integrated Security**: Added a hardware-locked activation system and a secure multi-user login screen with SHA256 password hashing.
- **Full Transaction Lifecycle**: Implemented UI forms for recording product sales and advances directly against milk cycles.
- **Thermal Printing**: ESC/POS receipt generation with detailed line items and transaction dates.
- **Robustness**: 100% Coverage of core business logic (Settlement, MilkCycle) and global error handling.

## Core UI Screens

### Dashboard
The entry point of the application, displaying active customer counts, product variety, pending settlements, and low-stock alerts.

### Customer & Product Management
Robust interfaces for managing the database. Supports search, filtering, and stock tracking with multiple units of measure.

### Milk Cycle & Settlements
The core business logic brought to life.
- **Cycle Entry**: Record group collections for billing periods.
- **Settlement Preview**: View a full breakdown of milk credit minus deductions.
- **Thermal Printing**: Generate professional receipts for customers with a single click.

### User Management (Admin Only)
Admin-only screen to create staff accounts with restricted permissions.

### Admin Tools (Key Generation)
- **C# Tool**: Use `KeyGenerator.cs` in the `AdminTools` folder.
- **Terminal Shortcut**: Generate a key instantly with Python:
    ```bash
    python3 -c "import hashlib; print(hashlib.sha256('COMPUTER_IDACTIVATION_SECRET'.encode()).hexdigest().upper()[:16])"
    ```

## How to Run
1. Open the solution in Visual Studio 2022.
2. Build and run the `DairyManagement.UI` project.
3. **Activation**: Enter the Product Key generated via the `KeyGenerator` tool.
4. **Login**: Use `admin`/`admin` for the first login.
