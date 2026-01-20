# Settlement Logic Documentation

## Overview
The settlement process is the core business logic of the dairy management system. It calculates the final amount payable to customers (milk suppliers) at the end of each 10-day cycle.

## Business Flow

### 1. **Cycle Creation**
- A new milk cycle is created for a customer
- Start date and end date (10 days) are defined
- Cycle remains open for transactions

### 2. **During the Cycle**
Customers can:
- **Sell milk** → Total milk amount is entered once (not daily)
- **Buy products** → Oil cake, feed, etc. (decreases what they receive)
- **Take advances** → Cash advances during the cycle (decreases what they receive)

### 3. **Settlement Calculation**
At the end of the cycle, the system calculates:

```
Final Payable = Milk Amount 
                - Total Product Purchases 
                - Total Advances Paid
```

### 4. **Settlement Record**
- A settlement record is created
- Receipt is printed (thermal printer)
- Payment is made to customer
- Cycle is marked as settled

## Detailed Settlement Formula

### Components

#### A. **Milk Amount** (Credit to Customer)
- Total value of milk supplied during the 10-day cycle
- Entered as a single amount in ₹
- Example: ₹5,000

#### B. **Product Purchases** (Debit from Customer)
- Sum of all product sales during the cycle
- Each sale: quantity × unit_price
- Example:
  ```
  Oil Cake: 20 KG × ₹25 = ₹500
  Cotton Seed: 10 KG × ₹30 = ₹300
  Total Product Purchases = ₹800
  ```

#### C. **Advance Payments** (Debit from Customer)
- Sum of all cash advances given during the cycle
- Example:
  ```
  Advance 1: ₹1,000 (Date: Day 3)
  Advance 2: ₹500 (Date: Day 7)
  Total Advances = ₹1,500
  ```

#### D. **Final Payable**
```
Final Payable = ₹5,000 - ₹800 - ₹1,500 = ₹2,700
```

## Settlement Scenarios

### Scenario 1: Normal Settlement (Positive Balance)
```
Milk Amount:           ₹10,000
Product Purchases:     -₹2,500
Advances Paid:         -₹3,000
─────────────────────────────
Final Payable:         ₹4,500  ✅ Pay customer ₹4,500
```

### Scenario 2: Zero Balance
```
Milk Amount:           ₹5,000
Product Purchases:     -₹3,000
Advances Paid:         -₹2,000
─────────────────────────────
Final Payable:         ₹0      ✅ No payment needed
```

### Scenario 3: Negative Balance (Customer Owes Money)
```
Milk Amount:           ₹3,000
Product Purchases:     -₹2,000
Advances Paid:         -₹2,500
─────────────────────────────
Final Payable:         -₹1,500 ⚠️ Customer owes ₹1,500
```

**Handling Negative Balance**:
- Carry forward to next cycle
- Deduct from next settlement
- Or collect immediately (business decision)

### Scenario 4: Only Milk (No Deductions)
```
Milk Amount:           ₹8,000
Product Purchases:     -₹0
Advances Paid:         -₹0
─────────────────────────────
Final Payable:         ₹8,000  ✅ Pay full milk amount
```

## Database Implementation

### Step 1: Calculate Product Sales Total
```sql
SELECT COALESCE(SUM(total_amount), 0) AS total_product_sales
FROM product_sales
WHERE cycle_id = @cycle_id;
```

### Step 2: Calculate Advance Payments Total
```sql
SELECT COALESCE(SUM(amount), 0) AS total_advance_paid
FROM advance_payments
WHERE cycle_id = @cycle_id;
```

### Step 3: Get Milk Amount
```sql
SELECT total_milk_amount
FROM milk_cycles
WHERE cycle_id = @cycle_id;
```

### Step 4: Create Settlement Record
```sql
INSERT INTO settlements (
    customer_id,
    cycle_id,
    settlement_date,
    milk_amount,
    total_product_sales,
    total_advance_paid,
    final_payable,
    payment_mode,
    is_paid
) VALUES (
    @customer_id,
    @cycle_id,
    CURRENT_TIMESTAMP,
    @milk_amount,
    @total_product_sales,
    @total_advance_paid,
    @milk_amount - @total_product_sales - @total_advance_paid,
    @payment_mode,
    false
);
```

