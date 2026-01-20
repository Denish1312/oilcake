# Domain Layer Documentation

## Overview
The Domain Layer is the core of the application, containing all business entities, rules, and logic. It has **no dependencies** on other layers, following Clean Architecture principles.

## Structure

```
DairyManagement.Domain/
├── Entities/           # Domain entities (business objects)
├── Enums/             # Enumerations
├── ValueObjects/      # Immutable value objects
└── DairyManagement.Domain.csproj
```

## Design Principles

### 1. **Domain-Driven Design (DDD)**
- Rich domain models with behavior
- Business logic encapsulated in entities
- Value objects for concepts without identity
- Ubiquitous language from business domain

### 2. **Encapsulation**
- Private setters on all properties
- Factory methods for creation
- Validation in constructors and methods
- Immutable value objects

### 3. **Business Invariants**
- All business rules enforced at domain level
- Invalid states impossible to create
- Exceptions thrown for rule violations

### 4. **No Dependencies**
- Domain layer has zero dependencies on other layers
- Only depends on .NET base libraries
- Infrastructure and Application depend on Domain, not vice versa

## Enums

### 1. **PaymentMode**
**Purpose**: Defines supported payment methods

**Values**:
- `Cash` = 1
- `BankTransfer` = 2
- `Cheque` = 3
- `UPI` = 4

**Usage**: Used in `AdvancePayment` and `Settlement` entities

---

### 2. **SettlementDetailType**
**Purpose**: Categorizes settlement line items

**Values**:
- `Milk` = 1 (credit to customer)
- `ProductSale` = 2 (debit from customer)
- `Advance` = 3 (debit from customer)

**Usage**: Used in `SettlementDetail` entity

---

### 3. **UnitOfMeasure**
**Purpose**: Defines product measurement units

**Values**:
- `Kilogram` = 1
- `Bag` = 2
- `Piece` = 3
- `Liter` = 4
- `Quintal` = 5

**Usage**: Used in `Product` entity (stored as string in database)

---

### 4. **StockStatus**
**Purpose**: Indicates product stock level

**Values**:
- `LowStock` = 1 (at or below reorder level)
- `MediumStock` = 2 (between reorder and 1.5x reorder)
- `GoodStock` = 3 (above 1.5x reorder level)
- `OutOfStock` = 4 (zero or negative)

**Usage**: Calculated property in `Product` entity

## Value Objects

### 1. **Money**
**Purpose**: Represents monetary amounts in Indian Rupees

**Key Features**:
- Immutable
- Currency-aware (always INR)
- Arithmetic operations (+, -, *)
- Comparison operators
- Allows negative amounts (for balances)

**Factory Methods**:
```csharp
Money.FromAmount(decimal amount)      // Positive amounts only
Money.FromBalance(decimal amount)     // Allows negative
Money.Zero                             // Zero money
```

**Properties**:
- `Amount`: Decimal value
- `Currency`: Always "INR"
- `IsPositive`, `IsNegative`, `IsZero`: Boolean checks

**Methods**:
- `ToString()`: Formats as "₹1,234.56"
- `ToStringWithSign()`: Formats as "+₹1,234.56" or "-₹1,234.56"

**Business Rules**:
- Cannot add/subtract money with different currencies
- FromAmount() throws exception for negative values
- FromBalance() allows negative values (for debts)

---

### 2. **DateRange**
**Purpose**: Represents a date range for milk cycles

**Key Features**:
- Immutable
- Validates end date >= start date
- Utility methods for cycle management
- Date-only (no time component)

**Factory Methods**:
```csharp
DateRange.Create(DateTime start, DateTime end)
DateRange.CreateTenDayCycle(DateTime start)
DateRange.CreateCycle(DateTime start, int days)
```

**Properties**:
- `StartDate`: Start of range
- `EndDate`: End of range
- `DurationInDays`: Number of days (inclusive)
- `HasEnded`: True if end date is past
- `IsActive`: True if today is within range
- `IsFuture`: True if start date is future

**Methods**:
- `Contains(DateTime date)`: Checks if date is in range
- `OverlapsWith(DateRange other)`: Checks for overlap
- `ToString()`: Formats as "01/01/2026 to 10/01/2026"

