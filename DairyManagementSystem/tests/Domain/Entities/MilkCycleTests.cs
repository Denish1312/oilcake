using DairyManagement.Domain.Entities;
using DairyManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace DairyManagement.Tests.Domain;

public class MilkCycleTests
{
    [Fact]
    public void CanBeSettled_WhenNotSettledAndHasMilk_ShouldBeTrue()
    {
        // Arrange (private setter access via reflection or just use the mock-like approach if needed, 
        // but here we use the Create logic if available, or just standard instantiation)
        
        // Let's check how MilkCycle is created
        var cycle = MilkCycle.Create(
            customerId: 1,
            startDate: DateTime.Now.AddDays(-10),
            endDate: DateTime.Now
        );
        cycle.UpdateMilkAmount(500);

        // Act & Assert
        cycle.CanBeSettled().Should().BeTrue();
    }

    [Fact]
    public void MarkAsSettled_ShouldSetFlagToTrue()
    {
        // Arrange
        var cycle = MilkCycle.Create(1, DateTime.Now.AddDays(-10), DateTime.Now);
        
        // Act
        cycle.MarkAsSettled();

        // Assert
        cycle.IsSettled.Should().BeTrue();
        cycle.SettlementDate.Should().NotBeNull();
    }

    [Fact]
    public void CanBeSettled_WhenAlreadySettled_ShouldBeFalse()
    {
        // Arrange
        var cycle = MilkCycle.Create(1, DateTime.Now.AddDays(-10), DateTime.Now);
        cycle.UpdateMilkAmount(500);
        cycle.MarkAsSettled();

        // Act & Assert
        cycle.CanBeSettled().Should().BeFalse();
    }
}
