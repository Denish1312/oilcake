# User & Admin Guide: Dairy Management System

This guide explains how to use the system, from the first-time setup to daily operations.

---

## 🚀 1. First-Time Setup (Activation)
When you share the software with a customer, they will see an **Activation Screen**.

### Admin Flow (You):
1.  **Receive ID**: The customer sends you their unique **Computer ID** (displayed on their screen).
2.  **Generate Key**: You run the **Admin Key Generator** (logic below) using their Computer ID.
3.  **Send Key**: You send the 16-character **Product Key** back to the customer.

### User Flow (Customer):
1.  **Enter Key**: The customer pastes the key into the Activation Screen.
2.  **Unlock**: Once activated, the app creates a `license.dat` file, and the software is unlocked forever on that machine.

---

## 🔐 2. Login & User Management
The software supports multiple users with different roles.

*   **Default Admin Credentials**:
    *   **Username**: `admin`
    *   **Password**: `admin`

### Creating Staff Users:
1.  Log in as **Admin**.
2.  Go to **Users** in the top menu.
3.  Create a username and password for your staff.
4.  Staff can now log in to record milk entries but cannot change sensitive system settings.

---

## 🥛 3. Daily Operations Flow

### [User] Recording Milk Collection:
1.  **Select Customer**: Pick the farmer from the list.
2.  **Start Cycle**: Click "Record Milk Collection" and set the date range (e.g., 1st to 10th of the month).
3.  **Enter Amount**: Input the total liters of milk collected.

### [User] Recording Sales/Advances:
1.  In the **Milk Cycles** list, find the active cycle.
2.  Click **Record Transaction**.
3.  Choose **Product Sale** (to deduct items like oilcake) or **Advance Payment** (for cash advances).
4.  Save. These will be automatically deducted during settlement.

### [Admin/User] Settlement & Printing:
1.  Once a cycle is complete, click **Settle**.
2.  Review the totals. The system calculates: `Milk Credit - Product Sales - Advances = Final Payable`.
3.  Click **Print Receipt** for a thermal receipt.
4.  Mark as **Paid** once the cash is handed over.

---

### Generating via Terminal (Quickest)
If you don't want to run the C# file, you can generate a key instantly using this terminal command:

**Python (Works on Windows/Linux/Mac):**
```bash
python3 -c "import hashlib; print(hashlib.sha256('CUSTOMER_ID_HEREACTIVATION_SECRET'.encode()).hexdigest().upper()[:16])"
```
*(Replace `CUSTOMER_ID_HERE` with the ID from the client's screen).*

---

### 📦 5. Building & Publishing (Windows Only)
Since this is a **WPF Windows Desktop** application, you **must run the build command from a Windows computer**. The Linux .NET SDK does not include the Windows Desktop build tools.

**Command to generate the .exe:**
```bash
dotnet publish src/DairyManagement.UI/DairyManagement.UI.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -o ./publish
```
