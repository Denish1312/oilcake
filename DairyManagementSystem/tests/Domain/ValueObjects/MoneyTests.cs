using DairyManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace DairyManagement.Tests.Domain;

public class MoneyTests
{
    [Fact]
    public void Create_WithPositiveAmount_ShouldSetAmount()
    {
        var money = Money.FromAmount(100.50m);
        money.Amount.Should().Be(100.50m);
        money.IsNegative.Should().BeFalse();
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldSetAmountAndNegativeFlag()
    {
        var money = Money.FromBalance(-50.25m);
        money.Amount.Should().Be(-50.25m);
        money.IsNegative.Should().BeTrue();
    }

    [Fact]
    public void Add_TwoMoneyObjects_ShouldSumAmounts()
    {
        var m1 = Money.FromAmount(100);
        var m2 = Money.FromAmount(50);
        
        var result = m1 + m2;
        
        result.Amount.Should().Be(150);
    }

    [Fact]
    public void Subtract_TwoMoneyObjects_ShouldSubtractAmounts()
    {
        var m1 = Money.FromAmount(100);
        var m2 = Money.FromAmount(30);
        
        var result = m1 - m2;
        
        result.Amount.Should().Be(70);
    }

    [Fact]
    public void Equals_SameAmount_ShouldBeTrue()
    {
        var m1 = Money.FromAmount(100);
        var m2 = Money.FromAmount(100);
        
        m1.Should().Be(m2);
    }

    [Fact]
    public void ToString_ShouldFormatCurrency()
    {
        var money = Money.FromAmount(1234.56m);
        money.ToString().Should().Contain("1,234.56");
    }
}
