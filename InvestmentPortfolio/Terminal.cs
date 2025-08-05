using Spectre.Console;

namespace InvestmentPortfolio;

public abstract class Terminal
{
    private const int TableDelay = 120;
    
    private static string GetProfitOrLossArrow(Asset asset)
    {
        return asset.GetProfitOrLoss() > 0 ? "↗ " : asset.GetProfitOrLoss() < 0 ? "↘ " : "→ ";
    }

    private static string GetProfitOrLossColor(Asset asset)
    {
        return asset.GetProfitOrLoss() > 0 ? "green" : asset.GetProfitOrLoss() < 0 ? "red" : "silver";
    }

    private static string GetProfitOrLossCompleteValue(Asset asset)
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
    
    public static void PrintStockAssetDetails(Asset asset)
    {
        AnsiConsole.Markup("\n[bold blue]Detalhes do Ativo:[/]");
        AnsiConsole.Markup($"\n[bold green]Ativo:[/] {asset.GetSymbol()}");
        AnsiConsole.Markup($"\n[bold green]Nome:[/] {asset.GetName()}");
        AnsiConsole.Markup($"\n[bold green]Tipo:[/] {asset.GetType()}");
        AnsiConsole.Markup($"\n[bold green]Valor:[/] {Helper.DoubleToCurrency(asset.GetCurrentPrice())}");
        Console.WriteLine("\n");
    }

    private static void AddRowToTable(Asset asset, Table table)
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
    
    public static void GetInfos(Portfolio portfolio)
    {
        AnsiConsole.Markup($"\n[bold blue]Nome:[/] {portfolio.GetName()}" +
                           $"\n[bold blue]CPF:[/] {portfolio.GetCpf()}" +
                           $"\n\n[bold blue]Quantidade de Ativos:[/] {portfolio.GetAssetsCount()}" +
                           "\n[bold blue]Saldo Total de Ativos:[/] " +
                           $"{Helper.DoubleToCurrency(portfolio.GetAssetsTotalValue())}");
    }
    
    public static void GetBoughtAssetResponse(int quantityAdded, string assetSymbol, string assetsCost)
    {
        AnsiConsole.Markup(
            $"\nVocê comprou [bold blue]{quantityAdded}[/] unidades do ativo [bold blue]{assetSymbol}[/]" +
            $"por [bold green]{assetsCost}[/].");
    }

    public static void GetSoldAssetResponse(int quantitySold, string assetSymbol, string assetEarning)
    {
        AnsiConsole.Markup(
            $"\nVocê vendeu [bold blue]{quantitySold}[/] unidades do ativo [bold blue]{assetSymbol}[/] " +
            $"por [bold green]{assetEarning}[/].");
    }

    private static void PrintPortfolioAssetDetails(Asset asset)
    {
        AnsiConsole.Markup("\n[bold green]Detalhes do seu Ativo:[/]");
        AnsiConsole.Markup($"\n\n[bold blue]Ativo:[/] {asset.GetSymbol()}");
        AnsiConsole.Markup($"\n[bold blue]Nome:[/] {asset.GetName()}");
        AnsiConsole.Markup($"\n[bold blue]Tipo:[/] {asset.GetType()}");
        AnsiConsole.Markup($"\n[bold blue]Valor de Venda:[/] {Helper.DoubleToCurrency(asset.GetCurrentPrice())}");
        
        AnsiConsole.Markup($"\n\n[bold blue]Data da Compra:[/] {asset.GetPurchaseDate():dd/MM/yyyy}");
        AnsiConsole.Markup($"\n[bold blue]Data da Compra:[/] {asset.GetPurchaseDate():dd/MM/yyyy}");
        AnsiConsole.Markup(
            $"\n[bold blue]Valor Pago na Compra:[/] {Helper.DoubleToCurrency(asset.GetPaidPrice())}");
        AnsiConsole.Markup(
            $"\n[bold blue]Lucro/Prejuízo:[/] " +
            $"{GetProfitOrLossCompleteValue(asset)}");
        
        AnsiConsole.Markup($"\n\n[bold blue]Quantidade:[/] {asset.GetQuantity()}");
    }
    
