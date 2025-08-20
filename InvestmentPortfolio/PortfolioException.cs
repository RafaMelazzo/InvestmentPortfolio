namespace InvestmentPortfolio;

public class PortfolioException(string message) : Exception(message);
public class ArgumentException(string message) : PortfolioException(message);
public class ArgumentNullException(string message) : PortfolioException(message);
public class ArgumentOutOfRangeException(string message) : PortfolioException(message);
public class InvalidOperationException(string message) : PortfolioException(message);
public class ValidationException(string message) : PortfolioException(message);
