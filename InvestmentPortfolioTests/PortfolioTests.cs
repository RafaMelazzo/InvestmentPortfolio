using InvestmentPortfolio;

namespace InvestmentPortfolioTests;

public class PortfolioTests
{
    [Fact]
    public void GetFirstName_Should_ReturnFirstName_WhenPortfolioIsCreated()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15"); // Random valid CPF for testing purposes

        // Act
        var firstName = portfolio.GetFirstName();

        // Assert
        Assert.Equal("Test", firstName);
    }

    [Fact]
    public void GetAssetsTotalQuantity_Should_ReturnTotalQuantityOfAssets_WhenPortfolioHasAssets()
    {
        // Arrange
        var mockAssets = new List<Asset>
        {
            new("TEST1", "Test Asset 1", "Ação", 100.0, 10, 50.0, new DateTime(2021, 1, 1)),
            new("TEST2", "Test Asset 2", "Ação", 200.0, 5, 150.0, new DateTime(2022, 2, 2))
        };
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, mockAssets);

        // Act
        var totalQuantity = portfolio.GetAssetsTotalQuantity();

        // Assert
        Assert.Equal(15, totalQuantity);
    }

    [Fact]
    public void GetAssetsTotalQuantity_Should_ReturnZero_WhenPortfolioHasNoAssets()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");

        // Act
        var totalQuantity = portfolio.GetAssetsTotalQuantity();

        // Assert
        Assert.Equal(0, totalQuantity);
    }

    [Fact]
    public void GetAssetsTotalPaidValue_Should_ReturnTotalPaidValue_WhenPortfolioHasAssets()
    {
        // Arrange
        var mockAssets = new List<Asset>
        {
            new("TEST1", "Test Asset 1", "Ação", 100.0, 10, 50.0, new DateTime(2021, 1, 1)),
            new("TEST2", "Test Asset 2", "Ação", 200.0, 5, 150.0, new DateTime(2022, 2, 2))
        };
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, mockAssets);

        // Act
        var totalPaidValue = portfolio.GetAssetsTotalPaidValue();

        // Assert
        Assert.Equal(1250.0, totalPaidValue);
    }

    [Fact]
    public void GetAssetsTotalPaidValue_Should_ReturnZero_WhenPortfolioHasNoAssets()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");

        // Act
        var totalPaidValue = portfolio.GetAssetsTotalPaidValue();

        // Assert
        Assert.Equal(0, totalPaidValue);
    }

    [Fact]
    public void GetAssetsTotalValue_Should_ReturnTotalValue_WhenPortfolioHasAssets()
    {
        // Arrange
        var mockAssets = new List<Asset>
        {
            new("TEST1", "Test Asset 1", "Ação", 100.0, 10, 50.0, new DateTime(2021, 1, 1)),
            new("TEST2", "Test Asset 2", "Ação", 200.0, 5, 150.0, new DateTime(2022, 2, 2))
        };
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, mockAssets);

        // Act
        var totalValue = portfolio.GetAssetsTotalValue();

        // Assert
        Assert.Equal(2000.0, totalValue);
    }

    [Fact]
    public void GetAssetsTotalValue_Should_ReturnZero_WhenPortfolioHasNoAssets()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");

        // Act
        var totalValue = portfolio.GetAssetsTotalValue();

        // Assert
        Assert.Equal(0, totalValue);
    }
    
    [Fact]
    public void HasAsset_Should_ReturnTrue_WhenAssetExists_Or_ReturnFalse_WhenAssetDoesNotExists()
    {
        // Arrange
        var mockAssets = new List<Asset>
        {
            new("TEST1", "Test Asset 1", "Ação", 100.0, 10, 50.0, new DateTime(2021, 1, 1)),
            new("TEST2", "Test Asset 2", "Ação", 200.0, 5, 150.0, new DateTime(2022, 2, 2))
        };
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, mockAssets);

        // Act
        var existingAsset1 = portfolio.HasAsset("TEST1");
        var existingAsset2 = portfolio.HasAsset("TEST2");
        var nonExistingAsset = portfolio.HasAsset("TEST3");

        // Assert
        Assert.True(existingAsset1);
        Assert.True(existingAsset2);
        Assert.False(nonExistingAsset);
    }

    [Theory]
    [InlineData("TEST1")]
    [InlineData("TEST2", 150)]
    public void GetAssetBySymbol_Should_ReturnCorrectAsset_WhenAssetExists(string assetSymbol, double expectedPrice = 0)
    {
        // Arrange
        var mockAssets = new List<Asset>
        {
            new("TEST1", "Test Asset 1", "Ação", 100.0, 10, 50.0, new DateTime(2021, 1, 1)),
            new("TEST2", "Test Asset 2", "Ação", 200.0, 5, 150.0, new DateTime(2022, 2, 2))
        };
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, mockAssets);

        // Act
        var asset = portfolio.GetAssetBySymbol(assetSymbol, expectedPrice);

        // Assert
        Assert.NotNull(asset);
        Assert.Equal(assetSymbol, asset.Symbol);
        if (expectedPrice > 0)
            Assert.Equal(expectedPrice, asset.PaidPrice);
    }

    [Theory]
    [InlineData("INEXISTENT")]
    [InlineData("TEST2", 200)]
    public void GetAssetBySymbol_Should_ReturnNull_WhenAssetDoesNotExist(string assetSymbol, double paidPrice = 0)
    {
        // Arrange
        var mockAssets = new List<Asset>
        {
            new("TEST1", "Test Asset 1", "Ação", 100.0, 10, 50.0, new DateTime(2021, 1, 1)),
            new("TEST2", "Test Asset 2", "Ação", 200.0, 5, 150.0, new DateTime(2022, 2, 2))
        };
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, mockAssets);

        // Act
        var asset = portfolio.GetAssetBySymbol(assetSymbol, paidPrice);

        // Assert
        Assert.Null(asset);
    }
    
    [Fact]
    public void GetAssetBySymbol_Should_ThrowArgumentException_WhenSymbolIsNullOrEmpty()
    {
        // Arrange
        var mockAssets = new List<Asset>
        {
            new("TEST1", "Test Asset 1", "Ação", 100.0, 10, 50.0, new DateTime(2021, 1, 1)),
            new("TEST2", "Test Asset 2", "Ação", 200.0, 5, 150.0, new DateTime(2022, 2, 2))
        };
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, mockAssets);
        
        // Act
        void NullAction() => portfolio.GetAssetBySymbol(null!);
        void EmptyAction() => portfolio.GetAssetBySymbol(string.Empty);
        
        // Assert
        Assert.Throws<ArgumentException>(NullAction);
        Assert.Throws<ArgumentException>(EmptyAction);
    }
    
    [Fact]
    public void GetAllAssetsWithSameSymbol_Should_ReturnAllMatchingAssets_WhenAssetsExist()
    {
        // Arrange
        var mockAssets = new List<Asset>
        {
            new("TEST", "Test Asset 1", "Ação", 100.0, 10, 50.0, new DateTime(2021, 1, 1)),
            new("TEST", "Test Asset 2", "Ação", 200.0, 5, 150.0, new DateTime(2022, 2, 2)),
            new("OTHER", "Other Asset", "Ação", 300.0, 3, 250.0, new DateTime(2023, 3, 3))
        };
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, mockAssets);

        // Act
        var assets = portfolio.GetAllAssetsWithSameSymbol("TEST");

        // Assert
        Assert.Equal(2, assets.Count);
        Assert.All(assets, a => Assert.Equal("TEST", a.Symbol));
    }

    [Fact]
    public void GetAllAssetsWithSameSymbol_Should_ReturnEmptyList_WhenNoMatchingAssetsExist()
    {
        // Arrange
        var mockAssets = new List<Asset>
        {
            new("TEST1", "Test Asset 1", "Ação", 100.0, 10, 50.0, new DateTime(2021, 1, 1)),
            new("TEST2", "Test Asset 2", "Ação", 200.0, 5, 150.0, new DateTime(2022, 2, 2))
        };
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, mockAssets);

        // Act
        var assets = portfolio.GetAllAssetsWithSameSymbol("INEXISTENT");

        // Assert
        Assert.Empty(assets);
    }

    [Fact]
    public void GetAllAssetsWithSameSymbol_Should_ThrowArgumentException_WhenSymbolIsNullOrEmpty()
    {
        // Arrange
        var mockAssets = new List<Asset>
        {
            new("TEST1", "Test Asset 1", "Ação", 100.0, 10, 50.0, new DateTime(2021, 1, 1)),
            new("TEST2", "Test Asset 2", "Ação", 200.0, 5, 150.0, new DateTime(2022, 2, 2))
        };
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, mockAssets);
        
        // Act
        void NullAction() => portfolio.GetAllAssetsWithSameSymbol(null!);
        void EmptyAction() => portfolio.GetAllAssetsWithSameSymbol(string.Empty);
        
        // Assert
        Assert.Throws<ArgumentException>(NullAction);
        Assert.Throws<ArgumentException>(EmptyAction);
    }

    [Fact]
    public void AddAsset_Should_AddAsset_WhenValidParametersAreProvided()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");
        var asset = new Asset("TEST", "Test Asset", "Ação", 100.0, 10, 50.0, new DateTime(2021, 1, 1));

        // Act
        portfolio.AddAsset(asset, 5, 50.0, new DateTime(2023, 1, 1));

        // Assert
        Assert.Contains(asset, portfolio.Assets);
        Assert.Single(portfolio.Assets);
        Assert.Equal(10, asset.Quantity);
        Assert.Equal(50.0, asset.PaidPrice);
    }
}
