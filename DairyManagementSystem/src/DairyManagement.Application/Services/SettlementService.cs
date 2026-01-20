using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.Domain.Entities;
using DairyManagement.Domain.Enums;
using DairyManagement.Infrastructure.Repositories;

namespace DairyManagement.Application.Services;

/// <summary>
/// Settlement service implementation
/// Handles settlement calculation and creation
/// </summary>
public class SettlementService : ISettlementService
{
    private readonly IUnitOfWork _unitOfWork;

    public SettlementService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SettlementDto> CreateSettlementAsync(CreateSettlementDto dto, string username, CancellationToken cancellationToken = default)
    {
        // Get cycle with all related data
        var cycle = await _unitOfWork.MilkCycles.GetByIdWithDetailsAsync(dto.CycleId, cancellationToken);
        if (cycle == null)
            throw new InvalidOperationException($"Milk cycle {dto.CycleId} not found");

        if (!cycle.CanBeSettled())
            throw new InvalidOperationException("Cycle cannot be settled. Either already settled or milk amount is zero");

        // Parse payment mode
        if (!Enum.TryParse<PaymentMode>(dto.PaymentMode, true, out var paymentMode))
            throw new ArgumentException($"Invalid payment mode: {dto.PaymentMode}");

        // Calculate totals
        var totalProductSales = cycle.CalculateTotalProductSales();
        var totalAdvances = cycle.CalculateTotalAdvances();

        // Create settlement
        var settlement = Settlement.Create(
            cycle.CustomerId,
            cycle.CycleId,
            cycle.TotalMilkAmount.Amount,
            totalProductSales.Amount,
            totalAdvances.Amount,
            paymentMode,
            dto.Notes
        );
        settlement.SetCreatedBy(username);

        // Add settlement details
        // 1. Milk entry (credit)
        var milkDetail = SettlementDetail.CreateMilkDetail(
            0, // Will be set by EF Core
            cycle.TotalMilkAmount.Amount,
            cycle.CycleEndDate, // Use end date as the reference point for milk
            $"Milk Amount ({cycle.DateRange.DurationInDays} days)"
        );
        settlement.AddDetail(milkDetail);

        // 2. Product sale entries (debits)
        foreach (var sale in cycle.ProductSales)
        {
            var saleDetail = SettlementDetail.CreateProductSaleDetail(
                0,
                sale.SaleId,
                $"{sale.Product.ProductName} - {sale.Quantity}",
                sale.TotalAmount.Amount,
                sale.SaleDate
            );
            settlement.AddDetail(saleDetail);
        }

        // 3. Advance entries (debits)
        foreach (var advance in cycle.AdvancePayments)
        {
            var advanceDetail = SettlementDetail.CreateAdvanceDetail(
                0,
                advance.AdvanceId,
                $"Advance on {advance.PaymentDate:dd/MM/yyyy}",
                advance.Amount.Amount,
                advance.PaymentDate
            );
            settlement.AddDetail(advanceDetail);
        }

        // Begin transaction
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Save settlement
            await _unitOfWork.Settlements.AddAsync(settlement, cancellationToken);
            
            // Mark cycle as settled
            cycle.MarkAsSettled();
            cycle.SetUpdatedBy(username);
            await _unitOfWork.MilkCycles.UpdateAsync(cycle, cancellationToken);
            
            // Commit
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Return DTO
            return await GetSettlementByIdAsync(settlement.SettlementId, cancellationToken) 
                   ?? throw new InvalidOperationException("Failed to retrieve created settlement");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<SettlementDto?> GetSettlementByIdAsync(int settlementId, CancellationToken cancellationToken = default)
    {
        var settlement = await _unitOfWork.Settlements.GetByIdWithDetailsAsync(settlementId, cancellationToken);
        return settlement == null ? null : MapToDto(settlement);
    }

    public async Task<SettlementDto?> GetSettlementByCycleIdAsync(int cycleId, CancellationToken cancellationToken = default)
    {
        var settlement = await _unitOfWork.Settlements.GetByCycleIdAsync(cycleId, cancellationToken);
        if (settlement == null) return null;
        
        // Load with details
        return await GetSettlementByIdAsync(settlement.SettlementId, cancellationToken);
    }

    public async Task<IEnumerable<SettlementDto>> GetCustomerSettlementsAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var settlements = await _unitOfWork.Settlements.GetByCustomerIdAsync(customerId, cancellationToken);
        return settlements.Select(MapToDto);
    }

