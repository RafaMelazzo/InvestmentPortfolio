using InvestmentPortfolio.Models.Enums;

namespace InvestmentPortfolio.Models.Interfaces;

public interface IUser
{
    Person Person { get; }
    string Password { get; }
    UserRoles Role { get; }
}