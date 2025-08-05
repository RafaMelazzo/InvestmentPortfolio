using Spectre.Console;

namespace InvestmentPortfolio;

public abstract class Terminal
{
    private static string GetProfitOrLossArrow(Asset asset)
    {
        return asset.GetProfitOrLoss() > 0 ? "↗ " : asset.GetProfitOrLoss() < 0 ? "↘ " : "→ ";
    }

    private static string GetProfitOrLossColor(Asset asset)
    {
        return asset.GetProfitOrLoss() > 0 ? "green" : asset.GetProfitOrLoss() < 0 ? "red" : "silver";
    }

    public static string GetProfitOrLossCompleteValue(Asset asset)
    {
        var color = GetProfitOrLossColor(asset);
        var arrow = GetProfitOrLossArrow(asset);
        var value = asset.IsProfit
            ? $"{Helper.DoubleToCurrency(asset.GetProfitOrLoss())} "
            : $"-{Helper.DoubleToCurrency(-asset.GetProfitOrLoss())} ";
        var percentage = asset.IsProfit 
            ? $"({(Math.Abs(asset.GetProfitOrLoss()) / asset.GetPaidPrice() * 100):F2}%)" 
            : $"(-{(Math.Abs(asset.GetProfitOrLoss()) / asset.GetPaidPrice() * 100):F2}%)";

        return $"[{color}]{arrow + value + percentage}[/]";
    }
    
    public static void PrintAssetDetails(Asset asset)
    {
        AnsiConsole.Markup("\n[bold blue]Detalhes do Ativo:[/]");
        AnsiConsole.Markup($"\n[bold green]Ativo:[/] {asset.GetSymbol()}");
        AnsiConsole.Markup($"\n[bold green]Nome:[/] {asset.GetName()}");
        AnsiConsole.Markup($"\n[bold green]Tipo:[/] {asset.GetType()}");
        AnsiConsole.Markup($"\n[bold green]Valor:[/] {Helper.DoubleToCurrency(asset.GetCurrentPrice())}");
        Console.WriteLine("\n");
    }

    public static void AddRowToTable(Asset asset, Table table)
    {
        table.AddRow(
            asset.GetSymbol(),
            asset.GetName(),
            asset.GetType(),
            asset.GetQuantity().ToString(),
            asset.GetPurchaseDate().ToString("dd/MM/yyyy"),
            $"{Helper.DoubleToCurrency(asset.GetPaidPrice())}\n({Helper.DoubleToCurrency(asset.GetPaidPrice() * asset.GetQuantity())})",
            $"{Helper.DoubleToCurrency(asset.GetCurrentPrice())}\n({Helper.DoubleToCurrency(asset.GetCurrentPrice() * asset.GetQuantity())})",
            GetProfitOrLossCompleteValue(asset)
        );
    }
}