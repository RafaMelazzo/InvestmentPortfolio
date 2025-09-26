using InvestmentPortfolio;
using InvestmentPortfolio.Exceptions;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Services;
using InvestmentPortfolio.Terminal;

var portfolio = new Portfolio(
    new Person(
        "Tony Stark",
        "35374527215", // Random valid CPF for testing purposes
        "tony@starkindustries.com"
    ),
    new Wallet(15213993.22)
);
AddSampleDataToPortfolio();

Navigation.WelcomeScreen(portfolio);
return;

void AddSampleDataToPortfolio()
{
    var sampleAssets = StockMarket.GetAllAssets();
    foreach (var asset in sampleAssets)
    {
        var quantity = 1;
        var paidPrice = asset.CurrentPrice;
        var purchaseDate = DateTime.Today;
        
        switch (asset.Symbol)
        {
            case "STNE":
                quantity = 150;
                paidPrice = 7.45;
                purchaseDate = new DateTime(2022, 6, 1);
                break;
            case "AAPL":
                quantity = 100;
                paidPrice = 150.92;
                purchaseDate = new DateTime(2022, 1, 15);
                break;
            case "GOOGL":
                quantity = 50;
                paidPrice = 2800.50;
                purchaseDate = new DateTime(2022, 3, 10);
                break;
            case "TSLA":
                quantity = 30;
                paidPrice = 702.91;
                purchaseDate = new DateTime(2022, 5, 20);
                break;
            case "LONGNAME":
                quantity = 200;
                paidPrice = 100.00;
                purchaseDate = new DateTime(2023, 1, 1);
                break;
            case "VERYEXP":
                quantity = 10;
                paidPrice = 1000000.00;
                purchaseDate = new DateTime(2023, 2, 1);
                break;
            case "VERYNEG":
                quantity = 5;
                paidPrice = 1000000.00;
                purchaseDate = new DateTime(2023, 3, 1);
                break;
            case "BIGPROFIT":
                quantity = 20;
                paidPrice = 0.12;
                purchaseDate = new DateTime(2023, 4, 1);
                break;
            case "BIGLOSS":
                quantity = 15;
                paidPrice = 1000.00;
                purchaseDate = new DateTime(2023, 5, 1);
                break;
            case "BOUGHTTODAY":
                quantity = 10;
                paidPrice = 52.00;
                break;
            case "MINIMUM":
                // Nothing to change here. Testing with minimum data.
                break;
        }

        try
        {
            portfolio.BuyAsset(asset, quantity, paidPrice, purchaseDate);
        }
        catch (PortfolioException e)
        {
            Helper.ShowError(e.Message);
        }
        catch (Exception)
        {
            Helper.ShowError("Ocorreu um erro inesperado ao adicionar um ativo ao seu portfolio.");
        }
    }
    
    try
    {
        portfolio.BuyAsset(StockMarket.GetAssetBySymbol("STNE")!, 7);
    }
    catch (PortfolioException e)
    {
        Helper.ShowError(e.Message);
    }
    catch (Exception)
    {
        Helper.ShowError("Ocorreu um erro inesperado ao adicionar um ativo ao seu portfolio.");
    }
}
