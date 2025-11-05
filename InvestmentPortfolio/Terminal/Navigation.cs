using InvestmentPortfolio.Exceptions;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Services;
using Spectre.Console;
using ArgumentException = InvestmentPortfolio.Exceptions.ArgumentException;
using ArgumentOutOfRangeException = InvestmentPortfolio.Exceptions.ArgumentOutOfRangeException;

namespace InvestmentPortfolio.Terminal;

public abstract class Navigation
{
    private const int TableDelay = 60;
    
    public static void LoginScreen()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("\n[bold green]CARTEIRA DE INVESTIMENTOS[/]");
        AnsiConsole.MarkupLine("\n[bold blue]Bem-vindo![/]");
        Console.WriteLine("Para começar, por favor, faça o login ou crie sua conta.");
        
        string option;
        do
        {
            AnsiConsole.MarkupLine("\n[bold orange3]OPÇÕES:[/]");
            AnsiConsole.MarkupLine("[bold blue] 1[/]: Login");
            AnsiConsole.MarkupLine("[bold blue] 2[/]: Criar Conta");
            AnsiConsole.MarkupLine("[bold blue]99[/]: Encerrar Programa");
            
            Console.Write("\nDigite o número da opção desejada: ");
            option = Console.ReadLine()!;
            
            switch (option)
            {
                case "1":
                    Login();
                    break;
                case "2":
                    CreateAccount();
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

    private static void Login()
    {
        var examplePortfolio = new Portfolio(
            new User(
                new Person(
                    "Tony Stark",
                    "35374527215", // Random valid CPF for testing purposes
                    "tony@starkindustries.com"
                ),
                "I'mIronMan!2025"
            ),
            new Wallet(15213993.22)
        );
        AddSampleDataToPortfolio(examplePortfolio);
        
        Console.Clear();
        AnsiConsole.MarkupLine("\n[bold green]CARTEIRA DE INVESTIMENTOS[/]");
        AnsiConsole.MarkupLine("\n[bold blue]Bem-vindo de volta![/]");
        Console.WriteLine("\nPor favor, insira suas credenciais para fazer o login.");
        AnsiConsole.MarkupLine("[gray]Utilize os seguintes dados para teste:[/]" +
                               "\n[bold gray]Documento:[/] [gray]353.745.272-15[/]" +
                               "\n[bold gray]Senha:[/] [gray]I'mIronMan!2025[/]\n");
        
        var document = AnsiConsole.Prompt(
            new TextPrompt<string>("Documento [gray](CPF ou CNPJ)[/]: ")
                .Validate(
                    d => examplePortfolio.User.Person.Document
                         != Helpers.CustomRegex.AnyNonDigitRegex().Replace(d, "")
                    ? ValidationResult.Error(
                        "[red]Documento inválido ou não encontrado no sistema. Tente novamente.[/]")
                    : ValidationResult.Success())
        );
        
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("Senha: ")
                .Secret()
                .Validate(s => examplePortfolio.User.Password != s
                    ? ValidationResult.Error("[red]Senha incorreta. Tente novamente.[/]")
                    : ValidationResult.Success())
        );

        if (examplePortfolio.User.Person.Document
                == Helpers.CustomRegex.AnyNonDigitRegex().Replace(document, "")
            && examplePortfolio.User.Password == password)
        {
            WelcomeScreen(examplePortfolio);
            return;
        }
        
        Helper.ShowError("Ocorreu um erro inesperado ao fazer o login. Pressione qualquer tecla " +
                         "para retornar à tela de login.");
        Console.ReadKey();
        LoginScreen();
    }

    private static void CreateAccount()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("\n[bold green]CARTEIRA DE INVESTIMENTOS[/]");
        AnsiConsole.MarkupLine("\n[bold blue]Seja bem-vindo![/]");
        Console.WriteLine("Por favor, insira seus dados para criar a conta.");
        
        var name = AnsiConsole.Prompt(
            new TextPrompt<string>("\n\nNome: ")
                .Validate(n => string.IsNullOrWhiteSpace(n)
                    ? ValidationResult.Error("[red]O nome não pode estar vazio.[/]")
                    : ValidationResult.Success())
        );

        var document = AnsiConsole.Prompt(
            new TextPrompt<string>("Documento [gray](CPF ou CNPJ)[/]: ")
                .Validate(d => !Person.IsValidDocument(d)
                    ? ValidationResult.Error(
                        "[red]Documento inválido. Tente novamente[/]")
                    : ValidationResult.Success())
        );
        
        var email = AnsiConsole.Prompt(
            new TextPrompt<string>("E-mail: ")
                .Validate(e => !Person.IsValidEmail(e)
                    ? ValidationResult.Error("[red]E-mail inválido. Tente novamente.[/]")
                    : ValidationResult.Success())
        );
        
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("Senha: ")
                .Secret()
                .Validate(p => !User.ValidatePassword(p)
                    ? ValidationResult.Error(
                        "[red]A senha deve ter entre 8 e 15 caracteres, contendo letras minúsculas e maiúsculas, " +
                        "além de números e caracteres especiais.[/]")
                    : ValidationResult.Success())
        );

