using InvestmentPortfolio.Exceptions;
using InvestmentPortfolio.Models.Enums;
using InvestmentPortfolio.Models.Interfaces;
using ArgumentException = InvestmentPortfolio.Exceptions.ArgumentException;

namespace InvestmentPortfolio.Models;

public class User : IUser
{
    public User(Person person, string password, UserRoles role = UserRoles.User)
    {
        Person = person;
        Password = ValidatePassword(password)
            ? password
            : throw new ArgumentException("Invalid password.");
        Role = role;
    }
    
    public Person Person { get; }
    public string Password { get; private set; }
    public UserRoles Role { get; private set; }
    
    public bool IsValidPassword(string password) => Password == password;
    
    public void ChangePassword(string newPassword)
    {
        Password = ValidatePassword(newPassword)
            ? newPassword
            : throw new ArgumentException("Invalid password.");
    }

    public void ChangeRole(string role)
    {
        if (!Enum.TryParse<UserRoles>(role, true, out var parsedRole))
            throw new ArgumentException("Invalid role.");
        
        Role = parsedRole;
    }

    private static bool ValidatePassword(string password)
    {
        var hasLowerChar = Helpers.CustomRegex.AnyQuantityOfLowerChar();
        var hasUpperChar = Helpers.CustomRegex.AnyQuantityOfUpperChar();
        var hasNumber = Helpers.CustomRegex.AnyQuantityOfNumbers();
        var hasSpecialChar = Helpers.CustomRegex.AnyQuantityOfSpecialChar();
        var hasBetweenMinAndMaxChars = Helpers.CustomRegex.BetweenEightAndFifteenChars();
        
        if (!hasLowerChar.IsMatch(password)
            && !hasUpperChar.IsMatch(password)
            && !hasNumber.IsMatch(password)
            && !hasSpecialChar.IsMatch(password)
            && !hasBetweenMinAndMaxChars.IsMatch(password))
            throw new ArgumentException(
                "Password must have between 8 and 15 characters, containing lower and uppercase letters, " +
                "as well as numbers and special characters.");
        
        return true;
    }
}