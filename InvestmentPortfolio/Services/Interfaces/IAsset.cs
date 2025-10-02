namespace InvestmentPortfolio.Services.Interfaces;

public interface IAsset
{
    public string Symbol { get; }
    public string Name { get; }
    public string Type { get; }
    public double CurrentPrice { get; }
    public int Quantity { get; }
    public double PaidPrice { get; }
    public DateTime PurchaseDate { get; }
}