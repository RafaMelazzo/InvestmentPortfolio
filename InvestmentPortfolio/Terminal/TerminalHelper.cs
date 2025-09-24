using Spectre.Console;

namespace InvestmentPortfolio.Terminal;

public static class TerminalHelper
{
    
    /// <summary>Clears the console and waits for a key press to return to the start</summary>
    public static void BackToStart()
    {
        Console.WriteLine("\n\nPressione qualquer tecla para retornar ao menu.");
        Console.ReadKey();
        Console.Clear();
    }
    
    /// <summary>
    /// Clears the console to shows an error message, and waits for a key press to return back to the main menu.
    /// </summary>
    /// <param name="message">Message to be displayed</param>
    public static void ShowError(string message)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"\n[bold red]Erro:[/] {message}");
        BackToStart();
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