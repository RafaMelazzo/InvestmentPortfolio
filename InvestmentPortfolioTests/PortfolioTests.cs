using InvestmentPortfolio;
using ArgumentException = InvestmentPortfolio.ArgumentException;
using ArgumentNullException = InvestmentPortfolio.ArgumentNullException;
using ArgumentOutOfRangeException = InvestmentPortfolio.ArgumentOutOfRangeException;
using InvalidOperationException = InvestmentPortfolio.InvalidOperationException;

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
    [InlineData(1, 100, 1)]
    [InlineData(2, 50)]
    [InlineData(2, 100, 1)]
    [InlineData(2, 110, 1)]
    [InlineData(2, 200, 2)]
    public void SellAsset_Should_SellAsset_WhenValidParametersAreProvided(
        int assetsCount,
        int quantityToSell,
        int removedAssetsCount = 0)
    {
        // Arrange
        List<Asset> assets = [];
        assets.AddRange(Enumerable.Range(1, assetsCount)
            .Select(i => new Asset(
                "TEST",
                "Test Asset",
                "Ação",
                300.0,
                100,
                10.0 * i, // Different paid price for each asset
                new DateTime(2025, 12, 1))
            )
        );
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15", 0, assets);
        var sellingAssets = portfolio.GetAllAssetsWithSameSymbol("TEST");

        // Act
        portfolio.SellAsset(sellingAssets, quantityToSell);

        // Assert
        var remainingAssets = portfolio.GetAllAssetsWithSameSymbol("TEST");
        Assert.Equal(assetsCount - removedAssetsCount, remainingAssets.Count);
        var totalRemainingQuantity = remainingAssets.Sum(a => a.Quantity);
        Assert.Equal(100 * assetsCount - quantityToSell, totalRemainingQuantity);
    }

    [Fact]
    public void SellAsset_Should_ThrowArgumentException_WhenAssetsListIsNullOrEmpty()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");

        // Act
        void NullAction() => portfolio.SellAsset(null!);
        void EmptyAction() => portfolio.SellAsset(new List<Asset>());

        // Assert
        var nullException = Assert.Throws<ArgumentException>(NullAction);
        var emptyException = Assert.Throws<ArgumentException>(EmptyAction);
        Assert.Equal("Asset list cannot be null or empty.", nullException.Message);
        Assert.Equal("Asset list cannot be null or empty.", emptyException.Message);
    }

    [Fact]
    public void SellAsset_Should_ThrowArgumentOutOfRangeException_WhenQuantityToSellIsInvalid()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");
        var assets = new List<Asset>
        {
            new("TEST",
                "Test Asset",
                "Ação",
                300.0,
                100,
                10.0,
                new DateTime(2025, 12, 1))
        };
        portfolio.Assets.AddRange(assets);
        var sellingAssets = portfolio.GetAllAssetsWithSameSymbol("TEST");

        // Act
        void Action() => portfolio.SellAsset(sellingAssets, 0);

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(Action);
        Assert.Equal("Quantity must be greater than zero.", exception.Message);
    }

    [Fact]
    public void SellAsset_Should_ThrowArgumentOutOfRangeException_WhenSellingMoreThanAvailableQuantity()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");
        var assets = new List<Asset>
        {
            new("TEST",
                "Test Asset",
                "Ação",
                300.0,
                100,
                10.0,
                new DateTime(2025, 12, 1))
        };
        portfolio.Assets.AddRange(assets);
        var sellingAssets = portfolio.GetAllAssetsWithSameSymbol("TEST");

        // Act
        void Action() => portfolio.SellAsset(sellingAssets, 200);

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(Action);
        Assert.Equal("You do not have 200 units of this asset. Available: 100.", exception.Message);
    }
    
    [Theory]
    [InlineData(10)]
    [InlineData(100, true)]
    public void ReduceAssetQuantity_Should_RemoveAsset_WhenQuantityReachesZero_Or_ReduceQuantityOtherwise(
        int quantityToSell,
        bool assetShouldBeRemoved = false)
    {
        // Arrange
        var asset = new Asset(
            "TEST",
            "Test Asset",
            "Ação",
            300.0,
            100,
            10.0,
            new DateTime(2025, 12, 1));
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");
        portfolio.Assets.Add(asset);
        var initialQuantity = asset.Quantity;
        
        // Act
        portfolio.ReduceAssetQuantity(asset, quantityToSell);
        
        // Assert
        if (assetShouldBeRemoved)
            Assert.False(portfolio.HasAsset("TEST"));
        else
            Assert.Equal(initialQuantity - quantityToSell, asset.Quantity);
    }

    [Fact]
    public void ReduceAssetQuantity_Should_ThrowArgumentNullException_WhenAssetIsNull()
    {
        // Arrange
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");

        // Act
        void Action() => portfolio.ReduceAssetQuantity(null!, 10);

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(Action);
        Assert.Equal("Asset cannot be null.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ReduceAssetQuantity_Should_ThrowArgumentOutOfRangeException_WhenQuantityIsInvalid(int invalidQuantity)
    {
        // Arrange
        var asset = new Asset(
            "TEST",
            "Test Asset",
            "Ação",
            300.0,
            100,
            10.0,
            new DateTime(2025, 12, 1));
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");
        portfolio.Assets.Add(asset);
        
        // Act
        void Action() => portfolio.ReduceAssetQuantity(asset, invalidQuantity);
        
        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(Action);
        Assert.Equal("Quantity must be greater than zero.", exception.Message);
    }

    [Fact]
    public void ReduceAssetQuantity_Should_ThrowInvalidOperationException_WhenAssetDoesNotExistInPortfolio()
    {
        // Arrange
        var asset = new Asset(
            "TEST",
            "Test Asset",
            "Ação",
            300.0,
            100,
            10.0,
            new DateTime(2025, 12, 1));
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");
        
        // Act
        void Action() => portfolio.ReduceAssetQuantity(asset, 10);
        
        // Assert
        var exception = Assert.Throws<InvalidOperationException>(Action);
        Assert.Equal("Asset with symbol TEST does not exist in the portfolio.", exception.Message);
    }

    [Fact]
    public void ReduceAssetQuantity_Should_ThrowInvalidOperationException_When_GetAssetBySymbol_ReturnsNull()
    {
        // Arrange
        var asset = new Asset(
            "TEST",
            "Test Asset",
            "Ação",
            300.0,
            100,
            10.0,
            new DateTime(2025, 12, 1));
        var portfolio = new Portfolio("Test Portfolio", "353.745.272-15");
        portfolio.Assets.Add(asset);
        
        // Act
        void Action() => portfolio.ReduceAssetQuantity(new Asset(
            "NONEXISTENT",
            "Nonexistent Asset",
            "Ação",
            300.0,
            100,
            10.0,
            new DateTime(2025, 12, 1)), 10);
        
        // Assert
        var exception = Assert.Throws<InvalidOperationException>(Action);
        Assert.Equal("Asset with symbol NONEXISTENT does not exist in the portfolio.", exception.Message);
    }
    
    [Fact]
    public void GetFormatedCpf_Should_ReturnFormattedCpf_WhenValidCpfIsProvided()
    {
        // Act
        var formattedCpf = Portfolio.GetFormatedCpf("35374527215");

        // Assert
        Assert.Equal("353.745.272-15", formattedCpf);
    }

    [Fact]
    public void GetFormatedCpf_Should_ThrowValidationException_WhenCpfIsNullOrEmpty()
    {
        // Act
        void NullAction() => Portfolio.GetFormatedCpf(null!);
        void EmptyAction() => Portfolio.GetFormatedCpf(string.Empty);

        // Assert
        var nullException = Assert.Throws<ValidationException>(NullAction);
        var emptyException = Assert.Throws<ValidationException>(EmptyAction);
        Assert.Equal("CPF não pode ser nulo ou vazio.", nullException.Message);
        Assert.Equal("CPF não pode ser nulo ou vazio.", emptyException.Message);
    }

    [Fact]
    public void GetFormatedCpf_Should_ThrowValidationException_WhenCpfIsInvalid()
    {
        // Act
        void InvalidCpf() => Portfolio.GetFormatedCpf("12345678910");
        void InvalidCpfLenght() => Portfolio.GetFormatedCpf("123");

        // Assert
        var invalidCpfException = Assert.Throws<ValidationException>(InvalidCpf);
        Assert.Equal("CPF inválido.", invalidCpfException.Message);
        var invalidCpfLengthException = Assert.Throws<ValidationException>(InvalidCpfLenght);
        Assert.Equal("CPF inválido.", invalidCpfLengthException.Message);
    }

    [Fact]
    public void ValidateCpf_Should_ReturnTrue_WhenValidCpfIsProvided_Or_ReturnFalseOtherwise()
    {
        // Act
        var validCpf = Portfolio.ValidateCpf("35374527215");
        var validCpfWithFormat = Portfolio.ValidateCpf("353.745.272-15");
        var invalidCpf = Portfolio.ValidateCpf("12345678910");
        var invalidCpfLength = Portfolio.ValidateCpf("123");
        var nullCpf = Portfolio.ValidateCpf(null!);
        var emptyCpf = Portfolio.ValidateCpf(string.Empty);
        var whitespaceCpf = Portfolio.ValidateCpf(" ");
        var repeatedCpf = Portfolio.ValidateCpf("11111111111");

        // Assert
        Assert.True(validCpf);
        Assert.True(validCpfWithFormat);
        Assert.False(invalidCpf);
        Assert.False(invalidCpfLength);
        Assert.False(nullCpf);
        Assert.False(emptyCpf);
        Assert.False(whitespaceCpf);
        Assert.False(repeatedCpf);
    }
    
    [Theory]
    [InlineData("123.456.789-10", "12345678910")]
    [InlineData("12345678910", "12345678910")]
    [InlineData("abc123", "123")]
    [InlineData("abc123def456", "123456")]
    [InlineData("!@#123!@#456", "123456")]
    public void AnyNonDigitRegex__Should_ReplaceAllNonDigitCharacters(string input, string expectedOutput)
    {
        // Act
        var result = Portfolio.AnyNonDigitRegex().Replace(input, "");

        // Assert
        Assert.Equal(expectedOutput, result);
    }
}
