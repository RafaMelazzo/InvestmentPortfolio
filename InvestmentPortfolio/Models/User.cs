using System.Text.RegularExpressions;
using InvestmentPortfolio.Models.Interfaces;

namespace InvestmentPortfolio.Models;

public partial class User : IUser
{
    public User(string name, int document, string documentType, string email)
    {
        Name = name;
        Document = document;
        DocumentType = documentType;
        Email = email;
    }

    public string Name { get; }
    public int Document { get; }
    public string DocumentType { get; }
    public string Email { get; }
    
    public string GetFirstName() => Name.Split(' ')[0];
    
    internal static string GetFormatedCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ValidationException("CPF não pode ser nulo ou vazio.");
        
        if (!ValidateCpf(cpf))
            throw new ValidationException("CPF inválido.");

        var cpfNumbers = AnyNonDigitRegex().Replace(cpf, "");
        return $"{cpfNumbers[..3]}.{cpfNumbers[3..6]}.{cpfNumbers[6..9]}-{cpfNumbers[9..11]}";
    }

    internal static bool ValidateCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var cpfNumbers = AnyNonDigitRegex().Replace(cpf, "");
        if (cpfNumbers.Length != 11)
            return false;

        // Check for repeated digits
        if (new string(cpfNumbers[0], 11) == cpfNumbers)
            return false;

        // Validate CPF digits
        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += (cpfNumbers[i] - '0') * (10 - i);
        
        var firstCheckDigit = (sum * 10) % 11;
        if (firstCheckDigit == 10)
            firstCheckDigit = 0;

        if (firstCheckDigit != cpfNumbers[9] - '0')
            return false;

        sum = 0;
        for (var i = 0; i < 10; i++)
            sum += (cpfNumbers[i] - '0') * (11 - i);
        
        var secondCheckDigit = (sum * 10) % 11;
        if (secondCheckDigit == 10)
            secondCheckDigit = 0;

        return secondCheckDigit == cpfNumbers[10] - '0';
    }

    [GeneratedRegex(@"\D")]
    internal static partial Regex AnyNonDigitRegex();
}