using Spectre.Console;

namespace InvestmentPortfolio;

public static class TerminalHelper
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
}