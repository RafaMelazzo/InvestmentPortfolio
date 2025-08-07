using Spectre.Console;

namespace InvestmentPortfolio;

public abstract class Terminal
{
    private const int TableDelay = 120;

    public static void WelcomeScreen(Portfolio portfolio)
    {
        string option;
        do
        {
            Console.Clear();
            AnsiConsole.MarkupLine("\n[bold green]CARTEIRA DE INVESTIMENTOS[/]");
            AnsiConsole.MarkupLine($"\nBem-vindo, [bold blue]{portfolio.GetFirstName()}[/]!");
            Console.WriteLine("Para começar, selecione uma das opções abaixo.");
            
            AnsiConsole.MarkupLine("\n[bold orange3]OPÇÕES:[/]");
            AnsiConsole.MarkupLine("[bold blue] 1[/]: Visualizar ativos da minha carteira");
            AnsiConsole.MarkupLine("[bold blue] 2[/]: Comprar ativos do mercado de ações");
            AnsiConsole.MarkupLine("[bold blue] 3[/]: Vender ativos da minha carteira");
            AnsiConsole.MarkupLine("[bold blue]99[/]: Sair");

            Console.Write("\nDigite o número da opção desejada: ");
            option = Console.ReadLine()!;

            switch (option)
            {
                case "1":
                    ViewAssets(portfolio);
                    break;
                case "2":
                    BuyAssetInPortfolio(portfolio);
                    break;
                case "3":
                    SellAssetFromPortfolio(portfolio);
                    break;
                case "99":
                    ExitProgram();
                    return;
                default:
                    TerminalHelper.ShowError($"Opção \"{option}\" inválida.");
                    break;
            }
        } while (option != "99");
    }

    private static void ViewAssets(Portfolio portfolio)
    {
        Console.Clear();

        GetInfos(portfolio);
        GetAssetsTable(portfolio);
        
        TerminalHelper.BackToStart();
    }

    private static void BuyAssetInPortfolio(Portfolio portfolio)
    {
        Console.Clear();

        GetInfos(portfolio);
            
        AnsiConsole.Markup("\n\n[bold orange3]COMPRAR ATIVOS:[/]\n");
        
        var assetSymbol = AnsiConsole.Prompt(
                new TextPrompt<string>("Digite o símbolo do ativo [grey](ex: STNE, AAPL, GOOGL)[/]: ")
            ).ToUpper().Trim();
        if (!StockMarket.AssetExists(assetSymbol))
        {
            TerminalHelper.ShowError($"Ativo com o símbolo \"{assetSymbol}\" não encontrado no mercado de ações.");
            return;
        }
        
        var asset = StockMarket.GetAssetBySymbol(assetSymbol);
        if (asset == null)
        {
            TerminalHelper.ShowError($"Ativo com o símbolo \"{assetSymbol}\" não encontrado.");
            return;
        }
        
        PrintStockAssetDetails(asset);
        
        var quantity = AnsiConsole.Prompt(
            new TextPrompt<int>("\n\nQuantas unidades você deseja adicionar? [grey](deixe em branco para 1)[/]: ")
                .DefaultValue(1)
                .Validate(
                    q => q <= 0
                        ? ValidationResult.Error("[red]A quantidade deve ser maior que zero.[/]")
                        : ValidationResult.Success())
        );
        
        portfolio.AddAsset(asset, quantity);
        
        TerminalHelper.BackToStart();
    }

    private static void SellAssetFromPortfolio(Portfolio portfolio)
    {
        Console.Clear();

        GetInfos(portfolio);
            
        AnsiConsole.Markup("\n\n[bold orange3]VENDER ATIVOS:[/]\n");
        
        if (portfolio.GetAssetsCount() == 0)
        {
            AnsiConsole.Markup("[bold red]Nenhum ativo encontrado[/]\n");
            return;
        }
        
        var assetSymbol = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecione o ativo que deseja vender:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Use as setas para cima/baixo para exibir mais opções)[/]")
                    .AddChoices(
                        portfolio.GetAssets()
                            .Select(a => a.GetSymbol())
                            .Distinct()
                            .OrderBy(s => s)
                            .ToList()
                    )
            );
        
        var assets = portfolio.GetAllAssetsWithSameSymbol(assetSymbol);
        PrintSameAssetListDetails(assets);

        Console.WriteLine("\n");
        
        var quantity = AnsiConsole.Prompt(
            new TextPrompt<int>("Quantas unidades você deseja vender? [grey](deixe em branco para 1)[/]: ")
                .DefaultValue(1)
                .Validate(
                    q => q <= 0
                        ? ValidationResult.Error("[red]A quantidade deve ser maior que zero.[/]")
                        : q > assets.Sum(a => a.GetQuantity())
                        ? ValidationResult.Error($"[red]Você não possui[/] [bold blue]{q}[/] [red]unidades deste ativo.[/]")
                        : ValidationResult.Success())
        );
        
        portfolio.SellAsset(assets, quantity);
        
        TerminalHelper.BackToStart();
    }

    private static void ExitProgram()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("\n[bold orange3]Saindo do sistema...[/]");
        Console.WriteLine("\nObrigado!");
    }
    
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

    private static void PrintStockAssetDetails(Asset asset)
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

    private static void GetInfos(Portfolio portfolio)
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
            $"\nVocê comprou [bold blue]{quantityAdded}[/] unidades do ativo [bold blue]{assetSymbol}[/] " +
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

    private static void PrintSameAssetListDetails(List<Asset> assets)
    {
        switch (assets.Count)
        {
            case <0:
                throw new ArgumentException("Asset list cannot be null or empty.", nameof(assets));
            case 0:
                TerminalHelper.ShowError("Nenhum ativo encontrado.");
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

    private static void GetAssetsTable(Portfolio portfolio)
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
                    AddRowToTable(asset, assetsTable);
                    ctx.Refresh();
                    Thread.Sleep(TableDelay);
                }
            });
        
        AnsiConsole.Console.Profile.Width = defaultConsoleWidth;
    }
}