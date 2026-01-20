using System.Text;
using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;

namespace DairyManagement.Application.Services;

public class ReceiptService : IReceiptService
{
    private const int PageWidth = 32; // Standard 58mm printer width (approx 32 chars)

    public string GenerateSettlementReceipt(SettlementDto settlement)
    {
        var sb = new StringBuilder();

        // Header
        sb.AppendLine(CenterText("DIARY MANAGEMENT SYSTEM"));
        sb.AppendLine(CenterText("Settlement Receipt"));
        sb.AppendLine(new string('-', PageWidth));

        // Info
        sb.AppendLine($"Date: {settlement.SettlementDate:dd/MM/yyyy HH:mm}");
        sb.AppendLine($"Code: {settlement.CustomerId}");
        sb.AppendLine($"Name: {settlement.CustomerName}");
        sb.AppendLine($"Period: {settlement.CycleStartDate:dd/MM} - {settlement.CycleEndDate:dd/MM}");
        sb.AppendLine(new string('-', PageWidth));

        // Details Grid
        sb.AppendLine(FormatLine("Date", "Description", "Amt"));
        sb.AppendLine(new string('-', PageWidth));

        var credits = settlement.Details.Where(d => d.IsCredit).ToList();
        var debits = settlement.Details.Where(d => !d.IsCredit).ToList();

        // Credits (Milk)
        foreach (var detail in credits)
        {
            sb.AppendLine(FormatLine(
                detail.TransactionDate.ToString("dd/MM"),
                detail.Description,
                detail.Amount.ToString("N0")
            ));
        }

        if (debits.Any())
        {
            sb.AppendLine(new string('.', PageWidth));
            foreach (var detail in debits)
            {
                sb.AppendLine(FormatLine(
                    detail.TransactionDate.ToString("dd/MM"),
                    detail.Description,
                    "-" + detail.Amount.ToString("N0")
                ));
            }
        }

        sb.AppendLine(new string('=', PageWidth));

        // Totals
        sb.AppendLine(FormatTotal("Milk Total:", settlement.MilkAmount));
        sb.AppendLine(FormatTotal("Deductions:", settlement.TotalProductSales + settlement.TotalAdvancePaid));
        
        sb.AppendLine(new string('-', PageWidth));
        
        string payableLabel = settlement.CustomerOwesMoney ? "Amt Owed:" : "Net Payable:";
        sb.AppendLine(FormatTotal(payableLabel, Math.Abs(settlement.FinalPayable)));

        // Footer
        sb.AppendLine(new string('-', PageWidth));
        sb.AppendLine(CenterText("Payment: " + settlement.PaymentMode));
        if (settlement.IsPaid) sb.AppendLine(CenterText("PAID"));
        sb.AppendLine();
        sb.AppendLine(CenterText("Thank You!"));
        sb.AppendLine("\n\n\n"); // Padding for tear-off

        return sb.ToString();
    }

    private string CenterText(string text)
    {
        if (text.Length >= PageWidth) return text.Substring(0, PageWidth);
        int leftPadding = (PageWidth - text.Length) / 2;
        return new string(' ', leftPadding) + text;
    }

    private string FormatLine(string date, string desc, string amt)
    {
        // Date: 5, Amt: 7, Desc: remaining
        int dateWidth = 6;
        int amtWidth = 8;
        int descWidth = PageWidth - dateWidth - amtWidth;

        string d = date.PadRight(dateWidth);
        string a = amt.PadLeft(amtWidth);
        string ds = desc.Length > descWidth ? desc.Substring(0, descWidth) : desc.PadRight(descWidth);

        return d + ds + a;
    }

    private string FormatTotal(string label, decimal amount)
    {
        string amtStr = "Rs." + amount.ToString("N2");
        int labelWidth = PageWidth - amtStr.Length;
        return label.PadRight(labelWidth) + amtStr;
    }
}
