using InvestmentPortfolio.Models.Interfaces;

namespace InvestmentPortfolio.Models;

public class Person : IPerson
{
    public Person(string name, string document, string email)
    {
        Name = name;
        Document = document;
        Email = email;
        DocumentType = SetDocumentType(document);
    }

    public string Name { get; }
    public string Document { get; }
    public string DocumentType { get; private set; }
    public string Email { get; }
    
    public string GetFirstName() => Name.Split(' ')[0];

    internal string SetDocumentType(string document)
    {
        DocumentType = GetUnformattedDocument(document).Length switch
        {
            11 => "CPF",
            14 => "CNPJ",
            _ => "Unknown"
        };
        return DocumentType;
    }
    
    internal string GetFormatedDocument(string document)
    {
        return DocumentType.ToUpper() switch
        {
            "CPF" => GetFormatedCpf(document),
            "CNPJ" => GetFormatedCnpj(document),
            _ => document
        };
    }
    
    internal static string GetFormatedCpf(string cpf)
    {
        var cpfNumbers = GetUnformattedDocument(cpf);
        return $"{cpfNumbers[..3]}.{cpfNumbers[3..6]}.{cpfNumbers[6..9]}-{cpfNumbers[9..11]}";
    }
    
    internal static string GetFormatedCnpj(string cnpj)
    {
        var cnpjNumbers = GetUnformattedDocument(cnpj);
        return $"{cnpjNumbers[..2]}.{cnpjNumbers[2..5]}.{cnpjNumbers[5..8]}/{cnpjNumbers[8..12]}-{cnpjNumbers[12..14]}";
    }
    
    internal static string GetUnformattedDocument(string document)
    {
        return Helper.AnyNonDigitRegex().Replace(document, "");
    }

    internal static bool ValidateCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var cpfNumbers = GetUnformattedDocument(cpf);
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
    
    internal static bool ValidateCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        var cnpjNumbers = GetUnformattedDocument(cnpj);
        if (cnpjNumbers.Length != 14)
            return false;

        // Check for repeated digits
        if (new string(cnpjNumbers[0], 14) == cnpjNumbers)
            return false;

        // Validate CNPJ digits
        var multipliers1 = new[] {5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2};
        var multipliers2 = new[] {6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2};

        var sum = 0;
        for (var i = 0; i < 12; i++)
            sum += (cnpjNumbers[i] - '0') * multipliers1[i];

        var firstCheckDigit = sum % 11;
        firstCheckDigit = firstCheckDigit < 2 ? 0 : 11 - firstCheckDigit;

        if (firstCheckDigit != cnpjNumbers[12] - '0')
            return false;

        sum = 0;
        for (var i = 0; i < 13; i++)
            sum += (cnpjNumbers[i] - '0') * multipliers2[i];

        var secondCheckDigit = sum % 11;
        secondCheckDigit = secondCheckDigit < 2 ? 0 : 11 - secondCheckDigit;

        return secondCheckDigit == cnpjNumbers[13] - '0';
    }
    
    
}