**Business Rules**:
- End date cannot be before start date
- Dates normalized to date-only (no time)
- Duration is inclusive (both start and end dates count)

---

### 3. **Quantity**
**Purpose**: Represents a quantity with unit of measure

**Key Features**:
- Immutable
- Unit-aware (KG, BAG, etc.)
- Arithmetic operations (+, -, *)
- Comparison operators
- Prevents negative quantities

**Factory Methods**:
```csharp
Quantity.Create(decimal value, string unit)
Quantity.Zero(string unit)
```

**Properties**:
- `Value`: Numeric quantity
- `Unit`: Unit of measure (uppercase)
- `IsZero`, `IsPositive`: Boolean checks

**Methods**:
- `IsSufficientFor(Quantity required)`: Checks if >= required
- `ToString()`: Formats as "25.00 KG"

**Business Rules**:
- Cannot add/subtract quantities with different units
- Cannot create negative quantities
- Subtraction throws if result would be negative
- Units are case-insensitive (normalized to uppercase)

## Entities

### Base Entity
**Purpose**: Base class for all domain entities

**Properties**:
- `CreatedAt`: When entity was created
- `UpdatedAt`: When entity was last updated
- `CreatedBy`: Username who created
- `UpdatedBy`: Username who last updated

**Methods**:
- `SetCreatedBy(string username)`: Sets creator
- `SetUpdatedBy(string username)`: Sets updater and updates timestamp

---

### 1. **Customer**
**Purpose**: Represents a milk supplier

**Key Properties**:
- `CustomerId`: Unique identifier
- `CustomerCode`: Unique code (e.g., CUST001)
- `FullName`: Customer's full name
- `PhoneNumber`: Contact number (optional)
- `Address`, `Village`: Location info (optional)
- `IsActive`: Active status

**Factory Methods**:
```csharp
Customer.Create(code, name, phone, address, village)
```

**Business Methods**:
- `Update(...)`: Updates customer info
- `Activate()` / `Deactivate()`: Manages active status
- `CanCreateNewCycle()`: Checks if active

**Business Rules**:
- Customer code required, max 20 chars, uppercase
- Full name required, max 200 chars
- New customers are active by default
- Only active customers can have new cycles

---

### 2. **Product**
**Purpose**: Represents a feed product

**Key Properties**:
- `ProductId`: Unique identifier
- `ProductCode`: Unique code (e.g., PROD001)
- `ProductName`: Product name
- `UnitOfMeasure`: Unit (KG, BAG, etc.)
- `UnitPrice`: Current price (Money)
- `CurrentStock`: Available quantity (Quantity)
- `ReorderLevel`: Minimum threshold (Quantity)
- `IsActive`: Active status

**Factory Methods**:
```csharp
Product.Create(code, name, unit, price, reorderLevel, description)
```

**Business Methods**:
- `Update(...)`: Updates product info
- `IncreaseStock(quantity)`: Adds stock (from purchase)
- `DecreaseStock(quantity)`: Removes stock (from sale)
- `HasSufficientStock(quantity)`: Checks availability
- `GetStockStatus()`: Returns StockStatus enum
- `NeedsReorder()`: Checks if stock <= reorder level

**Business Rules**:
- Product code required, max 20 chars, uppercase
- Product name required, max 200 chars
- Unit price cannot be negative
- Stock cannot go negative (enforced in DecreaseStock)
- New products start with zero stock

---

### 3. **MilkCycle**
**Purpose**: Represents a 10-day milk collection period

**Key Properties**:
- `CycleId`: Unique identifier
- `CustomerId`: Customer reference
- `CycleStartDate`, `CycleEndDate`: Date range
- `DateRange`: Value object for dates
- `TotalMilkAmount`: Total milk value (Money)
- `IsSettled`: Settlement status
- `SettlementDate`: When settled

**Factory Methods**:
```csharp
MilkCycle.Create(customerId, startDate, endDate, notes)
MilkCycle.CreateTenDayCycle(customerId, startDate, notes)
```

**Business Methods**:
- `SetMilkAmount(amount)`: Sets total milk value
- `MarkAsSettled()`: Marks as settled
- `CanBeSettled()`: Checks if ready for settlement
- `CalculateTotalProductSales()`: Sums product sales
- `CalculateTotalAdvances()`: Sums advances
- `CalculateEstimatedPayable()`: Preview calculation

