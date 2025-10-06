using InvestmentPortfolio.Services;
using ArgumentException = InvestmentPortfolio.Exceptions.ArgumentException;

namespace InvestmentPortfolio;

public abstract class StockMarket
{
    private static List<Asset>? Assets { get; set; }

    protected StockMarket(List<Asset>? assets = null)
    {
        Assets = assets ?? [];
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
                "NVDA",
                "NVIDIA Corporation",
                "Ação",
                275.50
            ),
            new Asset(
                "AMZN",
                "Amazon.com Inc.",
                "Ação",
                3300.00
            ),
            new Asset(
                "MSFT",
                "Microsoft Corporation",
                "Ação",
                299.35
            ),
            new Asset(
                "META",
                "Meta Platforms Inc.",
                "Ação",
                350.25
            ),
            new Asset(
                "PLTR",
                "Palantir Technologies Inc.",
                "Ação",
                25.75
            ),
            new Asset(
                "NFLX",
                "Netflix Inc.",
                "Ação", 540.10
            ),
            new Asset(
                "AMD",
                "Advanced Micro Devices Inc.",
                "Ação",
                105.60
            ),
            new Asset(
                "INTC",
                "Intel Corporation",
                "Ação",
                55.30
            ),
            new Asset(
                "BTC",
                "Bitcoin",
                "Criptomoeda",
                45000.00
            ),
            new Asset(
                "ETH",
                "Ethereum",
                "Criptomoeda",
                3000.00
            ),
            new Asset(
                "LTC",
                "Litecoin",
                "Criptomoeda",
                180.00
            ),
            new Asset(
                "DOGE",
                "Dogecoin",
                "Criptomoeda",
                0.25
            )
        );
        
        return Assets;
    }
    
    public static bool AssetExists(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.");
        
        var assets = GetAllAssets();
        return assets.Any(a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
    }
    
    public static Asset? GetAssetBySymbol(string symbol)
    {
        return Assets?.FirstOrDefault(asset => asset.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
    }
}
