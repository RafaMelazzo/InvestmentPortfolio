using InvestmentPortfolio.Services.Interfaces;
using ArgumentOutOfRangeException = InvestmentPortfolio.Exceptions.ArgumentOutOfRangeException;

namespace InvestmentPortfolio.Services;

public class Wallet : IWallet
{
    public Wallet(double initialAmount = 0)
    {
        if (initialAmount < 0)
            throw new ArgumentOutOfRangeException("Initial amount cannot be negative.");
        
        Balance = initialAmount;
    }
    
    public double Balance { get; private set; }

    public void Deposit(double amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException("Deposit amount must be greater than zero.");
        
        Balance += amount;
    }

    public void Withdraw(double amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException("Withdraw amount must be greater than zero.");
        
        if (amount > Balance)
            throw new ArgumentOutOfRangeException("Insufficient funds for this withdrawal.");
        
        Balance -= amount;
    }
}