        var initialBalance = AnsiConsole.Prompt(
            new TextPrompt<double>("Adicione fundos para comprar ações " +
                                   "[gray]Deixe em branco para não adicionar no momento[/]: ")
                .Validate(d => d < 0
                    ? ValidationResult.Error("[red]O valor do depósito não pode ser negativo.[/]")
                    : ValidationResult.Success())
                .DefaultValue(0)
        );
        
        try
        {
            var person = new Person(name, document, email);
            var user = new User(person, password);
            var wallet = new Wallet(initialBalance);
            var portfolio = new Portfolio(user, wallet);
        
            AnsiConsole.MarkupLine("\n\n[green]Conta criada com sucesso![/]");
            Console.WriteLine("\n\nPressione qualquer tecla para acessar o sistema.");
            Console.ReadKey();
            WelcomeScreen(portfolio);
        }
        catch (ValidationException e)
        {
            Helper.ShowError(e.Message + "\n\nPressione qualquer tecla para retornar à tela de login.");
            Console.ReadKey();
            LoginScreen();
        }
        catch (Exception)
        {
            Helper.ShowError("Ocorreu um erro inesperado ao criar sua conta. Pressione qualquer tecla " +
                             "para retornar à tela de login.");
            Console.ReadKey();
            LoginScreen();
        }
    }

    internal static void WelcomeScreen(Portfolio portfolio)
    {
        string option;
        do
        {
            Console.Clear();
            AnsiConsole.MarkupLine("\n[bold green]CARTEIRA DE INVESTIMENTOS[/]");
            AnsiConsole.MarkupLine($"\nBem-vindo, [bold blue]{portfolio.User.Person.GetFirstName()}[/]!");
            Console.WriteLine("Para começar, selecione uma das opções abaixo.");
            
            AnsiConsole.MarkupLine("\n[bold orange3]OPÇÕES:[/]");
            AnsiConsole.MarkupLine("[bold blue] 1[/]: Visualizar ativos da minha carteira");
            AnsiConsole.MarkupLine("[bold blue] 2[/]: Comprar ativos do mercado de ações");
            AnsiConsole.MarkupLine("[bold blue] 3[/]: Vender ativos da minha carteira");
            AnsiConsole.MarkupLine("[bold blue] 4[/]: Visualizar meu perfil");
            AnsiConsole.MarkupLine("[bold blue]99[/]: Deslogar do sistema");

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
                case "4":
                    ViewProfile(portfolio);
                    break;
                case "99":
                    Logout();
                    return;
                default:
                    Helper.ShowError($"Opção \"{option}\" inválida.");
                    break;
            }
        } while (option != "99");
    }

    private static void ViewAssets(Portfolio portfolio)
    {
        Console.Clear();

        GetAccountInfo(portfolio);
        GetAssetsTable(portfolio);
        
        Helper.BackToStart();
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
            Helper.ShowError($"Ativo com o símbolo \"{assetSymbol}\" não encontrado no mercado de ações.");
            return;
        }
        
        var asset = StockMarket.GetAssetBySymbol(assetSymbol);
        if (asset == null)
        {
            Helper.ShowError($"Ativo com o símbolo \"{assetSymbol}\" não encontrado.");
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
            portfolio.BuyAsset(asset, quantity);
            GetBoughtAssetResponse(
                quantity,
                asset.Symbol,
                Helpers.Currency.DoubleToCurrency(asset.CurrentPrice * quantity));
        }
        catch (PortfolioException e)
        {
            Helper.ShowError(e.Message);
        }
        catch (Exception)
        {
            Helper.ShowError("Ocorreu um erro inesperado ao adicionar um ativo ao seu portfolio.");
        }
        
        Helper.BackToStart();
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
            GetSoldAssetResponse(
                quantity,
                assetSymbol,
                Helpers.Currency.DoubleToCurrency(assets.First().CurrentPrice * quantity));
        }
        catch (PortfolioException e)
        {
            Helper.ShowError(e.Message);
        }
        catch (Exception)
        {
            Helper.ShowError("Ocorreu um erro inesperado ao vender o ativo.");
        }
        
        Helper.BackToStart();
    }
    
    private static void ViewProfile(Portfolio portfolio)
    {
        Console.Clear();

        GetAccountInfo(portfolio);
        
        string option;
        do
        {
            AnsiConsole.MarkupLine("\n[bold orange3]OPÇÕES DE PERFIL:[/]");
            AnsiConsole.MarkupLine("[bold blue] 1[/]: Adicionar fundos à conta");
            AnsiConsole.MarkupLine("[bold blue] 2[/]: Sacar fundos da conta");
            AnsiConsole.MarkupLine("[bold blue]99[/]: Voltar ao menu principal");

            Console.Write("\nDigite o número da opção desejada: ");
            option = Console.ReadLine()!;

            switch (option)
            {
                case "1":
                    DepositFundsToWallet(portfolio);
                    break;
                case "2":
                    WithdrawFundsFromWallet(portfolio);
                    break;
                case "99":
                    return;
                default:
                    Helper.ShowError($"Opção \"{option}\" inválida.");
                    break;
            }
        } while (option != "99");
    }
    
    private static void DepositFundsToWallet(Portfolio portfolio)
    {
        Console.Clear();

        GetAccountInfo(portfolio);
            
        AnsiConsole.Markup("\n\n[bold orange3]ADICIONAR FUNDOS À CONTA:[/]\n");
        
        var amount = AnsiConsole.Prompt(
            new TextPrompt<double>("Digite o valor que deseja adicionar: ")
                .Validate(
                    a => a <= 0
                        ? ValidationResult.Error("[red]O valor deve ser maior que zero.[/]")
                        : ValidationResult.Success())
        );

        try
        {
            portfolio.Wallet.Deposit(amount);
            AnsiConsole.Markup($"\nVocê adicionou [bold green]{Helpers.Currency.DoubleToCurrency(amount)}[/] " +
                               "à sua conta com sucesso!");
        }
        catch (ArgumentOutOfRangeException e)
        {
            Helper.ShowError(e.Message);
        }
        catch (Exception)
        {
            Helper.ShowError("Ocorreu um erro inesperado ao adicionar fundos à sua conta.");
        }
        
        Helper.BackToStart();
    }
    
    private static void WithdrawFundsFromWallet(Portfolio portfolio)
    {
        Console.Clear();

        GetAccountInfo(portfolio);
            
        AnsiConsole.Markup("\n\n[bold orange3]REMOVER FUNDOS DA CONTA:[/]\n");
        
        var amount = AnsiConsole.Prompt(
            new TextPrompt<double>("Digite o valor que deseja remover: ")
                .Validate(
                    a => a <= 0
                        ? ValidationResult.Error("[red]O valor deve ser maior que zero.[/]")
                        : a > portfolio.Wallet.Balance
                        ? ValidationResult.Error("[red]O valor não pode ser maior que o saldo disponível na conta.[/]")
                        : ValidationResult.Success())
        );

        try
        {
            portfolio.Wallet.Withdraw(amount);
            AnsiConsole.Markup($"\nVocê removeu [bold red]{Helpers.Currency.DoubleToCurrency(amount)}[/] " +
                               "da sua conta com sucesso!");
        }
        catch (ArgumentOutOfRangeException e)
        {
            Helper.ShowError(e.Message);
        }
        catch (Exception)
        {
            Helper.ShowError("Ocorreu um erro inesperado ao remover fundos da sua conta.");
        }
        
        Helper.BackToStart();
    }

    private static void Logout()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold orange3]Acesso encerrado com sucesso![/]");
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle("bold blue")
            .Start("Retornando à tela de login...", ctx => {
                Thread.Sleep(3000);
            });
        LoginScreen();
    }

    private static void ExitProgram()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("\n[bold orange3]Fechando sistema...[/]");
        Console.WriteLine("\nObrigado!");
        Environment.Exit(0);
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
            ? $"{Helpers.Currency.DoubleToCurrency(asset.GetProfitOrLoss())} "
            : $"-{Helpers.Currency.DoubleToCurrency(-asset.GetProfitOrLoss())} ";
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
        AnsiConsole.Markup($"\n[bold green]Valor:[/] {Helpers.Currency.DoubleToCurrency(asset.CurrentPrice)}");
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
            $"{Helpers.Currency.DoubleToCurrency(asset.PaidPrice)}" +
                $"\n({Helpers.Currency.DoubleToCurrency(asset.PaidPrice * asset.Quantity)})",
            $"{Helpers.Currency.DoubleToCurrency(asset.CurrentPrice)}" +
                $"\n({Helpers.Currency.DoubleToCurrency(asset.CurrentPrice * asset.Quantity)})",
            GetProfitOrLossCompleteValue(asset)
        );
    }

    private static void GetAccountInfo(Portfolio portfolio)
    {
        AnsiConsole.Markup($"\n[bold blue]Nome:[/] {portfolio.User.Person.Name}" +
                           $"\n[bold blue]{portfolio.User.Person.DocumentType}:[/] " +
                           $"{portfolio.User.Person.GetFormatedDocument(portfolio.User.Person.Document)}" +
                           $"\n[bold blue]E-mail:[/] {portfolio.User.Person.Email}" +
                           "\n[bold blue]Saldo em Conta:[/] " +
                           $"{Helpers.Currency.DoubleToCurrency(portfolio.Wallet.Balance)}" +
                           
                           $"\n\n[bold blue]Quantidade de Ativos:[/] {portfolio.Assets.Count}" +
                           "\n[bold blue]Saldo Total de Ativos:[/] " +
                           $"{Helpers.Currency.DoubleToCurrency(portfolio.GetAssetsTotalValue())}\n");
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
        AnsiConsole.Markup($"\n[bold blue]Valor de Venda:[/] {Helpers.Currency.DoubleToCurrency(asset.CurrentPrice)}");
        
        AnsiConsole.Markup($"\n\n[bold blue]Data da Compra:[/] {asset.PurchaseDate:dd/MM/yyyy}");
        AnsiConsole.Markup($"\n[bold blue]Data da Compra:[/] {asset.PurchaseDate:dd/MM/yyyy}");
        AnsiConsole.Markup(
            $"\n[bold blue]Valor Pago na Compra:[/] {Helpers.Currency.DoubleToCurrency(asset.PaidPrice)}");
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
                Helper.ShowError("Nenhum ativo encontrado.");
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
                    $"\n[bold blue]Valor de Venda:[/] {Helpers.Currency.DoubleToCurrency(firstAsset.CurrentPrice)}");
                
                foreach (var asset in assets)
                {
                    AnsiConsole.Markup($"\n\n[bold blue]Data da Compra:[/] {asset.PurchaseDate:dd/MM/yyyy}");
                    AnsiConsole.Markup(
                        $"\n[bold blue]Valor Pago na Compra:[/] {Helpers.Currency.DoubleToCurrency(asset.PaidPrice)}");
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
                    .Width(6).NoWrap().RightAligned()
                    .Footer($"[bold blue]{portfolio.GetAssetsTotalQuantity()}[/]");
                assetsTable.Columns[4]
                    .Width(15).NoWrap();
                assetsTable.Columns[5]
                    .Width(14).NoWrap().RightAligned()
                    .Footer($"[bold blue]{Helpers.Currency.DoubleToCurrency(portfolio.GetAssetsTotalPaidValue())}[/]");
                assetsTable.Columns[6]
                    .Width(14).NoWrap().RightAligned()
                    .Footer($"[bold blue]{Helpers.Currency.DoubleToCurrency(portfolio.GetAssetsTotalValue())}[/]");
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

    private static void AddSampleDataToPortfolio(Portfolio portfolio)
    {
        StockMarket.GetAllAssets();
        try
        {
            portfolio.BuyAsset(
                StockMarket.GetAssetBySymbol("STNE")!,
                150,
                7.45,
                new DateTime(2022, 6, 1));
            
            portfolio.BuyAsset(
                StockMarket.GetAssetBySymbol("AAPL")!,
                100,
                150.92,
                new DateTime(2022, 1, 15));
            
            portfolio.BuyAsset(
                StockMarket.GetAssetBySymbol("GOOGL")!,
                50,
                2800.50,
                new DateTime(2022, 3, 10));
            
            portfolio.BuyAsset(
                StockMarket.GetAssetBySymbol("TSLA")!,
                30,
                702.91,
                new DateTime(2022, 5, 20));
        }
        catch (PortfolioException e)
        {
            Helper.ShowError(e.Message);
        }
        catch (Exception)
        {
            Helper.ShowError("Ocorreu um erro inesperado ao adicionar um ativo ao seu portfolio.");
        }
    }
}