### Step 5: Create Settlement Details (for Receipt)
```sql
-- Milk entry (credit)
INSERT INTO settlement_details (settlement_id, detail_type, description, amount)
VALUES (@settlement_id, 'MILK', 'Milk Amount (10 days)', @milk_amount);

-- Product sales entries (debits)
INSERT INTO settlement_details (settlement_id, detail_type, reference_id, description, amount)
SELECT 
    @settlement_id,
    'PRODUCT_SALE',
    sale_id,
    CONCAT(p.product_name, ' - ', ps.quantity, ' ', p.unit_of_measure),
    ps.total_amount
FROM product_sales ps
INNER JOIN products p ON ps.product_id = p.product_id
WHERE ps.cycle_id = @cycle_id;

-- Advance entries (debits)
INSERT INTO settlement_details (settlement_id, detail_type, reference_id, description, amount)
SELECT 
    @settlement_id,
    'ADVANCE',
    advance_id,
    CONCAT('Advance on ', TO_CHAR(payment_date, 'DD/MM/YYYY')),
    amount
FROM advance_payments
WHERE cycle_id = @cycle_id;
```

### Step 6: Mark Cycle as Settled
```sql
UPDATE milk_cycles
SET is_settled = true,
    settlement_date = CURRENT_TIMESTAMP
WHERE cycle_id = @cycle_id;
```

## Application Layer Logic (C# Service)

### SettlementService.CalculateSettlement()

```csharp
public async Task<SettlementDto> CalculateSettlement(int cycleId)
{
    // 1. Get milk cycle
    var cycle = await _unitOfWork.MilkCycles.GetByIdAsync(cycleId);
    if (cycle.IsSettled)
        throw new BusinessException("Cycle already settled");

    // 2. Calculate product sales
    var productSales = await _unitOfWork.ProductSales
        .GetByCycleIdAsync(cycleId);
    var totalProductSales = productSales.Sum(ps => ps.TotalAmount);

    // 3. Calculate advances
    var advances = await _unitOfWork.AdvancePayments
        .GetByCycleIdAsync(cycleId);
    var totalAdvances = advances.Sum(a => a.Amount);

    // 4. Calculate final payable
    var finalPayable = cycle.TotalMilkAmount 
                       - totalProductSales 
                       - totalAdvances;

    // 5. Create settlement
    var settlement = new Settlement
    {
        CustomerId = cycle.CustomerId,
        CycleId = cycleId,
        SettlementDate = DateTime.UtcNow,
        MilkAmount = cycle.TotalMilkAmount,
        TotalProductSales = totalProductSales,
        TotalAdvancePaid = totalAdvances,
        FinalPayable = finalPayable,
        IsPaid = false
    };

    await _unitOfWork.Settlements.AddAsync(settlement);
    await _unitOfWork.SaveChangesAsync();

    return MapToDto(settlement);
}
```

## Business Rules & Validations

### 1. **Cycle Validation**
- ✅ Cycle must exist
- ✅ Cycle must not be already settled
- ✅ Cycle must have milk amount > 0
- ✅ Cycle dates must be valid

### 2. **Product Sales Validation**
- ✅ All sales must belong to the cycle
- ✅ All sales must be for the correct customer
- ✅ Product stock must be sufficient
- ✅ Prices must be positive

### 3. **Advance Validation**
- ✅ All advances must belong to the cycle
- ✅ All advances must be for the correct customer
- ✅ Advance amounts must be positive
- ✅ Payment mode must be valid

### 4. **Settlement Validation**
- ✅ One settlement per cycle (database constraint)
- ✅ Settlement date must be >= cycle end date
- ✅ All amounts must be calculated correctly
- ✅ Cannot modify settled cycle

## Error Handling

### Common Errors

1. **Cycle Already Settled**
   - Error: "This cycle has already been settled"
   - Action: Show existing settlement details

2. **No Milk Amount**
   - Error: "Milk amount not entered for this cycle"
   - Action: Redirect to milk entry screen

3. **Insufficient Stock**
   - Error: "Insufficient stock for product: {product_name}"
   - Action: Cancel settlement, adjust stock

