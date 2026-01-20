# Database Schema Documentation

## Overview
This **SQLite** database schema is designed for a zero-configuration dairy management system that handles:
- User Authentication & Roles
- Milk collection from customers (suppliers)
- Product sales (oil cake and feed products)
- Advance payments during cycles
- 10-day settlement cycles
- Stock management

## Database Design Principles

### 1. **Normalization**
- All tables are in 3rd Normal Form (3NF)
- No redundant data storage
- Clear separation of concerns

### 2. **Data Integrity**
- Primary keys on all tables
- Foreign key constraints with RESTRICT to prevent orphaned records
- Check constraints for business rules
- Unique constraints to prevent duplicates

### 3. **Audit Trail**
- Every table has `created_at`, `updated_at`, `created_by`, `updated_by`.
- Timestamps and User tracking are managed automatically via **Entity Framework Core** in the application layer.
- Full traceability of data changes.

### 4. **Performance**
- Strategic indexes on foreign keys
- Indexes on frequently queried columns
- Composite indexes for common query patterns

## Table Descriptions

### 1. **customers**
**Purpose**: Stores milk suppliers (called "customers" in business context)

**Key Fields**:
- `customer_code`: Unique identifier (e.g., CUST001)
- `full_name`: Customer's full name
- `phone_number`: Contact number
- `village`: Geographic location
- `is_active`: Soft delete flag

**Business Rules**:
- Customer code must be unique
- Active customers can have milk cycles
- Cannot delete customer with existing cycles (RESTRICT)

**Indexes**:
- `customer_code` for fast lookups
- `is_active` for filtering active customers
- `full_name` for search functionality

---

### 2. **products**
**Purpose**: Stores feed products (oil cake, cotton seed, etc.)

**Key Fields**:
- `product_code`: Unique identifier (e.g., PROD001)
- `product_name`: Display name
- `unit_of_measure`: KG, BAG, PIECE, etc.
- `unit_price`: Current selling price
- `current_stock`: Available quantity
- `reorder_level`: Minimum stock threshold

**Business Rules**:
- Price, stock, and reorder level must be >= 0
- Product code must be unique
- Stock is updated automatically via triggers (future enhancement)

**Indexes**:
- `product_code` for fast lookups
- `is_active` for filtering active products
- `current_stock` for stock reports

---

### 3. **milk_cycles**
**Purpose**: Represents 10-day milk collection periods

**Key Fields**:
- `customer_id`: Links to customer
- `cycle_start_date`: Start of 10-day period
- `cycle_end_date`: End of 10-day period
- `total_milk_amount`: Total milk value in ₹ (entered once)
- `is_settled`: Whether settlement is complete
- `settlement_date`: When settlement was done

**Business Rules**:
- End date must be >= start date
- One customer cannot have duplicate cycles on same start date
- Milk amount must be >= 0
- Cannot delete cycle with existing sales/advances (RESTRICT)

**Indexes**:
- `customer_id` for customer-specific queries
- `cycle_start_date, cycle_end_date` for date range queries
- `is_settled` for pending settlements report

---

### 4. **product_purchases**
**Purpose**: Tracks inventory purchases (stock IN)

**Key Fields**:
- `product_id`: Which product was purchased
- `purchase_date`: When purchase occurred
- `quantity`: Amount purchased
- `unit_price`: Price per unit at purchase time
- `total_amount`: quantity × unit_price
- `supplier_name`: Vendor name
- `invoice_number`: Reference document

**Business Rules**:
- Quantity must be > 0
- Unit price and total must be >= 0
- Increases `products.current_stock` (via application logic)

**Indexes**:
- `product_id` for product-wise purchase history
- `purchase_date` for date-based reports
- `invoice_number` for invoice lookup

---

### 5. **product_sales**
**Purpose**: Tracks product sales to customers (stock OUT)

**Key Fields**:
- `customer_id`: Who bought the product
- `product_id`: Which product was sold
- `cycle_id`: Which milk cycle this sale belongs to
- `sale_date`: When sale occurred
- `quantity`: Amount sold
- `unit_price`: Price per unit at sale time
- `total_amount`: quantity × unit_price

**Business Rules**:
- Quantity must be > 0
- Unit price and total must be >= 0
- Must be linked to a valid milk cycle
- Decreases `products.current_stock` (via application logic)
- Total amount is deducted from milk payment during settlement

**Indexes**:
- `customer_id` for customer purchase history
- `product_id` for product sales analysis
- `cycle_id` for settlement calculation
- `sale_date` for date-based reports

---

### 6. **advance_payments**
**Purpose**: Tracks advance money given to customers during cycle

**Key Fields**:
- `customer_id`: Who received the advance
- `cycle_id`: Which cycle the advance belongs to
- `payment_date`: When advance was given
- `amount`: Advance amount
- `payment_mode`: CASH, BANK_TRANSFER, CHEQUE, UPI
- `reference_number`: Transaction reference

**Business Rules**:
- Amount must be > 0
- Must be linked to a valid milk cycle
- Payment mode must be one of: CASH, BANK_TRANSFER, CHEQUE, UPI
- Total advances are deducted from milk payment during settlement

**Indexes**:
- `customer_id` for customer advance history
- `cycle_id` for settlement calculation
- `payment_date` for date-based reports

