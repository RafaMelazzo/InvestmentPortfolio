using System.Text.RegularExpressions;

namespace InvestmentPortfolio;

public partial class Portfolio
{
    public Portfolio(
        string name,
        string cpf,
        double walletBalance = 0,
        List<Asset>? assets = null)
    {
        Name = name;
        Cpf = GetFormatedCpf(cpf);
        WalletBalance = walletBalance;
        Assets = assets ?? [];
    }
    
    public string Name { get; }
    public string Cpf { get; }
    public List<Asset> Assets { get; }
    private double WalletBalance { get; set; }
    
    public string GetFirstName() => Name.Split(' ')[0];
    public int GetAssetsTotalQuantity() => Assets.Sum(a => a.Quantity);
    public double GetAssetsTotalPaidValue() => Assets.Sum(a => a.PaidPrice * a.Quantity);
    public double GetAssetsTotalValue() => Assets.Sum(a => a.CurrentPrice * a.Quantity);
    internal bool HasAsset(string symbol) => Assets.Any(a => a.Symbol.Equals(symbol));

    internal Asset? GetAssetBySymbol(string symbol, double paidValue = 0)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        if (paidValue <= 0)
            return Assets.FirstOrDefault(a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
                   ?? null;
        
        var asset = Assets.FirstOrDefault(
            a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase)
                      && Helper.NearlyEqual(a.PaidPrice, paidValue)
        );
        
        return asset ?? null;
    }

    public List<Asset> GetAllAssetsWithSameSymbol(string symbol)
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
            TerminalHelper.ShowError($"Ativo com o símbolo \"{asset.Symbol}\" não encontrado no mercado.");
            return;
        }
        
        var portfolioHasAsset = HasAsset(asset.Symbol);
        var assetsCost = Helper.DoubleToCurrency(paidValue * quantity);
        
        List<Asset> existingAssets = [];
        if (portfolioHasAsset)
            existingAssets = GetAllAssetsWithSameSymbol(asset.Symbol);

        if (existingAssets.Count > 0 && existingAssets.Exists(a => Helper.NearlyEqual(a.PaidPrice, paidValue)))
        {
            asset = existingAssets.First(a => Helper.NearlyEqual(a.PaidPrice, paidValue));
            
            try
            {
                Asset.AddQuantityToAsset(asset, quantity);
            }
            catch (Exception e)
            {
                TerminalHelper.ShowError(e.Message);
                throw;
            }
            
            Terminal.GetBoughtAssetResponse(quantity, asset.Symbol, assetsCost);
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
        Terminal.GetBoughtAssetResponse(quantity, asset.Symbol, assetsCost);
    }

    public void SellAsset(List<Asset> assets, int sellingQuantity = 1)
    {
        if (assets == null || assets.Count == 0)
            throw new ArgumentException("Asset list cannot be null or empty.", nameof(assets));
        
        if (sellingQuantity < 1)
            throw new ArgumentOutOfRangeException(nameof(sellingQuantity), "Quantity must be greater than zero.");
        
        var assetsTotalQuantity = assets.Sum(a => a.Quantity);
        
        if (sellingQuantity > assetsTotalQuantity)
            throw new ArgumentOutOfRangeException(
                nameof(sellingQuantity),
                $"You do not have {sellingQuantity} units of this asset. Available: {assetsTotalQuantity}"
            );
        
        if (assets.Count > 1)
            assets = assets.OrderByDescending(a => a.GetProfitOrLoss()).ToList();
        
        var firstAsset = assets.First();
        var assetSymbol = firstAsset.Symbol;
        var assetEarning = Helper.DoubleToCurrency(firstAsset.CurrentPrice * sellingQuantity);
        var quantitySold = sellingQuantity;

        while (sellingQuantity > 0 && assets.Count > 0)
        {
            foreach (var asset in assets)
            {
                var assetQuantity = asset.Quantity;

                if (assetQuantity >= sellingQuantity)
                {
                    ReduceAssetQuantity(asset, sellingQuantity);
                    WalletBalance += asset.CurrentPrice * assetQuantity;
                    sellingQuantity = 0;
                    break;
                }
                
                ReduceAssetQuantity(asset, assetQuantity);
                WalletBalance += asset.CurrentPrice * asset.Quantity;
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
        
        if (!HasAsset(asset.Symbol))
            throw new InvalidOperationException($"Asset with symbol {asset.Symbol} does not exist in the portfolio.");

        var existingAsset = GetAssetBySymbol(asset.Symbol);
        if (existingAsset == null)
            throw new InvalidOperationException($"No asset found with symbol {asset.Symbol}.");

        try
        {
            Asset.SubtractQuantityFromAsset(existingAsset, quantity);
        }
        catch (PortfolioException e)
        {
            TerminalHelper.ShowError(e.Message);
        }
        catch (Exception)
        {
            TerminalHelper.ShowError("Ocorreu um erro inesperado ao reduzir a quantidade do ativo.");
        }
        
        if (existingAsset.Quantity <= 0)
            Assets.Remove(existingAsset);
    }
    
    private static string GetFormatedCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ValidationException("CPF não pode ser nulo ou vazio.");
        
        if (!ValidateCpf(cpf))
            throw new ValidationException("CPF inválido.");

        var cpfNumbers = AnyNonDigitRegex().Replace(cpf, "");
        return $"{cpfNumbers[..3]}.{cpfNumbers[3..6]}.{cpfNumbers[6..9]}-{cpfNumbers[9..11]}";
    }

    private static bool ValidateCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var cpfNumbers = AnyNonDigitRegex().Replace(cpf, "");
        if (cpfNumbers.Length != 11)
            return false;

        // Check for repeated digits
        if (new string(cpfNumbers[0], 11) == cpfNumbers)
            return false;

        // Validate CPF digits
        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += (cpfNumbers[i] - '0') * (10 - i);
        
        var firstCheckDigit = (sum * 10) % 11;
        if (firstCheckDigit == 10)
            firstCheckDigit = 0;

        if (firstCheckDigit != cpfNumbers[9] - '0')
            return false;

        sum = 0;
        for (var i = 0; i < 10; i++)
            sum += (cpfNumbers[i] - '0') * (11 - i);
        
        var secondCheckDigit = (sum * 10) % 11;
        if (secondCheckDigit == 10)
            secondCheckDigit = 0;

        return secondCheckDigit == cpfNumbers[10] - '0';
    }

    [GeneratedRegex(@"\D")]
    private static partial Regex AnyNonDigitRegex();
}
