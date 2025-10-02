using System.Text.RegularExpressions;

namespace InvestmentPortfolio.Helpers;

public static partial class CustomRegex
{
    [GeneratedRegex(@"\D")]
    public static partial Regex AnyNonDigitRegex();

    [GeneratedRegex(@"\d+")]
    public static partial Regex AnyQuantityOfNumbers();

    [GeneratedRegex("[A-Z]+")]
    public static partial Regex AnyQuantityOfUpperChar();

    [GeneratedRegex("[a-z]+")]
    public static partial Regex AnyQuantityOfLowerChar();

    [GeneratedRegex("[^A-Za-z0-9]+")]
    public static partial Regex AnyQuantityOfSpecialChar();

    [GeneratedRegex(".{8,15}")]
    public static partial Regex BetweenEightAndFifteenChars();
}