---

### 7. **settlements**
**Purpose**: Stores final settlement calculations for milk cycles

**Key Fields**:
- `customer_id`: Customer being settled
- `cycle_id`: Which cycle is being settled (UNIQUE - one settlement per cycle)
- `milk_amount`: Total milk value in ₹
- `total_product_sales`: Sum of all product sales in cycle
- `total_advance_paid`: Sum of all advances in cycle
- `final_payable`: **milk_amount - total_product_sales - total_advance_paid**
- `payment_mode`: How final payment was made
- `is_paid`: Whether payment is complete

**Business Rules**:
- One settlement per cycle (UNIQUE constraint on cycle_id)
- All amounts must be >= 0
- Final payable can be negative (customer owes money)
- Payment mode must be one of: CASH, BANK_TRANSFER, CHEQUE, UPI

**Settlement Formula**:
```
Final Payable = Milk Amount 
                - Total Product Purchases 
                - Advance Paid
```

**Indexes**:
- `customer_id` for customer settlement history
- `cycle_id` for cycle lookup
- `settlement_date` for date-based reports
- `is_paid` for pending payments report

---

### 8. **settlement_details**
**Purpose**: Line-item breakdown for settlement receipt printing

**Key Fields**:
- `settlement_id`: Links to settlement
- `detail_type`: MILK, PRODUCT_SALE, ADVANCE
- `reference_id`: Links to product_sales or advance_payments
- `description`: Human-readable description
- `amount`: Line item amount

**Business Rules**:
- Detail type must be: MILK, PRODUCT_SALE, ADVANCE
- MILK entries are credits (+)
- PRODUCT_SALE and ADVANCE are debits (-)
- Cascade delete when settlement is deleted

**Indexes**:
- `settlement_id` for settlement details
- `detail_type` for filtering by type

---

### 9. **users**
**Purpose**: Stores application users for authentication and authorization.

**Key Fields**:
- `username`: Unique login name.
- `password_hash`: SHA256 hashed password.
- `role`: Admin or Staff.
- `is_active`: Whether the account can log in.

## Relationships

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

## Triggers

### `update_updated_at_column()`
- **Purpose**: Automatically updates `updated_at` timestamp on row modification
- **Applied to**: All tables with `updated_at` column
- **Trigger Type**: BEFORE UPDATE

## Views

### 1. **vw_customer_outstanding**
**Purpose**: Shows customers with pending (unsettled) cycles

**Columns**:
- Customer details
- Number of pending cycles
- Total milk amount
- Total product sales
- Total advances
- Estimated payable amount

**Use Case**: Dashboard to see who needs settlement

---

### 2. **vw_product_stock_status**
**Purpose**: Shows current stock levels with status indicators

**Columns**:
- Product details
- Current stock
- Reorder level
- Stock status (LOW_STOCK, MEDIUM_STOCK, GOOD_STOCK)

**Use Case**: Inventory management and reorder alerts

---

### 3. **vw_settlement_summary**
**Purpose**: Complete settlement history with customer and cycle details

**Columns**:
- Settlement details
- Customer information
- Cycle dates
- All settlement amounts
- Payment status

**Use Case**: Settlement reports and payment tracking

## Seed Data

The schema includes sample data for:
- **5 customers** (CUST001 to CUST005)
- **5 products** (PROD001 to PROD005)
  - Oil Cake
  - Cotton Seed
  - Wheat Bran
  - Mineral Mixture
  - Cattle Feed Pellets

## Data Integrity Features

### 1. **Referential Integrity**
- All foreign keys use `ON DELETE RESTRICT`
- Prevents accidental deletion of referenced records
- Ensures data consistency

### 2. **Check Constraints**
- Positive amounts (prices, quantities, payments)
- Valid date ranges (end_date >= start_date)
- Enumerated values (payment modes)

### 3. **Unique Constraints**
- Customer codes
- Product codes
- Customer + cycle start date combination
- Cycle + settlement (one settlement per cycle)

### 4. **Default Values**
- Timestamps default to CURRENT_TIMESTAMP
- Boolean flags default to appropriate values
- Numeric fields default to 0.00

## Performance Considerations

### Indexes Strategy
1. **Primary Keys**: Automatic B-tree indexes
2. **Foreign Keys**: Explicit indexes for JOIN performance
3. **Filter Columns**: Indexes on `is_active`, `is_settled`, `is_paid`
4. **Search Columns**: Indexes on codes, names, dates
5. **Composite Indexes**: For common multi-column queries

### Query Optimization
- Views pre-aggregate common calculations
- Indexes support WHERE, JOIN, and ORDER BY clauses
- Proper data types minimize storage and improve speed

## Conclusion
This SQLite schema provides a solid foundation for the dairy management system with:
- ✅ Integrated Authentication & Roles
- ✅ Zero-Configuration Deployment
- ✅ Performance optimization
- ✅ Audit trail managed by EF Core
- ✅ Scalability for local business usage

## Conclusion

This schema provides a solid foundation for the dairy management system with:
- ✅ Complete business logic coverage
- ✅ Data integrity and consistency
- ✅ Performance optimization
- ✅ Audit trail and traceability
- ✅ Scalability for future growth
