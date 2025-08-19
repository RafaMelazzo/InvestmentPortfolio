namespace InvestmentPortfolio;

public class Asset
{
    public Asset(
        string symbol,
        string name,
        string type,
        double currentPrice,
        int quantity = 1,
        double paidPrice = 0,
        DateTime purchaseDate = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type cannot be null or empty.", nameof(type));
        
        if (currentPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(currentPrice), "Current price must be greater than zero.");
        
        Symbol = symbol.ToUpper();
        Name = name;
        Type = type;
        CurrentPrice = currentPrice;
        Quantity = quantity > 0 ? quantity : 1;
        PaidPrice = paidPrice > 0 ? paidPrice : currentPrice;
        PurchaseDate = purchaseDate == default ? DateTime.Now : purchaseDate;
    }
    
    public string Symbol { get; }
    public string Name { get; }
    public string Type { get; }
    public double CurrentPrice { get; }
    public int Quantity { get; private set; }
    public double PaidPrice { get; }
    public DateTime PurchaseDate { get; }
    
    public double GetProfitOrLoss() => CurrentPrice - PaidPrice;
    public bool IsProfit() => CurrentPrice > PaidPrice;
    
    
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
