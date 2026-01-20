# Business Rules Documentation

## Overview
This document defines all business rules and constraints for the dairy management system. These rules ensure data integrity, business logic correctness, and operational consistency.

## 1. Customer Management

### Rule 1.1: Customer Code Uniqueness
- **Rule**: Each customer must have a unique customer code
- **Enforcement**: Database UNIQUE constraint
- **Format**: CUST001, CUST002, etc.
- **Validation**: Application layer checks before insert

### Rule 1.2: Active Customer Status
- **Rule**: Only active customers can have new milk cycles
- **Enforcement**: Application layer validation
- **Impact**: Inactive customers' historical data remains intact

### Rule 1.3: Customer Deletion
- **Rule**: Cannot delete customer with existing cycles
- **Enforcement**: Database FOREIGN KEY RESTRICT
- **Alternative**: Mark customer as inactive

### Rule 1.4: Contact Information
- **Rule**: Phone number is optional but recommended
- **Format**: 10-digit Indian mobile number
- **Validation**: Application layer regex validation

## 2. Product Management

### Rule 2.1: Product Code Uniqueness
- **Rule**: Each product must have a unique product code
- **Enforcement**: Database UNIQUE constraint
- **Format**: PROD001, PROD002, etc.

### Rule 2.2: Price Validation
- **Rule**: Unit price must be >= 0
- **Enforcement**: Database CHECK constraint
- **Business Logic**: Price of 0 means free item

### Rule 2.3: Stock Validation
- **Rule**: Current stock must be >= 0
- **Enforcement**: Database CHECK constraint
- **Business Logic**: Negative stock is not allowed

### Rule 2.4: Stock Updates
- **Rule**: Stock increases on purchase, decreases on sale
- **Enforcement**: Application layer logic
- **Validation**: Check sufficient stock before sale

### Rule 2.5: Reorder Level
- **Rule**: Reorder level must be >= 0
- **Enforcement**: Database CHECK constraint
- **Business Logic**: Alert when stock <= reorder level

### Rule 2.6: Product Deletion
- **Rule**: Cannot delete product with existing transactions
- **Enforcement**: Database FOREIGN KEY RESTRICT
- **Alternative**: Mark product as inactive

## 3. Milk Cycle Management

### Rule 3.1: Cycle Duration
- **Rule**: Cycle end date must be >= start date
- **Enforcement**: Database CHECK constraint
- **Business Logic**: Typically 10 days, but flexible

### Rule 3.2: Cycle Uniqueness
- **Rule**: One customer cannot have duplicate cycles on same start date
- **Enforcement**: Database UNIQUE constraint (customer_id, cycle_start_date)
- **Reason**: Prevents accidental duplicate entries

### Rule 3.3: Milk Amount Entry
- **Rule**: Milk amount is entered once per cycle (not daily)
- **Enforcement**: Application layer design
- **Business Logic**: Total milk value in ₹ for entire cycle

### Rule 3.4: Milk Amount Validation
- **Rule**: Milk amount must be >= 0
- **Enforcement**: Database CHECK constraint
- **Business Logic**: Zero means no milk supplied

### Rule 3.5: Settlement Status
- **Rule**: Once settled, cycle cannot be modified
- **Enforcement**: Application layer validation
- **Impact**: No edits to milk amount, sales, or advances after settlement

### Rule 3.6: Settlement Date
- **Rule**: Settlement date should be >= cycle end date
- **Enforcement**: Application layer validation (warning, not error)
- **Reason**: Settlement typically happens after cycle ends

### Rule 3.7: Cycle Deletion
- **Rule**: Cannot delete cycle with existing sales or advances
- **Enforcement**: Database FOREIGN KEY RESTRICT
- **Reason**: Maintains referential integrity

## 4. Product Purchase Management

### Rule 4.1: Quantity Validation
- **Rule**: Purchase quantity must be > 0
- **Enforcement**: Database CHECK constraint
- **Reason**: Cannot purchase negative or zero quantity

### Rule 4.2: Price Validation
- **Rule**: Unit price must be >= 0
- **Enforcement**: Database CHECK constraint
- **Business Logic**: Free items have price 0

### Rule 4.3: Total Amount Calculation
- **Rule**: Total amount = quantity × unit_price
- **Enforcement**: Application layer calculation
- **Validation**: Database CHECK constraint (total >= 0)

### Rule 4.4: Stock Update
- **Rule**: Stock increases by purchase quantity
- **Enforcement**: Application layer logic
- **Transaction**: Must be atomic (purchase + stock update)

### Rule 4.5: Invoice Number
- **Rule**: Invoice number is optional but recommended
- **Uniqueness**: Not enforced (same supplier may reuse numbers)

## 5. Product Sale Management

### Rule 5.1: Quantity Validation
- **Rule**: Sale quantity must be > 0
- **Enforcement**: Database CHECK constraint
- **Reason**: Cannot sell negative or zero quantity

