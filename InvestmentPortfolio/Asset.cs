using Spectre.Console;

namespace InvestmentPortfolio;

public class Asset(
    string symbol,
    string name,
    string type,
    double currentPrice,
    int quantity = 1,
    double paidPrice = 0,
    DateTime purchaseDate = default
) {
    public string Symbol { get; } = symbol;
    public string Name { get; } = name;
    public string Type { get; } = type;
    public double CurrentPrice { get; } = currentPrice;
    public int Quantity { get; set; } = quantity > 0 ? quantity : 1;
    public double PaidPrice { get; set; } = paidPrice > 0 ? paidPrice : currentPrice;
    public DateTime PurchaseDate { get; set; } = purchaseDate == default ? DateTime.Now : purchaseDate;
    public double ProfitOrLoss => CurrentPrice - PaidPrice;

    private bool IsProfit => CurrentPrice >= PaidPrice;

    private string GetProfitOrLossArrow()
    {
        return ProfitOrLoss > 0 ? "↗ " : ProfitOrLoss < 0 ? "↘ " : "→ ";
    }

    private string GetProfitOrLossColor()
    {
        return ProfitOrLoss > 0 ? "green" : ProfitOrLoss < 0 ? "red" : "silver";
    }

    public string GetProfitOrLossCompleteValue(double paidValue, double currentValue)
    {
        var color = GetProfitOrLossColor();
        var arrow = GetProfitOrLossArrow();
        var value = IsProfit
            ? $"{Helper.DoubleToCurrency(ProfitOrLoss)} "
            : $"-{Helper.DoubleToCurrency(-ProfitOrLoss)} ";
        var percentage = IsProfit 
            ? $"({(Math.Abs(ProfitOrLoss) / paidValue * 100):F2}%)" 
            : $"(-{(Math.Abs(ProfitOrLoss) / paidValue * 100):F2}%)";

        return $"[{color}]{arrow + value + percentage}[/]";
    }
    
    public static void PrintAssetDetails(Asset asset)
    {
        AnsiConsole.Markup("\n[bold blue]Detalhes do Ativo:[/]");
        AnsiConsole.Markup($"\n[bold green]Ativo:[/] {asset.Symbol}");
        AnsiConsole.Markup($"\n[bold green]Nome:[/] {asset.Name}");
        AnsiConsole.Markup($"\n[bold green]Tipo:[/] {asset.Type}");
        AnsiConsole.Markup($"\n[bold green]Valor:[/] {Helper.DoubleToCurrency(asset.CurrentPrice)}");
        Console.WriteLine("\n");
    }

    public void AddRowToTable(Table table)
    {
        table.AddRow(
            Symbol,
            Name,
            Type,
            Quantity.ToString(),
            PurchaseDate.ToString("dd/MM/yyyy"),
            $"{Helper.DoubleToCurrency(PaidPrice)}\n({Helper.DoubleToCurrency(PaidPrice * Quantity)})",
            $"{Helper.DoubleToCurrency(CurrentPrice)}\n({Helper.DoubleToCurrency(CurrentPrice * Quantity)})",
            GetProfitOrLossCompleteValue(PaidPrice, CurrentPrice)
        );
    }
}
