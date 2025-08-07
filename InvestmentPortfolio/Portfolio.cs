using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
    private List<Asset> Assets { get; }
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
    
    public string GetName() => Name;
    public string GetCpf() => Cpf;
    public List<Asset> GetAssets() => Assets;
    public double GetWalletBalance() => WalletBalance;
    public int GetQuantity() => Quantity;
    public double GetPaidTotal() => PaidTotal;
    public double GetAssetsTotalValue() => AssetsTotalValue;
    public int GetAssetsCount() => Assets.Count;
    
    public string GetFirstName()
    {
        return Name.Split(' ')[0];
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
            TerminalHelper.ShowError($"Ativo com o símbolo \"{asset.GetSymbol()}\" não encontrado no mercado.");
            return;
        }
        
        var portfolioHasAsset = HasAsset(asset.GetSymbol());
        var assetsCost = Helper.DoubleToCurrency(paidValue * quantity);
        
        List<Asset> existingAssets = [];
        if (portfolioHasAsset)
            existingAssets = GetAllAssetsWithSameSymbol(asset.GetSymbol());

        if (existingAssets.Count > 0 && existingAssets.Exists(a => Helper.NearlyEqual(a.GetPaidPrice(), paidValue)))
        {
            asset = existingAssets.First(a => Helper.NearlyEqual(a.GetPaidPrice(), paidValue));
            Asset.AddQuantityToAsset(asset, quantity);
            Terminal.GetBoughtAssetResponse(quantity, asset.GetSymbol(), assetsCost);
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
        Terminal.GetBoughtAssetResponse(quantity, asset.GetSymbol(), assetsCost);
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
        
        Terminal.GetSoldAssetResponse(quantitySold, assetSymbol, assetEarning);
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

    [GeneratedRegex(@"\D")]
    private static partial Regex AnyNonDigitRegex();
}
