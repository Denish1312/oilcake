using DairyManagement.Domain.Entities;
using DairyManagement.Domain.Enums;
using DairyManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace DairyManagement.Tests.Domain;

public class SettlementTests
{
    [Fact]
    public void Create_WithValidData_ShouldCalculateCorrectFinalPayable()
    {
        // Arrange
        decimal milkAmount = 1000m;
        decimal totalSales = 200m;
        decimal totalAdvances = 100m;
        
        // Act
        var settlement = Settlement.Create(
            customerId: 1,
            cycleId: 10,
            milkAmount: milkAmount,
            totalProductSales: totalSales,
            totalAdvancePaid: totalAdvances,
            paymentMode: PaymentMode.Cash
        );

        // Assert
        settlement.FinalPayable.Amount.Should().Be(700m);
        settlement.CustomerOwesMoney().Should().BeFalse();
    }

    [Fact]
    public void Create_WhenDeductionsExceedMilk_ShouldHaveNegativePayable()
    {
        // Arrange
        decimal milkAmount = 500m;
        decimal totalSales = 400m;
        decimal totalAdvances = 200m;
        
        // Act
        var settlement = Settlement.Create(
            customerId: 1,
            cycleId: 10,
            milkAmount: milkAmount,
            totalProductSales: totalSales,
            totalAdvancePaid: totalAdvances,
            paymentMode: PaymentMode.Cash
        );

        // Assert
        settlement.FinalPayable.Amount.Should().Be(-100m);
        settlement.CustomerOwesMoney().Should().BeTrue();
        settlement.GetAmountOwed().Amount.Should().Be(100m);
    }

    [Fact]
    public void MarkAsPaid_ShouldUpdateStatusAndDate()
    {
        // Arrange
        var settlement = Settlement.Create(1, 10, 1000, 0, 0, PaymentMode.Cash);
        
        // Act
        settlement.MarkAsPaid("REF123");

        // Assert
        settlement.IsPaid.Should().BeTrue();
        settlement.PaymentReference.Should().Be("REF123");
        settlement.PaymentDate.Should().NotBeNull();
    }

    [Fact]
    public void AddDetail_ShouldIncreaseDetailsCount()
    {
        // Arrange
        var settlement = Settlement.Create(1, 10, 1000, 0, 0, PaymentMode.Cash);
        var detail = SettlementDetail.CreateMilkDetail(0, 1000, DateTime.Now);

        // Act
        settlement.AddDetail(detail);

        // Assert
        settlement.Details.Should().HaveCount(1);
    }
}