    public static void PrintSameAssetListDetails(List<Asset> assets)
    {
        switch (assets.Count)
        {
            case <0:
                throw new ArgumentException("Asset list cannot be null or empty.", nameof(assets));
            case 0:
                Helper.ShowError("Nenhum ativo encontrado.");
                return;
            case 1:
                PrintPortfolioAssetDetails(assets.First());
                return;
            default:
                var firstAsset = assets.First();
                AnsiConsole.Markup("\n[bold green]Detalhes dos seus Ativos:[/]");
                AnsiConsole.Markup($"\n\n[bold blue]Ativo:[/] {firstAsset.GetSymbol()}");
                AnsiConsole.Markup($"\n[bold blue]Nome:[/] {firstAsset.GetName()}");
                AnsiConsole.Markup($"\n[bold blue]Tipo:[/] {firstAsset.GetType()}");
                AnsiConsole.Markup(
                    $"\n[bold blue]Valor de Venda:[/] {Helper.DoubleToCurrency(firstAsset.GetCurrentPrice())}");
                
                foreach (var asset in assets)
                {
                    AnsiConsole.Markup($"\n\n[bold blue]Data da Compra:[/] {asset.GetPurchaseDate():dd/MM/yyyy}");
                    AnsiConsole.Markup(
                        $"\n[bold blue]Valor Pago na Compra:[/] {Helper.DoubleToCurrency(asset.GetPaidPrice())}");
                    AnsiConsole.Markup($"\n[bold blue]Quantidade:[/] {asset.GetQuantity()}");
                    AnsiConsole.Markup(
                        $"\n[bold blue]Lucro/Prejuízo:[/] " +
                        $"{GetProfitOrLossCompleteValue(asset)}");
                }
                
                AnsiConsole.Markup(
                    $"\n\n\n[bold green]Quantidade Total de Ativos[/] [bold blue]{firstAsset.GetSymbol()}[/] " +
                    $"[bold green]disponíveis para venda:[/] {assets.Sum(a => a.GetQuantity())}"
                );
                break;
        }
    }

    public static void GetAssetsTable(Portfolio portfolio)
    {
        var defaultConsoleWidth = AnsiConsole.Console.Profile.Width;
        AnsiConsole.Console.Profile.Width = 140;
        
        AnsiConsole.Markup("\n\n[bold orange3]SUA CARTEIRA DE ATIVOS:[/]\n");
        
        if (portfolio.GetAssetsCount() == 0)
        {
            AnsiConsole.Markup("[bold red]Nenhum ativo encontrado[/]\n");
            return;
        }

        var assets = portfolio.GetAssets().OrderBy(a => a.GetSymbol());
        var assetsTable = new Table()
            .Border(TableBorder.HeavyHead)
            .ShowRowSeparators()
            .ShowFooters()
            .Expand();
        
        AnsiConsole.Live(assetsTable)
            .Start(ctx => 
            {
                assetsTable.AddColumn("[bold blue]Código[/]");
                assetsTable.AddColumn("[bold blue]Nome[/]");
                assetsTable.AddColumn("[bold blue]Tipo[/]");
                assetsTable.AddColumn("[bold blue]Quantidade[/]");
                assetsTable.AddColumn("[bold blue]Data da Compra[/]");
                assetsTable.AddColumn("[bold blue]Valor Pago[/]");
                assetsTable.AddColumn("[bold blue]Valor Atual[/]");
                assetsTable.AddColumn("[bold blue]Lucro/Prejuízo[/]");
        
                assetsTable.Columns[0]
                    .Width(6).NoWrap().Footer("[bold blue]TOTAL[/]");
                assetsTable.Columns[2]
                    .Width(10).NoWrap();
                assetsTable.Columns[3]
                    .Width(6).NoWrap().RightAligned().Footer($"[bold blue]{portfolio.GetQuantity()}[/]");
                assetsTable.Columns[4]
                    .Width(15).NoWrap();
                assetsTable.Columns[5]
                    .Width(14).NoWrap().RightAligned()
                    .Footer($"[bold blue]{Helper.DoubleToCurrency(portfolio.GetPaidTotal())}[/]");
                assetsTable.Columns[6]
                    .Width(14).NoWrap().RightAligned()
                    .Footer($"[bold blue]{Helper.DoubleToCurrency(portfolio.GetAssetsTotalValue())}[/]");
                assetsTable.Columns[7]
                    .Width(28).NoWrap();
                assetsTable.Columns[1]
                    .Width(Helper.GetDynamicColumnWidth(assetsTable, 1)).NoWrap();
                
                ctx.Refresh();
                Thread.Sleep(TableDelay);
                
                foreach (var asset in assets)
                {
                    Terminal.AddRowToTable(asset, assetsTable);
                    ctx.Refresh();
                    Thread.Sleep(TableDelay);
                }
            });
        
        AnsiConsole.Console.Profile.Width = defaultConsoleWidth;
    }
}