### Rule 5.2: Stock Availability
- **Rule**: Cannot sell more than available stock
- **Enforcement**: Application layer validation
- **Error**: "Insufficient stock for {product_name}"

### Rule 5.3: Price Validation
- **Rule**: Unit price must be >= 0
- **Enforcement**: Database CHECK constraint
- **Business Logic**: Free items have price 0

### Rule 5.4: Total Amount Calculation
- **Rule**: Total amount = quantity × unit_price
- **Enforcement**: Application layer calculation
- **Validation**: Database CHECK constraint (total >= 0)

### Rule 5.5: Cycle Association
- **Rule**: Every sale must be linked to a milk cycle
- **Enforcement**: Database FOREIGN KEY NOT NULL
- **Reason**: Required for settlement calculation

### Rule 5.6: Customer Validation
- **Rule**: Sale customer must match cycle customer
- **Enforcement**: Application layer validation
- **Reason**: Prevents cross-customer sales

### Rule 5.7: Settled Cycle
- **Rule**: Cannot add sales to settled cycle
- **Enforcement**: Application layer validation
- **Reason**: Settlement is immutable

### Rule 5.8: Stock Update
- **Rule**: Stock decreases by sale quantity
- **Enforcement**: Application layer logic
- **Transaction**: Must be atomic (sale + stock update)

## 6. Advance Payment Management

### Rule 6.1: Amount Validation
- **Rule**: Advance amount must be > 0
- **Enforcement**: Database CHECK constraint
- **Reason**: Cannot give negative or zero advance

### Rule 6.2: Cycle Association
- **Rule**: Every advance must be linked to a milk cycle
- **Enforcement**: Database FOREIGN KEY NOT NULL
- **Reason**: Required for settlement calculation

### Rule 6.3: Customer Validation
- **Rule**: Advance customer must match cycle customer
- **Enforcement**: Application layer validation
- **Reason**: Prevents cross-customer advances

### Rule 6.4: Settled Cycle
- **Rule**: Cannot add advances to settled cycle
- **Enforcement**: Application layer validation
- **Reason**: Settlement is immutable

### Rule 6.5: Payment Mode
- **Rule**: Payment mode must be CASH, BANK_TRANSFER, CHEQUE, or UPI
- **Enforcement**: Database CHECK constraint
- **Validation**: Application layer dropdown

### Rule 6.6: Reference Number
- **Rule**: Reference number required for non-cash payments
- **Enforcement**: Application layer validation
- **Reason**: Audit trail for bank transfers, cheques, UPI

## 7. Settlement Management

### Rule 7.1: One Settlement Per Cycle
- **Rule**: Each cycle can have only one settlement
- **Enforcement**: Database UNIQUE constraint on cycle_id
- **Reason**: Prevents duplicate settlements

### Rule 7.2: Settlement Formula
- **Rule**: Final Payable = Milk Amount - Product Sales - Advances
- **Enforcement**: Application layer calculation
- **Validation**: All amounts must be >= 0 (except final payable)

### Rule 7.3: Negative Balance Handling
- **Rule**: Final payable can be negative (customer owes money)
- **Enforcement**: No database constraint
- **Business Logic**: 
  - Option 1: Carry forward to next cycle
  - Option 2: Collect immediately
  - Option 3: Adjust in next settlement

### Rule 7.4: Settlement Date
- **Rule**: Settlement date should be >= cycle end date
- **Enforcement**: Application layer warning (not error)
- **Reason**: Typically settled after cycle ends

### Rule 7.5: Payment Mode
- **Rule**: Payment mode must be CASH, BANK_TRANSFER, CHEQUE, or UPI
- **Enforcement**: Database CHECK constraint
- **Validation**: Application layer dropdown

### Rule 7.6: Payment Status
- **Rule**: Settlement can be created but not paid immediately
- **Enforcement**: is_paid flag (default false)
- **Business Logic**: Mark as paid after actual payment

### Rule 7.7: Settlement Immutability
- **Rule**: Once created, settlement cannot be deleted
- **Enforcement**: Application layer restriction
- **Reason**: Audit trail and legal compliance
- **Alternative**: Void/cancel flag if needed

### Rule 7.8: Cycle Update
- **Rule**: When settlement is created, cycle is marked as settled
- **Enforcement**: Application layer logic
- **Transaction**: Must be atomic (settlement + cycle update)

## 8. Settlement Details Management

### Rule 8.1: Detail Types
- **Rule**: Detail type must be MILK, PRODUCT_SALE, or ADVANCE
- **Enforcement**: Database CHECK constraint
- **Validation**: Application layer logic

### Rule 8.2: Reference Linking
- **Rule**: 
  - PRODUCT_SALE details link to product_sales.sale_id
  - ADVANCE details link to advance_payments.advance_id
  - MILK details have no reference_id
