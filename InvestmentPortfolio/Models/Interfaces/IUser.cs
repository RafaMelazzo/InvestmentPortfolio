namespace InvestmentPortfolio.Models.Interfaces;

public interface IUser
{
    string Name { get; }
    int Document { get; }
    string DocumentType { get; }
    string Email { get; }
}
