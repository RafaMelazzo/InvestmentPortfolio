using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace InvestmentPortfolio;

public class Portfolio
{
    private readonly string _cpf;
    private double _walletBalance;
    private string Name { get; }
    private string Cpf
    {
        get => _cpf;
        init
        {
            var cpfNumbers = Regex.Replace(value, @"\D", "");

            if (cpfNumbers.Length != 11)
            {
                throw new ValidationException("CPF deve conter 11 números");
            }
            
            _cpf = $"{cpfNumbers[..3]}.{cpfNumbers[3..6]}.{cpfNumbers[6..9]}-{cpfNumbers[9..11]}";
        }
    }
    private List<Asset> Assets { get; set; }
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
    private int Quantity => Assets.Sum(asset => asset.Quantity);
    private double PaidTotal => Assets.Sum(asset => asset.PaidPrice * asset.Quantity);
    private double AssetsTotalValue => Assets.Sum(asset => asset.CurrentPrice * asset.Quantity);
    
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
        return Assets.Any(a => a.Symbol.Equals(symbol));
    }

    private Asset? GetAssetBySymbol(string symbol, double paidValue = 0)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        if (paidValue <= 0)
            return Assets.FirstOrDefault(a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase)) ?? null;
        
        var asset = Assets.FirstOrDefault(
            a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase)
                      && Helper.NearlyEqual(a.CurrentPrice, paidValue)
        );

        if (asset == null)
            return null;
        
        asset.PaidPrice = paidValue;
        return asset;
    }
    
    private List<Asset> GetAllAssetsWithSameSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        return Assets.FindAll(a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
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
            0   => asset.CurrentPrice,
            _   => paidValue
        };
        
        if (purchaseDate == default || purchaseDate > DateTime.Now)
            purchaseDate = DateTime.Today;

        var stockMarketAssets = StockMarket.GetAllAssets();
        var assetExists = stockMarketAssets.Exists(a => a.Symbol == asset.Symbol);
        if (!assetExists)
        {
            var message = $"Ativo com o símbolo \"{asset.Symbol}\" não encontrado no mercado.";
            Helper.ShowError(message);
            return;
        }
        
        var portfolioHasAsset = HasAsset(asset.Symbol);
        
        List<Asset> existingAssets = [];
        if (portfolioHasAsset)
            existingAssets = GetAllAssetsWithSameSymbol(asset.Symbol);

        if (existingAssets.Count > 0 && existingAssets.Exists(a => Helper.NearlyEqual(a.PaidPrice, paidValue)))
        {
            asset = existingAssets.First(a => Helper.NearlyEqual(a.PaidPrice, paidValue));
            AddQuantityToAsset(asset, quantity);
            return;
        }

        var newAsset = new Asset(
            asset.Symbol,
            asset.Name,
            asset.Type,
            asset.CurrentPrice,
            quantity,
            paidValue,
            purchaseDate
        );
        
        Assets.Add(newAsset);
    }

    private static void AddQuantityToAsset(Asset portfolioAsset, int quantity)
    {
        if (quantity <= 0)
        {
            Helper.ShowError("Quantidade deve ser maior que zero.");
            return;
        }
        
        portfolioAsset.Quantity += quantity;
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
                
                foreach (var asset in Assets)
                {
                    asset.AddRowToTable(assetsTable);
                    ctx.Refresh();
                    Thread.Sleep(TableDelay);
                }
            });
        
        AnsiConsole.Console.Profile.Width = defaultConsoleWidth;
    }
}
