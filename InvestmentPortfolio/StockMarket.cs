namespace InvestmentPortfolio;

public abstract class StockMarket
{
    private static List<Asset>? Assets { get; set; }

    protected StockMarket(List<Asset>? assets = null)
    {
        Assets = assets ?? new List<Asset>();
    }

    public static List<Asset> GetAllAssets()
    {
        Assets = [];
        
        Assets.AddRange(
            new Asset(
                "STNE",
                "StoneCo Ltd.",
                "Ação",
                14.86
            ),
            new Asset(
                "AAPL",
                "Apple Inc.",
                "Ação",
                175.13
            ),
            new Asset(
                "GOOGL",
                "Alphabet Inc.",
                "Ação",
                2900.75
            ),
            new Asset(
                "TSLA",
                "Tesla Inc.",
                "Ação",
                20.42
            ),
            new Asset(
                "LONGNAME",
                "This is a very long asset name that should be truncated in the table",
                "Ação",
                120.00
            ),
            new Asset(
                "VERYEXP",
                "Very Expensive Asset",
                "Ação",
                1200000.00
            ),
            new Asset(
                "VERYNEG",
                "Very Expensive Negative Asset",
                "Ação",
                800000.00
            ),
            new Asset(
                "BIGPROFIT",
                "Big Profit Asset",
                "Ação",
                1000.00
            ),
            new Asset(
                "BIGLOSS",
                "Big Loss Asset",
                "Ação",
                0.19
            ),
            new Asset(
                "BOUGHTTODAY",
                "Bought Today Asset",
                "Ação",
                52.00
            ),
            new Asset(
                "MINIMUM",
                "Minimum Info Asset",
                "Ação",
                45.00
            )
        );
        
        return Assets;
    }
    
    public static bool AssetExists(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));
        
        var assets = GetAllAssets();
        return assets.Any(a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
    }
    
    public static Asset? GetAssetBySymbol(string symbol)
    {
        return Assets?.FirstOrDefault(asset => asset.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
    }
}
