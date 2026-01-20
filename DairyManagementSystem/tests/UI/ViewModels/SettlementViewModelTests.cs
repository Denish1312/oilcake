using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.UI.ViewModels;
using DairyManagement.UI.Services;
using Moq;
using Xunit;
using FluentAssertions;

namespace DairyManagement.Tests.UI;

public class SettlementViewModelTests
{
    private readonly Mock<ISettlementService> _settlementServiceMock;
    private readonly Mock<IDialogService> _dialogServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<IReceiptService> _receiptServiceMock;
    private readonly Mock<IPrintService> _printServiceMock;

    public SettlementViewModelTests()
    {
        _settlementServiceMock = new Mock<ISettlementService>();
        _dialogServiceMock = new Mock<IDialogService>();
        _navigationServiceMock = new Mock<INavigationService>();
        _receiptServiceMock = new Mock<IReceiptService>();
        _printServiceMock = new Mock<IPrintService>();
    }

    [Fact]
    public void Constructor_ShouldInitializeCommands()
    {
        // Act
        var vm = CreateViewModel();

        // Assert
        vm.PreviewCommand.Should().NotBeNull();
        vm.CreateSettlementCommand.Should().NotBeNull();
        vm.PrintReceiptCommand.Should().NotBeNull();
    }

    [Fact]
    public async Task PreviewAsync_ShouldUpdatePreviewProperty()
    {
        // Arrange
        var settlementDto = new SettlementDto { CustomerName = "Test" };
        _settlementServiceMock.Setup(s => s.PreviewSettlementAsync(It.IsAny<int>(), default))
            .ReturnsAsync(settlementDto);
        
        var vm = CreateViewModel();
        vm.SetMilkCycle(1);

        // Act - SetMilkCycle calls PreviewAsync
        // We might need to wait for the async operation since it's fire-and-forget in SetMilkCycle
        await Task.Delay(100); 

        // Assert
        vm.Preview.Should().Be(settlementDto);
    }

    [Fact]
    public async Task PrintReceiptAsync_WhenPreviewIsNull_ShouldNotPrint()
    {
        // Arrange
        var vm = CreateViewModel();
        
        // Act
        await (vm.PrintReceiptCommand as dynamic).ExecuteAsync(null);

        // Assert
        _printServiceMock.Verify(p => p.PrintRawTextAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PrintReceiptAsync_WithPreview_ShouldCallPrintService()
    {
        // Arrange
        var settlementDto = new SettlementDto { CustomerName = "Test" };
        var vm = CreateViewModel();
        vm.Preview = settlementDto;
        
        _receiptServiceMock.Setup(r => r.GenerateSettlementReceipt(settlementDto))
            .Returns("RECEIPT_TEXT");

        // Act
        await (vm.PrintReceiptCommand as dynamic).ExecuteAsync(null);

        // Assert
        _printServiceMock.Verify(p => p.PrintRawTextAsync("RECEIPT_TEXT"), Times.Once);
    }

    private SettlementViewModel CreateViewModel()
    {
        return new SettlementViewModel(
            _settlementServiceMock.Object,
            _dialogServiceMock.Object,
            _navigationServiceMock.Object,
            _receiptServiceMock.Object,
            _printServiceMock.Object
        );
    }
}
