using InvestmentPortfolio.Exceptions;
using InvestmentPortfolio.Services;
using ArgumentException = InvestmentPortfolio.Exceptions.ArgumentException;
using ArgumentOutOfRangeException = InvestmentPortfolio.Exceptions.ArgumentOutOfRangeException;

namespace InvestmentPortfolioTests.Services;

public class AssetTests
{
    [Theory]
    [InlineData(null, "Test Asset", "Ação", 100.0, "symbol")]
    [InlineData("", "Test Asset", "Ação", 100.0, "symbol")]
    [InlineData(" ", "Test Asset", "Ação", 100.0, "symbol")]
    [InlineData("TEST", null, "Ação", 100.0, "name")]
    [InlineData("TEST", "", "Ação", 100.0, "name")]
    [InlineData("TEST", " ", "Ação", 100.0, "name")]
    [InlineData("TEST", "Test Asset", null, 100.0, "type")]
    [InlineData("TEST", "Test Asset", "", 100.0, "type")]
    [InlineData("TEST", "Test Asset", " ", 100.0, "type")]
    [InlineData("TEST", "Test Asset", "Ação", 0.0, "currentPrice")]
    [InlineData("TEST", "Test Asset", "Ação", -1.0, "currentPrice")]
    public void Asset_Should_ThrowException_WhenValuesAreNullOrWhiteSpaces_Or_CurrentPrice_IsLessThanOne(
        string? symbol,
        string? name,
        string? type,
        double currentPrice,
        string invalidParameter)
    {
        // Arrange
        Exception exception;
        
        // Act
        void Action() => new Asset(
            symbol!,
            name!,
            type!,
            currentPrice
        );
        
        // Assert
        switch (invalidParameter)
        {
            case "symbol":
                exception = Assert.Throws<ArgumentException>((Action)Action);
                Assert.Equal("Symbol cannot be null or empty.", exception.Message);
                break;
            case "name":
                exception = Assert.Throws<ArgumentException>((Action)Action);
                Assert.Equal("Name cannot be null or empty.", exception.Message);
                break;
            case "type":
                exception = Assert.Throws<ArgumentException>((Action)Action);
                Assert.Equal("Type cannot be null or empty.", exception.Message);
                break;
            case "currentPrice":
                exception = Assert.Throws<ArgumentOutOfRangeException>((Action)Action);
                Assert.Equal("Current price must be greater than zero.", exception.Message);
                break;
        }
    }
    
    [Theory]
    [InlineData(100, 50, 50)]
    [InlineData(150, 150, 0)]
    [InlineData(200, 300, -100)]
    public void GetProfitOrLoss_Should_ReturnExpectedValue_BasedOnCurrentAndPaidPrice(
        double currentPrice,
        double paidPrice,
        double expectedProfitOrLoss)
    {
        // Arrange
        const string mockAssetSymbol = "TEST";
        const string mockAssetName = "Test Asset";
        const string mockAssetType = "Ação";
        const int mockAssetQuantity = 10;
        
        var mockAsset = new Asset(
            mockAssetSymbol,
            mockAssetName,
            mockAssetType,
            currentPrice,
            mockAssetQuantity,
            paidPrice
        );
        
        // Act
        var profitOrLoss = mockAsset.GetProfitOrLoss();
        
        // Assert
        Assert.Equal(expectedProfitOrLoss, profitOrLoss);
    }
    
    [Theory]
    [InlineData(100, 50, true)]
    [InlineData(100, 200, false)]
    [InlineData(100, 100, false)]
    public void IsProfit_Should_ReturnExpectedResult_BasedOnCurrentAndPaidPrice(
        double currentPrice,
        double paidPrice,
        bool expectedResult)
    {
        // Arrange
        const string mockAssetSymbol = "TEST";
        const string mockAssetName = "Test Asset";
        const string mockAssetType = "Ação";
        const int mockAssetQuantity = 10;
        
        var mockAsset = new Asset(
            mockAssetSymbol,
            mockAssetName,
            mockAssetType,
            currentPrice,
            mockAssetQuantity,
            paidPrice
        );
        
        // Act
        var isProfit = mockAsset.IsProfit;
        
        // Assert
        Assert.Equal(expectedResult, isProfit());
    }