    public async Task<IEnumerable<SettlementDto>> GetUnpaidSettlementsAsync(CancellationToken cancellationToken = default)
    {
        var settlements = await _unitOfWork.Settlements.GetUnpaidSettlementsAsync(cancellationToken);
        return settlements.Select(MapToDto);
    }

    public async Task MarkAsPaidAsync(int settlementId, string? paymentReference, string username, CancellationToken cancellationToken = default)
    {
        var settlement = await _unitOfWork.Settlements.GetByIdAsync(settlementId, cancellationToken);
        if (settlement == null)
            throw new InvalidOperationException($"Settlement {settlementId} not found");

        settlement.MarkAsPaid(paymentReference);
        settlement.SetUpdatedBy(username);
        
        await _unitOfWork.Settlements.UpdateAsync(settlement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<SettlementDto> PreviewSettlementAsync(int cycleId, CancellationToken cancellationToken = default)
    {
        var cycle = await _unitOfWork.MilkCycles.GetByIdWithDetailsAsync(cycleId, cancellationToken);
        if (cycle == null)
            throw new InvalidOperationException($"Milk cycle {cycleId} not found");

        var totalProductSales = cycle.CalculateTotalProductSales();
        var totalAdvances = cycle.CalculateTotalAdvances();
        var estimatedPayable = cycle.CalculateEstimatedPayable();

        return new SettlementDto
        {
            CycleId = cycle.CycleId,
            CustomerId = cycle.CustomerId,
            CustomerName = cycle.Customer.FullName,
            CycleStartDate = cycle.CycleStartDate,
            CycleEndDate = cycle.CycleEndDate,
            MilkAmount = cycle.TotalMilkAmount.Amount,
            TotalProductSales = totalProductSales.Amount,
            TotalAdvancePaid = totalAdvances.Amount,
            FinalPayable = estimatedPayable.Amount,
            CustomerOwesMoney = estimatedPayable.IsNegative
        };
    }

    public async Task RecordAdvancePaymentAsync(int customerId, int cycleId, decimal amount, string username)
    {
        var cycle = await _unitOfWork.MilkCycles.GetByIdAsync(cycleId);
        if (cycle == null) throw new InvalidOperationException("Cycle not found");
        if (cycle.IsSettled) throw new InvalidOperationException("Cycle is already settled");

        var advance = AdvancePayment.Create(customerId, cycleId, amount, PaymentMode.Cash);
        advance.SetCreatedBy(username);

        await _unitOfWork.AdvancePayments.AddAsync(advance);
        await _unitOfWork.SaveChangesAsync();
    }

    private static SettlementDto MapToDto(Settlement settlement)
    {
        return new SettlementDto
        {
            SettlementId = settlement.SettlementId,
            CustomerId = settlement.CustomerId,
            CustomerName = settlement.Customer?.FullName ?? "",
            CycleId = settlement.CycleId,
            CycleStartDate = settlement.MilkCycle?.CycleStartDate ?? DateTime.MinValue,
            CycleEndDate = settlement.MilkCycle?.CycleEndDate ?? DateTime.MinValue,
            SettlementDate = settlement.SettlementDate,
            MilkAmount = settlement.MilkAmount.Amount,
            TotalProductSales = settlement.TotalProductSales.Amount,
            TotalAdvancePaid = settlement.TotalAdvancePaid.Amount,
            FinalPayable = settlement.FinalPayable.Amount,
            PaymentMode = settlement.PaymentMode.ToString(),
            PaymentReference = settlement.PaymentReference,
            PaymentDate = settlement.PaymentDate,
            IsPaid = settlement.IsPaid,
            CustomerOwesMoney = settlement.CustomerOwesMoney(),
            Details = settlement.Details.Select(d => new SettlementDetailDto
            {
                DetailId = d.DetailId,
                DetailType = d.DetailType.ToString(),
                Description = d.Description,
                Amount = d.Amount.Amount,
                TransactionDate = d.TransactionDate,
                IsCredit = d.IsCredit()
            }).ToList()
        };
    }
}
