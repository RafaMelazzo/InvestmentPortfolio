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

    private bool IsProfit => CurrentPrice >= PaidPrice;
    
    private static string GetProfitLossArrow(double paidValue, double currentValue)
    {
        var profitLoss = currentValue - paidValue;
        return profitLoss > 0 ? "↗ " : profitLoss < 0 ? "↘ " : "→ ";
    }
    
    private static string GetProfitLossColor(double paidValue, double currentValue)
    {
        var profitLoss = currentValue - paidValue;
        return profitLoss > 0 ? "green" : profitLoss < 0 ? "red" : "silver";
    }

    private string GetProfitLoss(double paidValue, double currentValue)
    {
        var profitLoss = currentValue - paidValue;
        var arrow = GetProfitLossArrow(paidValue, currentValue);
        var value = IsProfit
            ? $"{Helper.DoubleToCurrency(profitLoss)} "
            : $"-{Helper.DoubleToCurrency(-profitLoss)} ";
        var percentage = IsProfit 
            ? $"({(Math.Abs(profitLoss) / paidValue * 100):F2}%)" 
            : $"(-{(Math.Abs(profitLoss) / paidValue * 100):F2}%)";

        return arrow + value + percentage;
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
        var color = GetProfitLossColor(PaidPrice, CurrentPrice);
        var profitLoss = GetProfitLoss(PaidPrice, CurrentPrice);
        
        table.AddRow(
            Symbol,
            Name,
            Type,
            Quantity.ToString(),
            PurchaseDate.ToString("dd/MM/yyyy"),
            $"{Helper.DoubleToCurrency(PaidPrice)}\n({Helper.DoubleToCurrency(PaidPrice * Quantity)})",
            $"{Helper.DoubleToCurrency(CurrentPrice)}\n({Helper.DoubleToCurrency(CurrentPrice * Quantity)})",
            $"[{color}]{profitLoss}[/]"
        );
    }
}
