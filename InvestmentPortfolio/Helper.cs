using System.Globalization;
using Spectre.Console;

namespace InvestmentPortfolio;

public static class Helper
{
    ///<summary>Checks if two double values are nearly equal within a given epsilon</summary>
    ///<param name="a">First value</param>
    ///<param name="b">Second value</param>
    ///<param name="epsilon">Tolerance for comparison (default is double.Epsilon)</param>
    ///<returns>True if nearly equal, otherwise false</returns>
    public static bool NearlyEqualDouble(double a, double b, double epsilon = double.Epsilon)
    {
        const double minNormal = 2.2250738585072014E-308d;
        var absA = Math.Abs(a);
        var absB = Math.Abs(b);
        var diff = Math.Abs(a - b);

        // shortcut, handles infinities
        if (a.Equals(b))
            return true;
        
        // a or b is zero or both are extremely close to it
        // relative error is less meaningful here
        if (a == 0 || b == 0 || absA + absB < minNormal)
            return diff < (epsilon * minNormal);
        
        // use relative error
        return diff / (absA + absB) < epsilon;
    }
    
    ///<summary>Format a double value to currency string</summary>
    ///<param name="value">Value to be formatted</param>
    ///<param name="culture">Culture for formatting (default is "pt-BR")</param>
    ///<returns>string</returns>
    public static string DoubleToCurrency(double value, string culture = "pt-BR")
    {
        return value.ToString("C", new CultureInfo(culture));
    }
}
