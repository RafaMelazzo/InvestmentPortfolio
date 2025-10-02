namespace InvestmentPortfolio.Models.Enums;

public enum UserRoles
{
    Administrator,
    Manager,
    User
}

public enum AdministratorPermissions
{
    CreateUser,
    ReadUser,
    UpdateUser,
    DeleteUser,
    CreateInvestment,
    ReadInvestment,
    UpdateInvestment,
    DeleteInvestment,
    CreatePortfolio,
    ReadPortfolio,
    UpdatePortfolio,
    DeletePortfolio,
    AssignInvestmentToPortfolio
}

public enum ManagerPermissions
{
    CreateInvestment,
    ReadInvestment,
    UpdateInvestment,
    DeleteInvestment,
    CreatePortfolio,
    ReadPortfolio,
    UpdatePortfolio,
    DeletePortfolio,
    AssignInvestmentToPortfolio
}

public enum UserPermissions
{
    ReadInvestment,
    ReadPortfolio
}