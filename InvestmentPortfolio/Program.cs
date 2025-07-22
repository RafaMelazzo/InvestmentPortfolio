using InvestmentPortfolio;
using Spectre.Console;

var portfolio = new Portfolio(
    "Tony Stark",
    "123.456.789-10"
);
AddSampleDataToPortfolio();

WelcomeScreen();
return;

void WelcomeScreen()
{
    string option;
    do
    {
        Console.Clear();
        AnsiConsole.MarkupLine("\n[bold green]CARTEIRA DE INVESTIMENTOS[/]");
        AnsiConsole.MarkupLine($"\nBem-vindo, [bold blue]{portfolio.GetFirstName()}[/]!");
        Console.WriteLine("Para começar, selecione uma das opções abaixo.");
        
        AnsiConsole.MarkupLine("\n[bold orange3]OPÇÕES:[/]");
        AnsiConsole.MarkupLine("[bold blue] 1[/]: Adicionar ativos na carteira");
        AnsiConsole.MarkupLine("[bold blue] 2[/]: Visualizar meus ativos");
        AnsiConsole.MarkupLine("[bold blue]99[/]: Sair");

        Console.Write("\nDigite o número da opção desejada: ");
        option = Console.ReadLine()!;

        switch (option)
        {
            case "1":
                AddAssetToPortfolio();
                break;
            case "2":
                ViewAssets();
                break;
            case "99":
                ExitProgram();
                return;
            default:
                Helper.ShowError($"Opção \"{option}\" inválida.");
                break;
        }
    } while (option != "99");
}

void AddAssetToPortfolio()
{
    Console.Clear();

    portfolio.GetInfos();
        
    AnsiConsole.Markup("\n\n[bold orange3]ADICIONAR ATIVO NA CARTEIRA:[/]\n");
    
    var assetSymbol = AnsiConsole.Prompt(
            new TextPrompt<string>("Digite o símbolo do ativo [grey](ex: STNE, AAPL, GOOGL)[/]: ")
        ).ToUpper().Trim();
    if (!StockMarket.AssetExists(assetSymbol))
    {
        Helper.ShowError($"Ativo com o símbolo \"{assetSymbol}\" não encontrado no mercado de ações.");
        return;
    }
    
    var asset = StockMarket.GetAssetBySymbol(assetSymbol);
    if (asset == null)
    {
        Helper.ShowError($"Ativo com o símbolo \"{assetSymbol}\" não encontrado.");
        return;
    }
    
    Asset.PrintAssetDetails(asset);
    
    var quantity = AnsiConsole.Prompt(
        new TextPrompt<int>("\n\nQuantas unidades você deseja adicionar? [grey](deixe em branco para 1)[/]: ")
            .DefaultValue(1)
            .Validate(
                q => q <= 0
                    ? ValidationResult.Error("A quantidade deve ser maior que zero.")
                    : ValidationResult.Success())
    );
    
    portfolio.AddAsset(asset, quantity);
    
    Helper.BackToStart();
}

void ViewAssets()
{
    Console.Clear();

    portfolio.GetInfos();
    portfolio.GetAssetsTable();
    
    Helper.BackToStart();
}

void ExitProgram()
{
    Console.Clear();
    AnsiConsole.MarkupLine("\n[bold orange3]Saindo do sistema...[/]");
    Console.WriteLine("\nObrigado!");
}

void AddSampleDataToPortfolio()
{
    var sampleAssets = StockMarket.GetAllAssets();
    foreach (var asset in sampleAssets)
    {
        var quantity = 1;
        var paidPrice = asset.CurrentPrice;
        var purchaseDate = DateTime.Now;
        
        switch (asset.Symbol)
        {
            case "STNE":
                quantity = 150;
                paidPrice = 7.45;
                purchaseDate = new DateTime(2022, 6, 1);
                break;
            case "AAPL":
                quantity = 100;
                paidPrice = 150.92;
                purchaseDate = new DateTime(2022, 1, 15);
                break;
            case "GOOGL":
                quantity = 50;
                paidPrice = 2800.50;
                purchaseDate = new DateTime(2022, 3, 10);
                break;
            case "TSLA":
                quantity = 30;
                paidPrice = 702.91;
                purchaseDate = new DateTime(2022, 5, 20);
                break;
            case "LONGNAME":
                quantity = 200;
                paidPrice = 100.00;
                purchaseDate = new DateTime(2023, 1, 1);
                break;
            case "VERYEXP":
                quantity = 10;
                paidPrice = 1000000.00;
                purchaseDate = new DateTime(2023, 2, 1);
                break;
            case "VERYNEG":
                quantity = 5;
                paidPrice = 1000000.00;
                purchaseDate = new DateTime(2023, 3, 1);
                break;
            case "BIGPROFIT":
                quantity = 20;
                paidPrice = 0.12;
                purchaseDate = new DateTime(2023, 4, 1);
                break;
            case "BIGLOSS":
                quantity = 15;
                paidPrice = 1000.00;
                purchaseDate = new DateTime(2023, 5, 1);
                break;
            case "BOUGHTTODAY":
                quantity = 10;
                paidPrice = 52.00;
                break;
            case "MINIMUM":
                // Nothing to change here. Testing with minimum data.
                break;
        }
        
        portfolio.AddAsset(
            asset,
            quantity,
            paidPrice,
            purchaseDate
        );
    }

    portfolio.AddAsset(
        StockMarket.GetAssetBySymbol("STNE")!,
        7
    );
}