- **Enforcement**: Application layer logic
- **Reason**: Traceability to source transactions

### Rule 8.3: Cascade Delete
- **Rule**: When settlement is deleted, details are auto-deleted
- **Enforcement**: Database FOREIGN KEY CASCADE
- **Reason**: Details are meaningless without settlement

### Rule 8.4: Amount Sign
- **Rule**: 
  - MILK: Positive (credit to customer)
  - PRODUCT_SALE: Positive (debit from customer)
  - ADVANCE: Positive (debit from customer)
- **Enforcement**: Application layer logic
- **Display**: Show PRODUCT_SALE and ADVANCE as negative on receipt

## 9. Audit and Compliance

### Rule 9.1: Audit Fields
- **Rule**: All tables must have created_at, updated_at, created_by, updated_by
- **Enforcement**: Database schema
- **Automation**: Triggers update updated_at automatically

### Rule 9.2: Timestamp Accuracy
- **Rule**: Use CURRENT_TIMESTAMP for all timestamps
- **Enforcement**: Database defaults
- **Reason**: Server time is authoritative

### Rule 9.3: User Tracking
- **Rule**: created_by and updated_by should store username
- **Enforcement**: Application layer sets values
- **Reason**: Audit trail for compliance

### Rule 9.4: Data Retention
- **Rule**: No automatic deletion of historical data
- **Enforcement**: Application layer policy
- **Reason**: Legal and business requirements

## 10. Stock Management

### Rule 10.1: Stock Calculation
- **Rule**: Current stock = Previous stock + Purchases - Sales
- **Enforcement**: Application layer logic
- **Validation**: Stock should never go negative

### Rule 10.2: Reorder Alert
- **Rule**: Alert when stock <= reorder level
- **Enforcement**: Application layer notification
- **Display**: Show in dashboard/reports

### Rule 10.3: Stock Adjustment
- **Rule**: Manual stock adjustments require reason
- **Enforcement**: Application layer validation
- **Audit**: Log all adjustments with reason

## 11. Data Validation

### Rule 11.1: Required Fields
- **Rule**: All NOT NULL fields must have values
- **Enforcement**: Database schema + application validation
- **User Experience**: Mark required fields in UI

### Rule 11.2: Data Types
- **Rule**: Enforce correct data types (dates, decimals, integers)
- **Enforcement**: Database schema + application validation
- **User Experience**: Use appropriate input controls

### Rule 11.3: String Lengths
- **Rule**: Respect VARCHAR limits
- **Enforcement**: Database schema + application validation
- **User Experience**: Show character count in UI

### Rule 11.4: Decimal Precision
- **Rule**: Amounts use DECIMAL(10, 2) for accuracy
- **Enforcement**: Database schema
- **Reason**: Avoid floating-point errors

## 12. Security and Access

### Rule 12.1: User Authentication
- **Rule**: All operations require an authenticated user.
- **Enforcement**: Secure login screen.
- **Logic**: Use SHA256 password hashing for storage.

### Rule 12.2: Role-Based Access
- **Rule**: Distinct permissions for Admin and Staff.
- **Logic**: 
  - Admin: Full access to all features.
  - Staff: Limited to daily operations (no user management).

### Rule 12.3: Software Licensing
- **Rule**: Software requires hardware-locked activation.
- **Enforcement**: Activation screen on first launch.
- **Logic**: Product key is derived from a unique Hardware ID using a secret salt.

## 13. Performance and Scalability

### Rule 13.1: Pagination
- **Rule**: Large result sets must be paginated
- **Enforcement**: Application layer
- **Reason**: Performance and user experience

### Rule 13.2: Index Usage
- **Rule**: Use indexes for frequently queried columns
- **Enforcement**: Database schema
- **Monitoring**: Check slow query logs

### Rule 13.3: Archival
- **Rule**: Archive old data periodically (future)
- **Enforcement**: Application layer (future)
- **Reason**: Maintain performance as data grows

## 14. Error Handling

### Rule 14.1: User-Friendly Messages
- **Rule**: Show clear error messages to users
- **Enforcement**: Application layer
- **Example**: "Insufficient stock" instead of "FK violation"

### Rule 14.2: Logging
- **Rule**: Log all errors with context
- **Enforcement**: Application layer
- **Reason**: Debugging and monitoring

### Rule 14.3: Transaction Rollback
- **Rule**: Rollback on error to maintain consistency
- **Enforcement**: Application layer (Unit of Work pattern)
- **Reason**: Data integrity

## Conclusion

These business rules ensure:
- ✅ Data integrity and consistency
- ✅ Correct business logic execution
- ✅ Audit trail and compliance
- ✅ User-friendly error handling
- ✅ Scalability and performance
- ✅ Security and access control

All rules must be enforced at appropriate layers:
- **Database**: Constraints, triggers, foreign keys
- **Application**: Business logic, validation, transactions
- **UI**: User experience, input validation, feedback
