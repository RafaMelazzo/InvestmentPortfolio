namespace InvestmentPortfolio.Services.Interfaces;

public interface IWallet
{
    double Balance { get; }
    void Deposit(double amount);
    void Withdraw(double amount);
}