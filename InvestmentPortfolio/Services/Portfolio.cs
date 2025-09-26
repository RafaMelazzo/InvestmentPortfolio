using InvestmentPortfolio.Exceptions;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Terminal;
using ArgumentException = InvestmentPortfolio.Exceptions.ArgumentException;
using ArgumentNullException = InvestmentPortfolio.Exceptions.ArgumentNullException;
using ArgumentOutOfRangeException = InvestmentPortfolio.Exceptions.ArgumentOutOfRangeException;
using InvalidOperationException = InvestmentPortfolio.Exceptions.InvalidOperationException;

namespace InvestmentPortfolio.Services;

public partial class Portfolio
{
    public Portfolio(
        Person person,
        double walletBalance = 0,
        List<Asset>? assets = null)
    {
        Person = person;
        WalletBalance = walletBalance;
        Assets = assets ?? [];
    }
    
    public Person Person { get; }
    public List<Asset> Assets { get; }
    public double WalletBalance { get; private set; }
    
    public int GetAssetsTotalQuantity() => Assets.Sum(a => a.Quantity);
    public double GetAssetsTotalPaidValue() => Assets.Sum(a => a.PaidPrice * a.Quantity);
    public double GetAssetsTotalValue() => Assets.Sum(a => a.CurrentPrice * a.Quantity);
    internal bool HasAsset(string symbol) => Assets.Any(a => a.Symbol.Equals(symbol));

    internal Asset? GetAssetBySymbol(string symbol, double paidValue = 0)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.");

        if (paidValue <= 0)
            return Assets.FirstOrDefault(a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
                   ?? null;
        
        var asset = Assets.FirstOrDefault(
            a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase)
                      && Helpers.Helper.NearlyEqualDouble(a.PaidPrice, paidValue)
        );
        
        return asset ?? null;
    }

    /// <summary>
    ///  Returns a list of all assets with the same symbol, ordered by the specified property.
    ///  If the property does not exist, it defaults to ordering by PaidPrice.
    ///  If no assets with the specified symbol are found, it returns an empty list.
    /// </summary>
    /// <param name="symbol">The symbol of the assets to search for.</param>
    /// <param name="orderBy">
    ///  The property by which to order the assets.
    ///  Possible values are `Name`, `Type`, `CurrentPrice`, `Quantity`, `PaidPrice`, or `PurchaseDate`.
    ///  These values are case-sensitive. Also, `Symbol` is valid, but will return the assets without ordering, since
    ///  this method returns all assets with the same symbol.
    ///  If the property does not exist, it defaults to ordering by `PaidPrice`.
    /// </param>
    /// <exception cref="ArgumentException">Thrown when the symbol is null or empty.</exception>
    /// <returns>The list of assets with the specified symbol, ordered by the specified property.</returns>
    public List<Asset> GetAllAssetsWithSameSymbol(string symbol, string orderBy = nameof(Asset.PaidPrice))
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.");
        
        var property = typeof(Asset).GetProperty(orderBy);
        if (property == null)
            property = typeof(Asset).GetProperty(nameof(Asset.PaidPrice))!;
        
        return Assets
            .FindAll(a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
            .OrderBy(a => property.GetValue(a, null))
            .ToList();
    }

    public void AddAsset(Asset asset, int quantity = 1, double paidValue = 0, DateTime purchaseDate = default)
    {
        if (asset == null)
            throw new ArgumentNullException("Asset cannot be null.");
        
        if (quantity < 1)
            throw new ArgumentOutOfRangeException("Quantity must be greater than zero.");

        paidValue = paidValue switch
        {
            < 0 => throw new ArgumentOutOfRangeException("Paid value cannot be negative."),
            0   => asset.CurrentPrice,
            _   => paidValue
        };
        
        if (purchaseDate == default || purchaseDate > DateTime.Today)
            purchaseDate = DateTime.Today;

        var stockMarketAssets = StockMarket.GetAllAssets();
        var assetExists = stockMarketAssets.Exists(a => a.Symbol == asset.Symbol);
        
        if (!assetExists)
            throw new ValidationException($"Ativo com o símbolo \"{asset.Symbol}\" não encontrado no mercado." );
        
        var portfolioHasAsset = HasAsset(asset.Symbol);
        var assetsCost = Helpers.Helper.DoubleToCurrency(paidValue * quantity);
        
        List<Asset> existingAssets = [];
        if (portfolioHasAsset)
        {
            try
            {
                existingAssets = GetAllAssetsWithSameSymbol(asset.Symbol);
            }
            catch (PortfolioException e)
            {
                TerminalHelper.ShowError(e.Message);
            }
            catch (Exception)
            {
                TerminalHelper.ShowError("Ocorreu um erro inesperado ao buscar todos ativos com o mesmo símbolo.");
            }
        }

        if (existingAssets.Count > 0 && existingAssets.Exists(a => Helpers.Helper.NearlyEqualDouble(a.PaidPrice, paidValue)))
        {
            asset = existingAssets.First(a => Helpers.Helper.NearlyEqualDouble(a.PaidPrice, paidValue));

            try
            {
                Asset.AddQuantityToAsset(asset, quantity);
            }
            catch (PortfolioException e)
            {
                TerminalHelper.ShowError(e.Message);
            }
            catch (Exception)
            {
                TerminalHelper.ShowError("Ocorreu um erro inesperado ao aumentar a quantidade de um ativo existente.");
            }
            
            Terminal.Terminal.GetBoughtAssetResponse(quantity, asset.Symbol, assetsCost);
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
        Terminal.Terminal.GetBoughtAssetResponse(quantity, asset.Symbol, assetsCost);
    }

    public void SellAsset(List<Asset> assets, int sellingQuantity = 1)
    {
        if (assets == null || assets.Count == 0)
            throw new ArgumentException("Asset list cannot be null or empty.");
        
        if (sellingQuantity < 1)
            throw new ArgumentOutOfRangeException("Quantity must be greater than zero.");
        
        var assetsTotalQuantity = assets.Sum(a => a.Quantity);
        
        if (sellingQuantity > assetsTotalQuantity)
            throw new ArgumentOutOfRangeException(
                $"You do not have {sellingQuantity} units of this asset. Available: {assetsTotalQuantity}.");
        
        if (assets.Count > 1)
            assets = assets.OrderByDescending(a => a.GetProfitOrLoss()).ToList();
        
        var firstAsset = assets.First();
        var assetSymbol = firstAsset.Symbol;
        var assetEarning = Helpers.Helper.DoubleToCurrency(firstAsset.CurrentPrice * sellingQuantity);
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
        
        Terminal.Terminal.GetSoldAssetResponse(quantitySold, assetSymbol, assetEarning);
    }
    
    internal void ReduceAssetQuantity(Asset asset, int quantity)
    {
        if (asset == null)
            throw new ArgumentNullException("Asset cannot be null.");
        
        if (quantity < 1)
            throw new ArgumentOutOfRangeException("Quantity must be greater than zero.");
        
        if (!HasAsset(asset.Symbol))
            throw new InvalidOperationException($"Asset with symbol {asset.Symbol} does not exist in the portfolio.");

        var existingAsset = GetAssetBySymbol(asset.Symbol)!;

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
}
