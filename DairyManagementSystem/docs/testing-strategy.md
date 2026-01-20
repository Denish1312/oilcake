# Dairy Management System - Testing Strategy

To ensure the system is robust and professional before production, I suggest implementing a multi-layered testing suite using **xUnit**, **Moq**, and **FluentAssertions**.

## 1. Domain Unit Tests (High Priority)
Focus on the "Heart" of the application where business rules live.
- **Value Objects**: Verify `Money` addition/subtraction and `Quantity` unit conversions.
- **Entities**: Test `Settlement.Create` calculations to ensure Milk - Sales - Advances always equals the correct Final Payable.
- **Business Logic**: Verify that a `MilkCycle` cannot be settled twice.

## 2. Application Service Tests
Test the orchestration and workflows.
- **Settlement Service**: Use Mocks for repositories to verify that creating a settlement correctly updates the cycle status and saves details.
- **Stock Service**: Ensure that recording a product sale correctly reduces inventory and handles "Out of Stock" scenarios.
- **Receipt Service**: Verify that the thermal receipt text contains all required fields and correct date formats.

## 3. UI ViewModel Tests
Ensure the interface behaves correctly.
- **Navigation**: Verify that clicking "Cancel" in a form actually navigates back to the list.
- **State Management**: Test that `Loading` indicators turn on/off during async service calls.
- **Search**: Verify that filtering the customer list updates the `ObservableCollection`.

## 4. Infrastructure Integration Tests (Optional but Recommended)
Test the database interaction.
- **Repositories**: Verify that EF Core correctly maps our entities to SQLite tables.
- **Constraints**: Ensure database-level unique constraints (like duplicate customer codes) are caught.

---

### Implementation Plan
I suggest creating a single test project: `DairyManagement.Tests`.
1. **Setup**: Create the project and add dependencies (xUnit, Moq).
2. **Domain**: Start with `Settlement` and `Money` tests.
3. **Application**: Test the `SettlementService` and `ReceiptService`.
4. **UI**: Test the `SettlementViewModel` logic.

**Shall I proceed with setting up the test project and writing the first set of Domain tests?**