    [Fact]
    public void AddQuantityToAsset_Should_IncreaseAssetQuantity_WhenValidQuantityIsProvided()
    {
        // Arrange
        const string mockAssetSymbol = "TEST";
        const string mockAssetName = "Test Asset";
        const string mockAssetType = "Ação";
        const double mockAssetCurrentPrice = 100.0;
        const int initialQuantity = 10;
        const int quantityToAdd = 5;
        
        var mockAsset = new Asset(
            mockAssetSymbol,
            mockAssetName,
            mockAssetType,
            mockAssetCurrentPrice,
            initialQuantity
        );
        
        // Act
        Asset.AddQuantityToAsset(mockAsset, quantityToAdd);
        
        // Assert
        Assert.Equal(initialQuantity + quantityToAdd, mockAsset.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddQuantityToAsset_Should_ThrowException_And_NotChangeAssetQuantity_WhenInvalidQuantityIsProvided(
        int invalidQuantityToAdd)
    {
        // Arrange
        const string mockAssetSymbol = "TEST";
        const string mockAssetName = "Test Asset";
        const string mockAssetType = "Ação";
        const double mockAssetCurrentPrice = 100.0;
        const int initialQuantity = 10;
        
        var mockAsset = new Asset(
            mockAssetSymbol,
            mockAssetName,
            mockAssetType,
            mockAssetCurrentPrice,
            initialQuantity
        );
        
        // Act
        void Action() => Asset.AddQuantityToAsset(mockAsset, invalidQuantityToAdd);

        // Assert
        var exception = Assert.Throws<ValidationException>((Action)Action);
        Assert.Equal("Quantidade deve ser maior que zero.", exception.Message);
        Assert.Equal(initialQuantity, mockAsset.Quantity);
    }

    [Fact]
    public void SubtractQuantityFromAsset_Should_DecreaseAssetQuantity_WhenValidQuantityIsProvided()
    {
        // Arrange
        const string mockAssetSymbol = "TEST";
        const string mockAssetName = "Test Asset";
        const string mockAssetType = "Ação";
        const double mockAssetCurrentPrice = 100.0;
        const int initialQuantity = 10;
        const int quantityToSubtract = 5;
        
        var mockAsset = new Asset(
            mockAssetSymbol,
            mockAssetName,
            mockAssetType,
            mockAssetCurrentPrice,
            initialQuantity
        );
        
        // Act
        Asset.SubtractQuantityFromAsset(mockAsset, quantityToSubtract);
        
        // Assert
        Assert.Equal(initialQuantity - quantityToSubtract, mockAsset.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SubtractQuantityFromAsset_Should_ThrowException_WhenInvalidQuantityIsProvided(
        int invalidQuantityToSubtract)
    {
        // Arrange
        const string mockAssetSymbol = "TEST";
        const string mockAssetName = "Test Asset";
        const string mockAssetType = "Ação";
        const double mockAssetCurrentPrice = 100.0;
        const int initialQuantity = 10;
        
        var mockAsset = new Asset(
            mockAssetSymbol,
            mockAssetName,
            mockAssetType,
            mockAssetCurrentPrice,
            initialQuantity
        );
        
        // Act
        void Action() => Asset.SubtractQuantityFromAsset(mockAsset, invalidQuantityToSubtract);

        // Assert
        var exception = Assert.Throws<ValidationException>((Action)Action);
        Assert.Equal("Quantidade deve ser maior que zero.", exception.Message);
        Assert.Equal(initialQuantity, mockAsset.Quantity);
    }

    [Fact]
    public void SubtractQuantityFromAsset_Should_ThrowException_WhenQuantityToSubtractIsGreaterThanCurrentQuantity()
    {
        // Arrange
        const string mockAssetSymbol = "TEST";
        const string mockAssetName = "Test Asset";
        const string mockAssetType = "Ação";
        const double mockAssetCurrentPrice = 100.0;
        const int initialQuantity = 10;
        const int quantityToSubtract = 15; // maior que a quantidade atual
        
        var mockAsset = new Asset(
            mockAssetSymbol,
            mockAssetName,
            mockAssetType,
            mockAssetCurrentPrice,
            initialQuantity
        );
        
        // Act
        void Action() => Asset.SubtractQuantityFromAsset(mockAsset, quantityToSubtract);
        
        // Assert
        var exception = Assert.Throws<ValidationException>((Action)Action);
        Assert.Equal("Quantidade a ser subtraída não pode ser maior que a quantidade atual do ativo.", exception.Message);
        Assert.Equal(initialQuantity, mockAsset.Quantity);
    } 
}
