using System.Globalization;
using Spectre.Console;

namespace InvestmentPortfolio;

public static class Helper
{
    /**
     * <summary>Clears the console and waits for a key press to return to the start</summary>
     */
    public static void BackToStart()
    {
        Console.WriteLine("\n\nPressione qualquer tecla para retornar ao menu.");
        Console.ReadKey();
        Console.Clear();
    }
    
    /**
     * <summary>Clears the console and shows an error message</summary>
     * <param name="message">Message to be displayed</param>
     */
    public static void ShowError(string message)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"\n[bold red]Erro:[/] {message}");
        BackToStart();
    }
    
    /**
     * <summary>Checks if two double values are nearly equal within a given epsilon</summary>
     * <param name="a">First value</param>
     * <param name="b">Second value</param>
     * <param name="epsilon">Tolerance for comparison</param>
     * <returns>True if nearly equal, otherwise false</returns>
     */
    public static bool NearlyEqual(double a, double b, double epsilon = double.Epsilon)
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
    
    /**
     * <summary>Format a double value to currency string</summary>
     * <param name="value">Value to be formatted</param>
     * <param name="culture">Culture for formatting (default is "pt-BR")</param>
     * <returns>string</returns>
     */
    public static string DoubleToCurrency(double value, string culture = "pt-BR")
    {
        return value.ToString("C", new CultureInfo(culture));
    }
    
    public static int? GetDynamicColumnWidth(Table table, int columnId, int cellPadding = 1)
    {
        var totalWidth = AnsiConsole.Console.Profile.Width;
        var cellsPadding = cellPadding * 2 * table.Columns.Count;
        int? columnsWidth = 0;
        
        for (var i = 0; i < table.Columns.Count; i++)
        {
            if (i == columnId) continue; // Skip the dynamic column
            columnsWidth += table.Columns[i].Width;
        }
            
        return totalWidth - cellsPadding - columnsWidth;
    }
}