4. **Negative Balance**
   - Warning: "Customer owes ₹{amount}. Proceed?"
   - Action: Confirm before creating settlement

## Receipt Format (Thermal Printer)

```
═══════════════════════════════════════
        DAIRY SETTLEMENT RECEIPT
═══════════════════════════════════════
Customer: Ramesh Kumar (CUST001)
Phone: 9876543210
Cycle: 01/01/2026 to 10/01/2026
Settlement Date: 10/01/2026 18:30
───────────────────────────────────────
CREDITS:
Milk Amount (10 days)        ₹10,000.00
───────────────────────────────────────
DEBITS:
Oil Cake - 20 KG               -₹500.00
Cotton Seed - 10 KG            -₹300.00
Advance on 03/01/2026        -₹1,000.00
Advance on 07/01/2026          -₹500.00
───────────────────────────────────────
Total Milk Amount:           ₹10,000.00
Total Product Purchases:        -₹800.00
Total Advances:               -₹1,500.00
───────────────────────────────────────
FINAL PAYABLE:                ₹7,700.00
═══════════════════════════════════════
Payment Mode: CASH
Paid: YES
───────────────────────────────────────
Signature: _______________
───────────────────────────────────────
Thank you for your business!
═══════════════════════════════════════
```

## Reporting Queries

### 1. **Pending Settlements**
```sql
SELECT 
    c.customer_code,
    c.full_name,
    mc.cycle_start_date,
    mc.cycle_end_date,
    mc.total_milk_amount,
    COALESCE(SUM(ps.total_amount), 0) AS product_sales,
    COALESCE(SUM(ap.amount), 0) AS advances,
    mc.total_milk_amount - 
        COALESCE(SUM(ps.total_amount), 0) - 
        COALESCE(SUM(ap.amount), 0) AS estimated_payable
FROM milk_cycles mc
INNER JOIN customers c ON mc.customer_id = c.customer_id
LEFT JOIN product_sales ps ON mc.cycle_id = ps.cycle_id
LEFT JOIN advance_payments ap ON mc.cycle_id = ap.cycle_id
WHERE mc.is_settled = false
GROUP BY c.customer_code, c.full_name, mc.cycle_id;
```

### 2. **Settlement History**
```sql
SELECT 
    s.settlement_date,
    c.customer_code,
    c.full_name,
    s.milk_amount,
    s.total_product_sales,
    s.total_advance_paid,
    s.final_payable,
    s.payment_mode,
    s.is_paid
FROM settlements s
INNER JOIN customers c ON s.customer_id = c.customer_id
ORDER BY s.settlement_date DESC;
```

### 3. **Customer Settlement Summary**
```sql
SELECT 
    c.customer_code,
    c.full_name,
    COUNT(s.settlement_id) AS total_settlements,
    SUM(s.milk_amount) AS total_milk,
    SUM(s.total_product_sales) AS total_purchases,
    SUM(s.total_advance_paid) AS total_advances,
    SUM(s.final_payable) AS total_paid
FROM customers c
LEFT JOIN settlements s ON c.customer_id = s.customer_id
GROUP BY c.customer_id;
```

## Testing Scenarios

### Test Case 1: Normal Settlement
- Create cycle with ₹10,000 milk
- Add 2 product sales (₹800 total)
- Add 1 advance (₹1,500)
- Expected: Final payable = ₹7,700

### Test Case 2: No Deductions
- Create cycle with ₹5,000 milk
- No product sales
- No advances
- Expected: Final payable = ₹5,000

### Test Case 3: Negative Balance
- Create cycle with ₹2,000 milk
- Add product sales (₹1,500)
- Add advance (₹1,000)
- Expected: Final payable = -₹500 (customer owes)

### Test Case 4: Multiple Products
- Create cycle with ₹15,000 milk
- Add 5 different product sales
- Add 3 advances
- Verify all line items in settlement_details

## Conclusion

The settlement logic is straightforward but critical:
1. **Simple formula**: Milk - Products - Advances
2. **Clear audit trail**: All transactions linked to cycle
3. **Detailed breakdown**: Line items for transparency
4. **Flexible payment**: Multiple payment modes supported
5. **Immutable**: Once settled, cycle cannot be modified
