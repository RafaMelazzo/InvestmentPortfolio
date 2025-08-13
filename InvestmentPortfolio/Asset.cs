namespace InvestmentPortfolio;

public class Asset(
    string symbol,
    string name,
    string type,
    double currentPrice,
    int quantity = 1,
    double paidPrice = 0,
    DateTime purchaseDate = default)
{
    private string Symbol { get; } = symbol;
    private string Name { get; } = name;
    private string Type { get; } = type;
    private double CurrentPrice { get; } = currentPrice;
    private int Quantity { get; set; } = quantity > 0 ? quantity : 1;
    private double PaidPrice { get; } = paidPrice > 0 ? paidPrice : currentPrice;
    private DateTime PurchaseDate { get; } = purchaseDate == default ? DateTime.Now : purchaseDate;
    private double ProfitOrLoss => CurrentPrice - PaidPrice;
    
    public string GetSymbol() => Symbol.ToUpper();
    public string GetName() => Name;
    public new string GetType() => Type;
    public double GetCurrentPrice() => CurrentPrice;
    public int GetQuantity() => Quantity;
    public double GetPaidPrice() => PaidPrice;
    public DateTime GetPurchaseDate() => PurchaseDate;
    public double GetProfitOrLoss() => ProfitOrLoss;
    public bool IsProfit => CurrentPrice >= PaidPrice;

    public static void AddQuantityToAsset(Asset asset, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantity));
        }
        
        asset.Quantity += quantity;
    }
    
    public static void SubtractQuantityFromAsset(Asset asset, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantity));
        }
        
        if (quantity > asset.Quantity)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantidade a ser subtraída não pode ser maior que a quantidade atual do ativo."
            );
        }
        
        asset.Quantity -= quantity;
    }
}