**Business Rules**:
- End date must be >= start date
- Cannot modify settled cycles
- Cannot settle with zero milk amount
- Milk amount cannot be negative

---

### 4. **ProductPurchase**
**Purpose**: Represents inventory purchase (stock in)

**Key Properties**:
- `PurchaseId`: Unique identifier
- `ProductId`: Product reference
- `PurchaseDate`: When purchased
- `Quantity`: Amount purchased (Quantity)
- `UnitPrice`: Price per unit (Money)
- `TotalAmount`: Total cost (Money)
- `SupplierName`, `InvoiceNumber`: Optional references

**Factory Methods**:
```csharp
ProductPurchase.Create(productId, quantity, unit, unitPrice, supplier, invoice, notes)
```

**Business Methods**:
- `Update(...)`: Updates purchase details

**Business Rules**:
- Quantity must be positive
- Unit price cannot be negative
- Total amount = quantity × unit price
- Unit must match product's unit

---

### 5. **ProductSale**
**Purpose**: Represents product sale to customer (stock out)

**Key Properties**:
- `SaleId`: Unique identifier
- `CustomerId`, `ProductId`, `CycleId`: References
- `SaleDate`: When sold
- `Quantity`: Amount sold (Quantity)
- `UnitPrice`: Price per unit (Money)
- `TotalAmount`: Total value (Money)

**Factory Methods**:
```csharp
ProductSale.Create(customerId, productId, cycleId, quantity, unit, unitPrice, notes)
```

**Business Methods**:
- `ValidateCustomerMatch(cycleCustomerId)`: Ensures customer matches cycle
- `Update(...)`: Updates sale details

**Business Rules**:
- Quantity must be positive
- Unit price cannot be negative
- Total amount = quantity × unit price
- Sale customer must match cycle customer
- Cannot modify after cycle is settled

---

### 6. **AdvancePayment**
**Purpose**: Represents cash advance to customer

**Key Properties**:
- `AdvanceId`: Unique identifier
- `CustomerId`, `CycleId`: References
- `PaymentDate`: When given
- `Amount`: Advance amount (Money)
- `PaymentMode`: Payment method (enum)
- `ReferenceNumber`: Transaction reference (required for non-cash)

**Factory Methods**:
```csharp
AdvancePayment.Create(customerId, cycleId, amount, paymentMode, reference, notes)
```

**Business Methods**:
- `ValidateCustomerMatch(cycleCustomerId)`: Ensures customer matches cycle
- `Update(...)`: Updates advance details

**Business Rules**:
- Amount must be positive
- Reference number required for non-cash payments
- Advance customer must match cycle customer
- Cannot modify after cycle is settled

---

### 7. **Settlement**
**Purpose**: Represents final settlement calculation

**Key Properties**:
- `SettlementId`: Unique identifier
- `CustomerId`, `CycleId`: References
- `SettlementDate`: When settled
- `MilkAmount`: Total milk value (Money)
- `TotalProductSales`: Sum of product sales (Money)
- `TotalAdvancePaid`: Sum of advances (Money)
- `FinalPayable`: Calculated payable (Money, can be negative)
- `PaymentMode`: How paid
- `IsPaid`: Payment status

**Factory Methods**:
```csharp
Settlement.Create(customerId, cycleId, milkAmount, productSales, advances, paymentMode, notes)
```

**Business Methods**:
- `AddDetail(detail)`: Adds line item
- `MarkAsPaid(reference)`: Marks as paid
- `CustomerOwesMoney()`: Checks if negative balance
- `GetAmountOwed()`: Returns absolute amount owed
- `ValidateCalculation()`: Verifies math

**Settlement Formula**:
```
FinalPayable = MilkAmount - TotalProductSales - TotalAdvancePaid
```

**Business Rules**:
- One settlement per cycle (enforced by database)
- All amounts except final payable must be >= 0
- Final payable can be negative (customer owes money)
- Cannot modify after creation (immutable)
- Negative balance carries forward to next cycle

---

### 8. **SettlementDetail**
**Purpose**: Represents settlement line item (for receipt)

