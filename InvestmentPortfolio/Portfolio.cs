using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace InvestmentPortfolio;

public partial class Portfolio
{
    private readonly string _cpf;
    private double _walletBalance;
    private string Name { get; }
    private string Cpf
    {
        get => _cpf;
        init
        {
            var cpfNumbers = AnyNonDigitRegex().Replace(value, "");

            if (cpfNumbers.Length != 11)
            {
                throw new ValidationException("CPF deve conter 11 números");
            }
            
            _cpf = $"{cpfNumbers[..3]}.{cpfNumbers[3..6]}.{cpfNumbers[6..9]}-{cpfNumbers[9..11]}";
        }
    }
    public List<Asset> Assets { get; }
    private double WalletBalance
    {
        get => _walletBalance;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Wallet balance cannot be negative.");
            }
            
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException("Wallet balance must be a valid number.", nameof(value));
            }
            
            _walletBalance = value;
        }
    }
    private int Quantity => Assets.Sum(asset => asset.GetQuantity());
    private double PaidTotal => Assets.Sum(asset => asset.GetPaidPrice() * asset.GetQuantity());
    private double AssetsTotalValue => Assets.Sum(asset => asset.GetCurrentPrice() * asset.GetQuantity());
    
    public Portfolio(string name, string cpf, List<Asset>? assets = null, double walletBalance = 0)
    {
        Name = name;
        _cpf = cpf;
        Cpf = cpf;
        Assets = assets ?? new List<Asset>();
        WalletBalance = walletBalance;
    }
    
    private const int TableDelay = 120;
    
    public string GetFirstName()
    {
        return Name.Split(' ')[0];
    }
    
    public void GetInfos()
    {
        AnsiConsole.Markup($"\n[bold blue]Nome:[/] {Name}" +
                           $"\n[bold blue]CPF:[/] {Cpf}" +
                           $"\n\n[bold blue]Quantidade de Ativos:[/] {Assets.Count}" +
                           $"\n[bold blue]Saldo Total de Ativos:[/] {Helper.DoubleToCurrency(AssetsTotalValue)}");
    }
    
    private bool HasAsset(string symbol)
    {
        return Assets.Any(a => a.GetSymbol().Equals(symbol));
    }

    private Asset? GetAssetBySymbol(string symbol, double paidValue = 0)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        if (paidValue <= 0)
            return Assets.FirstOrDefault(a => a.GetSymbol().Equals(symbol, StringComparison.OrdinalIgnoreCase))
                   ?? null;
        
        var asset = Assets.FirstOrDefault(
            a => a.GetSymbol().Equals(symbol, StringComparison.OrdinalIgnoreCase)
                      && Helper.NearlyEqual(a.GetCurrentPrice(), paidValue)
        );

        if (asset == null)
            return null;
        
        asset.SetPaidPrice(paidValue);
        return asset;
    }

    public List<Asset> GetAllAssetsWithSameSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        return Assets.FindAll(a => a.GetSymbol().Equals(symbol, StringComparison.OrdinalIgnoreCase));
    }

    public Asset CombineAssetsWithSameSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        var existingAssets = GetAllAssetsWithSameSymbol(symbol);
        
        switch (existingAssets.Count)
        {
            case 0:
                throw new InvalidOperationException($"No assets found with symbol: {symbol}");
            case 1:
                return existingAssets.First();
            default:
            {
                var combinedAsset = existingAssets
                    .GroupBy(a => a.GetSymbol())
                    .Select(group => new Asset(
                        group.Key,
                        group.First().GetName(),
                        group.First().GetType(),
                        group.First().GetCurrentPrice(),
                        group.Sum(a => a.GetQuantity()),
                        group.First().GetPaidPrice(),
                        group.First().GetPurchaseDate()))
                    .FirstOrDefault();

                return combinedAsset
                       ?? throw new InvalidOperationException($"Failed to combine assets with symbol: {symbol}");
            }
        }
    }

    public void AddAsset(Asset asset, int quantity = 1, double paidValue = 0, DateTime purchaseDate = default)
    {
        if (asset == null)
            throw new ArgumentNullException(nameof(asset), "Asset cannot be null.");
        
        if (quantity < 1)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        paidValue = paidValue switch
        {
            < 0 => throw new ArgumentOutOfRangeException(nameof(paidValue), "Paid value cannot be negative."),
            0   => asset.GetCurrentPrice(),
            _   => paidValue
        };
        
        if (purchaseDate == default || purchaseDate > DateTime.Now)
            purchaseDate = DateTime.Today;

        var stockMarketAssets = StockMarket.GetAllAssets();
        var assetExists = stockMarketAssets.Exists(a => a.GetSymbol() == asset.GetSymbol());
        if (!assetExists)
        {
            var message = $"Ativo com o símbolo \"{asset.GetSymbol()}\" não encontrado no mercado.";
            Helper.ShowError(message);
            return;
        }
        
        var portfolioHasAsset = HasAsset(asset.GetSymbol());
        
        List<Asset> existingAssets = [];
        if (portfolioHasAsset)
            existingAssets = GetAllAssetsWithSameSymbol(asset.GetSymbol());

        if (existingAssets.Count > 0 && existingAssets.Exists(a => Helper.NearlyEqual(a.GetPaidPrice(), paidValue)))
        {
            asset = existingAssets.First(a => Helper.NearlyEqual(a.GetPaidPrice(), paidValue));
            Asset.AddQuantityToAsset(asset, quantity);
            return;
        }

        var newAsset = new Asset(
            asset.GetSymbol(),
            asset.GetName(),
            asset.GetType(),
            asset.GetCurrentPrice(),
            quantity,
            paidValue,
            purchaseDate
        );
        
        Assets.Add(newAsset);
    }

    public void SellAsset(List<Asset> assets, int sellingQuantity = 1)
    {
        if (assets == null || assets.Count == 0)
            throw new ArgumentException("Asset list cannot be null or empty.", nameof(assets));
        
        if (sellingQuantity < 1)
            throw new ArgumentOutOfRangeException(nameof(sellingQuantity), "Quantity must be greater than zero.");
        
        var assetsTotalQuantity = assets.Sum(a => a.GetQuantity());
        
        if (sellingQuantity > assetsTotalQuantity)
            throw new ArgumentOutOfRangeException(
                nameof(sellingQuantity),
                $"You do not have {sellingQuantity} units of this asset. Available: {assetsTotalQuantity}"
            );
        
        if (assets.Count > 1)
            assets = assets.OrderByDescending(a => a.GetProfitOrLoss()).ToList();
        
        var firstAsset = assets.First();
        var assetSymbol = firstAsset.GetSymbol();
        var assetEarning = Helper.DoubleToCurrency(firstAsset.GetCurrentPrice() * sellingQuantity);
        var quantitySold = sellingQuantity;

        while (sellingQuantity > 0 && assets.Count > 0)
        {
            foreach (var asset in assets)
            {
                var assetQuantity = asset.GetQuantity();

                if (assetQuantity >= sellingQuantity)
                {
                    ReduceAssetQuantity(asset, sellingQuantity);
                    WalletBalance += asset.GetCurrentPrice() * assetQuantity;
                    sellingQuantity = 0;
                    break;
                }
                
                ReduceAssetQuantity(asset, assetQuantity);
                WalletBalance += asset.GetCurrentPrice() * asset.GetQuantity();
                sellingQuantity -= assetQuantity;
            }
        }
        
        AnsiConsole.Markup(
            $"\nVocê vendeu [bold blue]{quantitySold}[/] unidades do ativo [bold blue]{assetSymbol}[/] " +
            $"por [bold green]{assetEarning}[/].");
    }
    
    private void ReduceAssetQuantity(Asset asset, int quantity)
    {
        if (asset == null)
            throw new ArgumentNullException(nameof(asset), "Asset cannot be null.");
        
        if (quantity < 1)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        
        if (!HasAsset(asset.GetSymbol()))
            throw new InvalidOperationException($"Asset with symbol {asset.GetSymbol()} does not exist in the portfolio.");

        var existingAsset = GetAssetBySymbol(asset.GetSymbol());
        if (existingAsset == null)
            throw new InvalidOperationException($"No asset found with symbol {asset.GetSymbol()}.");

        Asset.SubtractQuantityFromAsset(existingAsset, quantity);
        
        if (existingAsset.GetQuantity() <= 0)
            Assets.Remove(existingAsset);
    }

    private static void PrintAssetDetails(Asset asset)
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
            $"{Terminal.GetProfitOrLossCompleteValue(asset)}");
        
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
                PrintAssetDetails(assets.First());
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
                        $"{Terminal.GetProfitOrLossCompleteValue(asset)}");
                }
                
                AnsiConsole.Markup(
                    $"\n\n\n[bold green]Quantidade Total de Ativos[/] [bold blue]{firstAsset.GetSymbol()}[/] " +
                    $"[bold green]disponíveis para venda:[/] {assets.Sum(a => a.GetQuantity())}"
                );
                break;
        }
    }

    public void GetAssetsTable()
    {
        var defaultConsoleWidth = AnsiConsole.Console.Profile.Width;
        AnsiConsole.Console.Profile.Width = 140;
        
        AnsiConsole.Markup("\n\n[bold orange3]SUA CARTEIRA DE ATIVOS:[/]\n");
        
        if (Assets.Count == 0)
        {
            AnsiConsole.Markup("[bold red]Nenhum ativo encontrado[/]\n");
            return;
        }

        var assets = Assets.OrderBy(a => a.GetSymbol());
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
                    .Width(6).NoWrap().RightAligned().Footer($"[bold blue]{Quantity}[/]");
                assetsTable.Columns[4]
                    .Width(15).NoWrap();
                assetsTable.Columns[5]
                    .Width(14).NoWrap().RightAligned()
                    .Footer($"[bold blue]{Helper.DoubleToCurrency(PaidTotal)}[/]");
                assetsTable.Columns[6]
                    .Width(14).NoWrap().RightAligned()
                    .Footer($"[bold blue]{Helper.DoubleToCurrency(AssetsTotalValue)}[/]");
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

    [GeneratedRegex(@"\D")]
    private static partial Regex AnyNonDigitRegex();
}
