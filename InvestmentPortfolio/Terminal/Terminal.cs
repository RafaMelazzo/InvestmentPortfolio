using InvestmentPortfolio.Exceptions;
using InvestmentPortfolio.Services;
using Spectre.Console;
using ArgumentException = InvestmentPortfolio.Exceptions.ArgumentException;

namespace InvestmentPortfolio.Terminal;

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
            AnsiConsole.MarkupLine($"\nBem-vindo, [bold blue]{portfolio.Person.GetFirstName()}[/]!");
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

        GetAccountInfo(portfolio);
        GetAssetsTable(portfolio);
        
        TerminalHelper.BackToStart();
    }

    private static void BuyAssetInPortfolio(Portfolio portfolio)
    {
        Console.Clear();

        GetAccountInfo(portfolio);
            
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

        try
        {
            portfolio.AddAsset(asset, quantity);
        }
        catch (PortfolioException e)
        {
            TerminalHelper.ShowError(e.Message);
        }
        catch (Exception)
        {
            TerminalHelper.ShowError("Ocorreu um erro inesperado ao adicionar um ativo ao seu portfolio.");
        }
        
        TerminalHelper.BackToStart();
    }

    private static void SellAssetFromPortfolio(Portfolio portfolio)
    {
        Console.Clear();

        GetAccountInfo(portfolio);
            
        AnsiConsole.Markup("\n\n[bold orange3]VENDER ATIVOS:[/]\n");
        
        if (portfolio.Assets.Count == 0)
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
                        portfolio.Assets
                            .Select(a => a.Symbol)
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
                        : q > assets.Sum(a => a.Quantity)
                        ? ValidationResult.Error($"[red]Você não possui[/] [bold blue]{q}[/] [red]unidades deste ativo.[/]")
                        : ValidationResult.Success())
        );

        try
        {
            portfolio.SellAsset(assets, quantity);
        }
        catch (PortfolioException e)
        {
            TerminalHelper.ShowError(e.Message);
        }
        catch (Exception)
        {
            TerminalHelper.ShowError("Ocorreu um erro inesperado ao vender o ativo.");
        }
        
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
        var value = asset.IsProfit()
            ? $"{Helpers.Helper.DoubleToCurrency(asset.GetProfitOrLoss())} "
            : $"-{Helpers.Helper.DoubleToCurrency(-asset.GetProfitOrLoss())} ";
        var percentage = asset.IsProfit() 
            ? $"({(Math.Abs(asset.GetProfitOrLoss()) / asset.PaidPrice * 100):F2}%)" 
            : $"(-{(Math.Abs(asset.GetProfitOrLoss()) / asset.PaidPrice * 100):F2}%)";

        return $"[{color}]{arrow + value + percentage}[/]";
    }

    private static void PrintStockAssetDetails(Asset asset)
    {
        AnsiConsole.Markup("\n[bold blue]Detalhes do Ativo:[/]");
        AnsiConsole.Markup($"\n[bold green]Ativo:[/] {asset.Symbol}");
        AnsiConsole.Markup($"\n[bold green]Nome:[/] {asset.Name}");
        AnsiConsole.Markup($"\n[bold green]Tipo:[/] {asset.Type}");
        AnsiConsole.Markup($"\n[bold green]Valor:[/] {Helpers.Helper.DoubleToCurrency(asset.CurrentPrice)}");
        Console.WriteLine("\n");
    }

    private static void AddRowToTable(Asset asset, Table table)
    {
        table.AddRow(
            asset.Symbol,
            asset.Name,
            asset.Type,
            asset.Quantity.ToString(),
            asset.PurchaseDate.ToString("dd/MM/yyyy"),
            $"{Helpers.Helper.DoubleToCurrency(asset.PaidPrice)}" +
                $"\n({Helpers.Helper.DoubleToCurrency(asset.PaidPrice * asset.Quantity)})",
            $"{Helpers.Helper.DoubleToCurrency(asset.CurrentPrice)}" +
                $"\n({Helpers.Helper.DoubleToCurrency(asset.CurrentPrice * asset.Quantity)})",
            GetProfitOrLossCompleteValue(asset)
        );
    }

    private static void GetAccountInfo(Portfolio portfolio)
    {
        AnsiConsole.Markup($"\n[bold blue]Nome:[/] {portfolio.Person.Name}" +
                           $"\n[bold blue]{portfolio.Person.DocumentType}:[/] " +
                           $"{portfolio.Person.GetFormatedDocument(portfolio.Person.Document)}" +
                           $"\n[bold blue]E-mail:[/] {portfolio.Person.Email}" +
                           
                           $"\n\n[bold blue]Quantidade de Ativos:[/] {portfolio.Assets.Count}" +
                           "\n[bold blue]Saldo Total de Ativos:[/] " +
                           $"{Helpers.Helper.DoubleToCurrency(portfolio.GetAssetsTotalValue())}" +
                           "\n[bold blue]Saldo da Carteira:[/] " +
                           $"{Helpers.Helper.DoubleToCurrency(portfolio.WalletBalance)}\n");
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
        AnsiConsole.Markup($"\n\n[bold blue]Ativo:[/] {asset.Symbol}");
        AnsiConsole.Markup($"\n[bold blue]Nome:[/] {asset.Name}");
        AnsiConsole.Markup($"\n[bold blue]Tipo:[/] {asset.Type}");
        AnsiConsole.Markup($"\n[bold blue]Valor de Venda:[/] {Helpers.Helper.DoubleToCurrency(asset.CurrentPrice)}");
        
        AnsiConsole.Markup($"\n\n[bold blue]Data da Compra:[/] {asset.PurchaseDate:dd/MM/yyyy}");
        AnsiConsole.Markup($"\n[bold blue]Data da Compra:[/] {asset.PurchaseDate:dd/MM/yyyy}");
        AnsiConsole.Markup(
            $"\n[bold blue]Valor Pago na Compra:[/] {Helpers.Helper.DoubleToCurrency(asset.PaidPrice)}");
        AnsiConsole.Markup(
            $"\n[bold blue]Lucro/Prejuízo:[/] " +
            $"{GetProfitOrLossCompleteValue(asset)}");
        
        AnsiConsole.Markup($"\n\n[bold blue]Quantidade:[/] {asset.Quantity}");
    }

    private static void PrintSameAssetListDetails(List<Asset> assets)
    {
        switch (assets.Count)
        {
            case <0:
                throw new ArgumentException("Asset list cannot be null or empty.");
            case 0:
                TerminalHelper.ShowError("Nenhum ativo encontrado.");
                return;
            case 1:
                PrintPortfolioAssetDetails(assets.First());
                return;
            default:
                var firstAsset = assets.First();
                AnsiConsole.Markup("\n[bold green]Detalhes dos seus Ativos:[/]");
                AnsiConsole.Markup($"\n\n[bold blue]Ativo:[/] {firstAsset.Symbol}");
                AnsiConsole.Markup($"\n[bold blue]Nome:[/] {firstAsset.Name}");
                AnsiConsole.Markup($"\n[bold blue]Tipo:[/] {firstAsset.Type}");
                AnsiConsole.Markup(
                    $"\n[bold blue]Valor de Venda:[/] {Helpers.Helper.DoubleToCurrency(firstAsset.CurrentPrice)}");
                
                foreach (var asset in assets)
                {
                    AnsiConsole.Markup($"\n\n[bold blue]Data da Compra:[/] {asset.PurchaseDate:dd/MM/yyyy}");
                    AnsiConsole.Markup(
                        $"\n[bold blue]Valor Pago na Compra:[/] {Helpers.Helper.DoubleToCurrency(asset.PaidPrice)}");
                    AnsiConsole.Markup($"\n[bold blue]Quantidade:[/] {asset.Quantity}");
                    AnsiConsole.Markup(
                        $"\n[bold blue]Lucro/Prejuízo:[/] " +
                        $"{GetProfitOrLossCompleteValue(asset)}");
                }
                
                AnsiConsole.Markup(
                    $"\n\n\n[bold green]Quantidade Total de Ativos[/] [bold blue]{firstAsset.Symbol}[/] " +
                    $"[bold green]disponíveis para venda:[/] {assets.Sum(a => a.Quantity)}"
                );
                break;
        }
    }

    private static void GetAssetsTable(Portfolio portfolio)
    {
        var defaultConsoleWidth = AnsiConsole.Console.Profile.Width;
        AnsiConsole.Console.Profile.Width = 140;
        
        AnsiConsole.Markup("\n\n[bold orange3]SUA CARTEIRA DE ATIVOS:[/]\n");
        
        if (portfolio.Assets.Count == 0)
        {
            AnsiConsole.Markup("[bold red]Nenhum ativo encontrado[/]\n");
            return;
        }

        var assets = portfolio.Assets.OrderBy(a => a.Symbol);
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
                    .Width(6).NoWrap().RightAligned().Footer($"[bold blue]{portfolio.GetAssetsTotalQuantity()}[/]");
                assetsTable.Columns[4]
                    .Width(15).NoWrap();
                assetsTable.Columns[5]
                    .Width(14).NoWrap().RightAligned()
                    .Footer($"[bold blue]{Helpers.Helper.DoubleToCurrency(portfolio.GetAssetsTotalPaidValue())}[/]");
                assetsTable.Columns[6]
                    .Width(14).NoWrap().RightAligned()
                    .Footer($"[bold blue]{Helpers.Helper.DoubleToCurrency(portfolio.GetAssetsTotalValue())}[/]");
                assetsTable.Columns[7]
                    .Width(28).NoWrap();
                assetsTable.Columns[1]
                    .Width(TerminalHelper.GetDynamicColumnWidth(assetsTable, 1)).NoWrap();
                
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