using DairyManagement.Application.DTOs;
using DairyManagement.Application.Services;
using FluentAssertions;
using Xunit;

namespace DairyManagement.Tests.Application;

public class ReceiptServiceTests
{
    [Fact]
    public void GenerateSettlementReceipt_ShouldContainKeyInformation()
    {
        // Arrange
        var service = new ReceiptService();
        var settlement = new SettlementDto
        {
            CustomerId = 101,
            CustomerName = "John Doe",
            SettlementDate = new DateTime(2026, 1, 19),
            CycleStartDate = new DateTime(2026, 1, 1),
            CycleEndDate = new DateTime(2026, 1, 10),
            MilkAmount = 5000,
            TotalProductSales = 1000,
            TotalAdvancePaid = 500,
            FinalPayable = 3500,
            PaymentMode = "Cash",
            Details = new List<SettlementDetailDto>
            {
                new() { Description = "Milk Amount", Amount = 5000, IsCredit = true, TransactionDate = new DateTime(2026, 1, 10) },
                new() { Description = "Oil Cake", Amount = 1000, IsCredit = false, TransactionDate = new DateTime(2026, 1, 5) },
                new() { Description = "Advance", Amount = 500, IsCredit = false, TransactionDate = new DateTime(2026, 1, 7) }
            }
        };

        // Act
        var receipt = service.GenerateSettlementReceipt(settlement);

        // Assert
        receipt.Should().Contain("DIARY MANAGEMENT SYSTEM");
        receipt.Should().Contain("John Doe");
        receipt.Should().Contain("3,500.00"); // Final Payable
        receipt.Should().Contain("05/01"); // Date for Oil Cake
        receipt.Should().Contain("07/01"); // Date for Advance
    }
}