**Key Properties**:
- `DetailId`: Unique identifier
- `SettlementId`: Settlement reference
- `DetailType`: Type (Milk, ProductSale, Advance)
- `ReferenceId`: Links to ProductSale or AdvancePayment (null for Milk)
- `Description`: Human-readable text
- `Amount`: Line item amount (Money)

**Factory Methods**:
```csharp
SettlementDetail.CreateMilkDetail(settlementId, amount, description)
SettlementDetail.CreateProductSaleDetail(settlementId, saleId, description, amount)
SettlementDetail.CreateAdvanceDetail(settlementId, advanceId, description, amount)
```

**Business Methods**:
- `IsCredit()`: True for Milk entries
- `IsDebit()`: True for ProductSale and Advance
- `GetSignedAmount()`: Returns signed amount for display

**Business Rules**:
- Amount must be >= 0 (sign determined by type)
- Description required
- Milk entries have no reference ID
- ProductSale and Advance entries must have reference ID

## Relationships

```
Customer (1) ----< (N) MilkCycle
Customer (1) ----< (N) ProductSale
Customer (1) ----< (N) AdvancePayment
Customer (1) ----< (N) Settlement

Product (1) ----< (N) ProductPurchase
Product (1) ----< (N) ProductSale

MilkCycle (1) ----< (N) ProductSale
MilkCycle (1) ----< (N) AdvancePayment
MilkCycle (1) ---- (1) Settlement

Settlement (1) ----< (N) SettlementDetail
```

## Validation Strategy

### Constructor Validation
- All required fields validated in factory methods
- Exceptions thrown for invalid data
- Impossible to create invalid entities

### Method Validation
- Business methods validate parameters
- State changes validated before execution
- Clear exception messages for violations

### Immutability
- Value objects are completely immutable
- Entities have private setters
- Changes only through explicit methods

## Exception Handling

### ArgumentException
- Thrown for invalid method parameters
- Includes parameter name and reason

### InvalidOperationException
- Thrown for invalid state transitions
- Example: Modifying settled cycle
- Example: Insufficient stock

## Usage Examples

### Creating a Customer
```csharp
var customer = Customer.Create(
    customerCode: "CUST001",
    fullName: "Ramesh Kumar",
    phoneNumber: "9876543210",
    village: "Dharampur"
);
customer.SetCreatedBy("admin");
```

### Creating a Product
```csharp
var product = Product.Create(
    productCode: "PROD001",
    productName: "Oil Cake",
    unitOfMeasure: "KG",
    unitPrice: 25.00m,
    reorderLevel: 100.00m,
    description: "Premium quality oil cake"
);
```

### Creating a Milk Cycle
```csharp
var cycle = MilkCycle.CreateTenDayCycle(
    customerId: 1,
    startDate: new DateTime(2026, 1, 1),
    notes: "January cycle"
);
cycle.SetMilkAmount(10000.00m);
```

### Recording a Product Sale
```csharp
var sale = ProductSale.Create(
    customerId: 1,
    productId: 1,
    cycleId: 1,
    quantity: 20.00m,
    unit: "KG",
    unitPrice: 25.00m,
    notes: "Oil cake purchase"
);
```

### Creating a Settlement
```csharp
var settlement = Settlement.Create(
    customerId: 1,
    cycleId: 1,
    milkAmount: 10000.00m,
    totalProductSales: 800.00m,
    totalAdvancePaid: 1500.00m,
    paymentMode: PaymentMode.Cash
);
// FinalPayable = 10000 - 800 - 1500 = 7700
```

## Testing Considerations

### Unit Testing
- Test entity creation with valid/invalid data
- Test business methods
- Test value object immutability
- Test calculations (settlement formula)

### Test Coverage
- All factory methods
- All business methods
- All validation rules
- Edge cases (zero amounts, negative balances)

## Future Enhancements

### Potential Additions
1. **Domain Events**: Publish events for state changes
2. **Specifications**: Query patterns for complex filtering
3. **Aggregate Roots**: Enforce consistency boundaries
4. **Domain Services**: Complex operations spanning multiple entities

## Conclusion

The Domain Layer provides:
- ✅ Rich domain models with behavior
- ✅ Complete business rule enforcement
- ✅ Immutable value objects
- ✅ Clear validation and error handling
- ✅ No external dependencies
- ✅ Production-ready entities
