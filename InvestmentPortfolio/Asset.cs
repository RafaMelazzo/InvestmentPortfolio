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
    private double PaidPrice { get; set; } = paidPrice > 0 ? paidPrice : currentPrice;
    private DateTime PurchaseDate { get; set; } = purchaseDate == default ? DateTime.Now : purchaseDate;
    private double ProfitOrLoss => CurrentPrice - PaidPrice;
    
    public string GetSymbol() => Symbol.ToUpper();
    public string GetName() => Name;
    public new string GetType() => Type.ToUpper();
    public double GetCurrentPrice() => CurrentPrice;
    public int GetQuantity() => Quantity;
    public double GetPaidPrice() => PaidPrice;
    public DateTime GetPurchaseDate() => PurchaseDate;
    public double GetProfitOrLoss() => ProfitOrLoss;
    public bool IsProfit => CurrentPrice >= PaidPrice;
    
    public void SetPaidPrice(double price)
    {
        if (price <= 0)
        {
            Helper.ShowError("Valor pago deve ser maior que zero.");
            return;
        }
        
        PaidPrice = price;
    }

    public static void AddQuantityToAsset(Asset asset, int quantity)
    {
        if (quantity <= 0)
        {
            Helper.ShowError("Quantidade deve ser maior que zero.");
            return;
        }
        
        asset.Quantity += quantity;
    }
    
    public static void SubtractQuantityFromAsset(Asset asset, int quantity)
    {
        if (quantity <= 0)
        {
            Helper.ShowError("Quantidade deve ser maior que zero.");
            return;
        }
        
        if (quantity > asset.Quantity)
        {
            Helper.ShowError($"Quantidade a ser subtraída ({quantity}) é maior que a quantidade atual ({asset.Quantity}).");
            return;
        }
        
        asset.Quantity -= quantity;
    }
}
