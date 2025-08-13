using InvestmentPortfolio;

namespace InvestmentPortfolioTests;

public class AssetTests
{
    [Fact] // SERÁ EXECUTADO UMA ÚNICA VEZ
    public void AssetGetters_Should_ReturnTheirCorrectValues()
    {
        // Arrange
        const string mockAssetSymbol = "TEST";
        const string mockAssetName = "Test Asset";
        const string mockAssetType = "Ação";
        const double mockAssetCurrentPrice = 100.0;
        const int mockAssetQuantity = 10;
        const double mockAssetPaidPrice = 50.0;
        var mockAssetPurchaseDate = new DateTime(2001, 9, 11);
        const double mockAssetProfitOrLoss = mockAssetCurrentPrice - mockAssetPaidPrice;
        
        var mockAsset = new Asset(
            mockAssetSymbol,
            mockAssetName,
            mockAssetType,
            mockAssetCurrentPrice,
            mockAssetQuantity,
            mockAssetPaidPrice,
            mockAssetPurchaseDate
        );
        
        // Act
        var symbol = mockAsset.GetSymbol();
        var name = mockAsset.GetName();
        var type = mockAsset.GetType();
        var currentPrice = mockAsset.GetCurrentPrice();
        var quantity = mockAsset.GetQuantity();
        var paidPrice = mockAsset.GetPaidPrice();
        var purchaseDate = mockAsset.GetPurchaseDate();
        var profitOrLoss = mockAsset.GetProfitOrLoss();
        
        // Assert
        Assert.Equal(mockAssetSymbol, symbol);
        Assert.Equal(mockAssetName, name);
        Assert.Equal(mockAssetType, type);
        Assert.Equal(mockAssetCurrentPrice, currentPrice);
        Assert.Equal(mockAssetQuantity, quantity);
        Assert.Equal(mockAssetPaidPrice, paidPrice);
        Assert.Equal(mockAssetPurchaseDate, purchaseDate);
        Assert.Equal(mockAssetProfitOrLoss, profitOrLoss);
    }
    
    [Fact]
    public void IsProfit_Should_ReturnTrue_WhenCurrentPriceIsGreaterThanOrEqualToPaidPrice()
    {
        // Arrange
        const string mockAssetSymbol = "TEST";
        const string mockAssetName = "Test Asset";
        const string mockAssetType = "Ação";
        const double mockAssetCurrentPrice = 100.0;
        const int mockAssetQuantity = 10;
        const double mockAssetPaidPrice = 50.0;
        
        var mockAsset = new Asset(
            mockAssetSymbol,
            mockAssetName,
            mockAssetType,
            mockAssetCurrentPrice,
            mockAssetQuantity,
            mockAssetPaidPrice
        );
        
        // Act
        var isProfit = mockAsset.IsProfit;
        
        // Assert
        Assert.True(isProfit);
    }

    [Fact]
    public void IsProfit_Should_ReturnFalse_WhenCurrentPriceIsLessThanPaidPrice()
    {
        // Arrange
        const string mockAssetSymbol = "TEST";
        const string mockAssetName = "Test Asset";
        const string mockAssetType = "Ação";
        const double mockAssetCurrentPrice = 40.0;
        const int mockAssetQuantity = 10;
        const double mockAssetPaidPrice = 50.0;
        
        var mockAsset = new Asset(
            mockAssetSymbol,
            mockAssetName,
            mockAssetType,
            mockAssetCurrentPrice,
            mockAssetQuantity,
            mockAssetPaidPrice
        );
        
        // Act
        var isProfit = mockAsset.IsProfit;
        
        // Assert
        Assert.False(isProfit);
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
        Assert.Equal(initialQuantity + quantityToAdd, mockAsset.GetQuantity());
    }

    [Theory] // SERÁ EXECUTADO N VEZES, UMA PARA CADA DADO
    [InlineData(0)]
    [InlineData(-1)]
    public void AddQuantityToAsset_Should_ThrowException_And_NotChangeAssetQuantity_WhenInvalidQuantityIsProvided(int invalidQuantityToAdd)
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
        var exception = Assert.Throws<ArgumentException>((Action)Action);
        Assert.Equal("Quantidade deve ser maior que zero. (Parameter 'quantity')", exception.Message);
        Assert.Equal("quantity", exception.ParamName);
        Assert.Equal(initialQuantity, mockAsset.GetQuantity());
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
        Assert.Equal(initialQuantity - quantityToSubtract, mockAsset.GetQuantity());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SubtractQuantityFromAsset_Should_ThrowException_WhenInvalidQuantityIsProvided(int invalidQuantityToSubtract)
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
        var exception = Assert.Throws<ArgumentException>((Action)Action);
        Assert.Equal("Quantidade deve ser maior que zero. (Parameter 'quantity')", exception.Message);
        Assert.Equal("quantity", exception.ParamName);
        Assert.Equal(initialQuantity, mockAsset.GetQuantity());
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
        var exception = Assert.Throws<ArgumentOutOfRangeException>((Action)Action);
        Assert.Equal("Quantidade a ser subtraída não pode ser maior que a quantidade atual do ativo. (Parameter 'quantity')", exception.Message);
        Assert.Equal("quantity", exception.ParamName);
        Assert.Equal(initialQuantity, mockAsset.GetQuantity());
    }
}
