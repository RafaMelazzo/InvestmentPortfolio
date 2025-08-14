namespace InvestmentPortfolio;

public class PortfolioException : Exception
{
    public PortfolioException(string message) : base(message)
    {
        
    }
}

public class ValidationException : PortfolioException
{
    public ValidationException(string message) : base(message)
    {
        
    }
}