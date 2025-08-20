using InvestmentPortfolio;
using ArgumentException = InvestmentPortfolio.ArgumentException;
using ArgumentNullException = InvestmentPortfolio.ArgumentNullException;
using ArgumentOutOfRangeException = InvestmentPortfolio.ArgumentOutOfRangeException;

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
        var nullException = Assert.Throws<ArgumentException>(NullAction);
        var emptyException = Assert.Throws<ArgumentException>(EmptyAction);
        Assert.Equal("Symbol cannot be null or empty.", nullException.Message);
        Assert.Equal("Symbol cannot be null or empty.", emptyException.Message);
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
        var nullException = Assert.Throws<ArgumentException>(NullAction);
        var emptyException = Assert.Throws<ArgumentException>(EmptyAction);
        Assert.Equal("Symbol cannot be null or empty.", nullException.Message);
        Assert.Equal("Symbol cannot be null or empty.", emptyException.Message);
    }

    [Fact]
    public void AddAsset_Should_AddAsset_WhenValidParametersAreProvided()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");
        var purchaseDate = DateTime.Now;
        var asset = new Asset(
            "STNE", // Needs to be an asset registered in the StockMarket
            "Test Asset",
            "Ação",
            15.0,
            1,
            15,
            purchaseDate);

        // Act
        portfolio.AddAsset(asset, 1, 15.0, purchaseDate);

        // Assert
        Assert.Equivalent(asset, portfolio.GetAssetBySymbol("STNE"));
        Assert.Single(portfolio.Assets);
    }

    [Fact]
    public void AddAsset_Should_ThrowArgumentNullException_WhenAssetIsNull()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");

        // Act
        void Action() => portfolio.AddAsset(null!);

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(Action);
        Assert.Equal("Asset cannot be null.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddAsset_Should_ThrowArgumentOutOfRangeException_WhenQuantityIsInvalid(int invalidQuantity)
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");
        var asset = new Asset(
            "STNE", // Needs to be an asset registered in the StockMarket
            "Test Asset",
            "Ação",
            15.0,
            1,
            15,
            DateTime.Now);

        // Act
        void Action() => portfolio.AddAsset(asset, invalidQuantity);

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(Action);
        Assert.Equal("Quantity must be greater than zero.", exception.Message);
    }

    [Fact]
    public void AddAsset_Should_ThrowArgumentOutOfRangeException_WhenPaidValueIsNegative()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");
        var asset = new Asset(
            "STNE", // Needs to be an asset registered in the StockMarket
            "Test Asset",
            "Ação",
            15.0,
            1,
            15,
            DateTime.Now);

        // Act
        void Action() => portfolio.AddAsset(asset, 1, -10.0);

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(Action);
        Assert.Equal("Paid value cannot be negative.", exception.Message);
    }

    [Fact]
    public void AddAsset_Should_ThrowPortfolioException_WhenAssetNotFoundInStockMarket()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");
        var asset = new Asset(
            "NONEXISTENT", // This asset does not exist in the StockMarket
            "Nonexistent Asset",
            "Ação",
            15.0,
            1,
            15,
            DateTime.Now);

        // Act
        void Action() => portfolio.AddAsset(asset);

        // Assert
        var exception = Assert.Throws<ValidationException>(Action);
        Assert.Equal("Ativo com o símbolo \"NONEXISTENT\" não encontrado no mercado.", exception.Message);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(1, 100)]
    [InlineData(2, 10)]
    [InlineData(2, 50)]
    [InlineData(2, 75)]
    [InlineData(2, 100)]
    public void SellAsset_Should_SellAsset_WhenValidParametersAreProvided(
        int assetsCount,
        int quantityToSell)
    {
        // Arrange
        List<Asset> assets = [];
        var quantityPerAsset = 100 / assetsCount;
        assets.AddRange(Enumerable.Range(1, assetsCount)
            .Select(i => new Asset(
                "TEST",
                "Test Asset",
                "Ação",
                100.0,
                quantityPerAsset,
                10.0 * i, // Different paid price for each asset
                new DateTime(2025, 12, 1))
            )
        );
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, assets);
        var portfolioAssets = portfolio.GetAllAssetsWithSameSymbol("TEST").OrderBy(a => a.PaidPrice).ToList();

        // Act
        portfolio.SellAsset(assets, quantityToSell);

        // Assert
        
    }
}
