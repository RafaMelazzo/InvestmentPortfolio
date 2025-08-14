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
    public string Symbol { get; } = symbol.ToUpper();
    public string Name { get; } = name;
    public string Type { get; } = type;
    public double CurrentPrice { get; } = currentPrice;
    public int Quantity { get; private set; } = quantity > 0 ? quantity : 1;
    public double PaidPrice { get; } = paidPrice > 0 ? paidPrice : currentPrice;
    public DateTime PurchaseDate { get; } = purchaseDate == default ? DateTime.Now : purchaseDate;
    public double ProfitOrLoss { get; } = currentPrice - paidPrice;
    public bool IsProfit { get; } = currentPrice > paidPrice;
    
    
    /// <summary>Add the specified quantity to the asset's current quantity.</summary>
    /// 
    /// <param name="asset">The asset to which to add the quantity.</param>
    /// <param name="quantity">The quantity to add.</param>
    /// 
    /// <exception cref="ValidationException">Thrown when the quantity to add is less than or equal to zero.</exception>
    public static void AddQuantityToAsset(Asset asset, int quantity)
    {
        if (quantity <= 0)
            throw new ValidationException("Quantidade deve ser maior que zero.");
        
        asset.Quantity += quantity;
    }
    
    /// <summary>Subtract the specified quantity from the asset's current quantity.</summary>
    /// 
    /// <param name="asset">The asset from which to subtract the quantity.</param>
    /// <param name="quantity">The quantity to subtract.</param>
    /// 
    /// <exception cref="ValidationException">Thrown when the quantity to subtract is less than or equal to zero,
    /// or greater than the asset's current quantity.</exception>
    public static void SubtractQuantityFromAsset(Asset asset, int quantity)
    {
        if (quantity <= 0)
            throw new ValidationException("Quantidade deve ser maior que zero.");
        
        if (quantity > asset.Quantity)
            throw new ValidationException("Quantidade a ser subtraída não pode ser maior que a quantidade atual do ativo.");
        
        asset.Quantity -= quantity;
    }
}
