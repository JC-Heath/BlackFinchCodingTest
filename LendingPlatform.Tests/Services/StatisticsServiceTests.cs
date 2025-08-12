using FluentAssertions;
using LendingPlatform.ConsoleApp.Models;
using LendingPlatform.ConsoleApp.Services;
using NSubstitute;
using System.Text.Json;
using Xunit;

namespace LendingPlatform.Tests.Services;

public class StatisticsServiceTests
{
    [Fact]
    public void GetStatistics_WithNoApplications_ShouldReturnZeroValues()
    {
        // Arrange
        var mockRepository = Substitute.For<ILoanRepository>();
        mockRepository.GetLoanApplications().Returns(new List<LoanApplication>());
        var service = new StatisticsService(mockRepository);

        // Act
        var (approved, declined, total, mean) = service.GetStatistics();

        // Assert
        approved.Should().Be(0);
        declined.Should().Be(0);
        total.Should().Be(0);
        mean.Should().Be("0.00");
    }

    [Fact]
    public void GetStatistics_WithMixedApplications_ShouldReturnCorrectStatistics()
    {
        // Arrange
        var applications = new List<LoanApplication>
        {
            new(500000, 1000000, 800),  // Approved - LTV 50%
            new(400000, 1000000, 800),  // Approved - LTV 40%
            new(600000, 1000000, 700),  // Declined - insufficient credit for LTV 60%
            new(1000000, 1500000, 900)  // Declined - large loan with insufficient credit
        };

        var mockRepository = Substitute.For<ILoanRepository>();
        mockRepository.GetLoanApplications().Returns(applications);
        var service = new StatisticsService(mockRepository);

        // Act
        var (approved, declined, total, mean) = service.GetStatistics();

        // Assert
        approved.Should().Be(2);
        declined.Should().Be(2);
        total.Should().Be(2500000); // Sum of all loan amounts
        mean.Should().Be("54.17"); // Average LTV: (50 + 40 + 60 + 66.67) / 4 = 54.17
    }

    [Fact]
    public void GetStatistics_WithOnlyApprovedApplications_ShouldReturnCorrectStatistics()
    {
        // Arrange
        var applications = new List<LoanApplication>
        {
            new(300000, 1000000, 800),  // Approved - LTV 30%
            new(500000, 1000000, 800),  // Approved - LTV 50%
        };

        var mockRepository = Substitute.For<ILoanRepository>();
        mockRepository.GetLoanApplications().Returns(applications);
        var service = new StatisticsService(mockRepository);

        // Act
        var (approved, declined, total, mean) = service.GetStatistics();

        // Assert
        approved.Should().Be(2);
        declined.Should().Be(0);
        total.Should().Be(800000);
        mean.Should().Be("40.00"); // (30 + 50) / 2 = 40
    }

    [Fact]
    public void GetStatistics_WithOnlyDeclinedApplications_ShouldReturnCorrectStatistics()
    {
        // Arrange
        var applications = new List<LoanApplication>
        {
            new(50000, 1000000, 800),   // Declined - below minimum loan amount
            new(2000000, 1000000, 800), // Declined - above maximum loan amount
        };

        var mockRepository = Substitute.For<ILoanRepository>();
        mockRepository.GetLoanApplications().Returns(applications);
        var service = new StatisticsService(mockRepository);

        // Act
        var (approved, declined, total, mean) = service.GetStatistics();

        // Assert
        approved.Should().Be(0);
        declined.Should().Be(2);
        total.Should().Be(2050000); // Sum includes all loan amounts, even declined ones
        mean.Should().Be("102.50"); // (5 + 200) / 2 = 102.5
    }

    [Fact]
    public void GetStatisticsAsObject_ShouldReturnCorrectStatisticsResult()
    {
        // Arrange
        var applications = new List<LoanApplication>
        {
            new(500000, 1000000, 800),  // Approved - LTV 50%
            new(300000, 1000000, 700),  // Declined - insufficient credit for LTV 30%
        };

        var mockRepository = Substitute.For<ILoanRepository>();
        mockRepository.GetLoanApplications().Returns(applications);
        var service = new StatisticsService(mockRepository);

        // Act
        var result = service.GetStatisticsAsObject();

        // Assert
        result.NumberOfApprovedLoans.Should().Be(1);
        result.NumberOfDeclinedLoans.Should().Be(1);
        result.TotalValueOfLoansWritten.Should().Be(800000);
        result.MeanAverageLoanToValueRatio.Should().Be("40.00");
    }

    [Fact]
    public void GetStatisticsAsJson_ShouldReturnValidJsonString()
    {
        // Arrange
        var applications = new List<LoanApplication>
        {
            new(500000, 1000000, 800),  // Approved - LTV 50%
        };

        var mockRepository = Substitute.For<ILoanRepository>();
        mockRepository.GetLoanApplications().Returns(applications);
        var service = new StatisticsService(mockRepository);

        // Act
        var jsonResult = service.GetStatisticsAsJson();

        // Assert
        jsonResult.Should().NotBeNullOrEmpty();
        
        // Verify it's valid JSON by deserializing
        var deserializedResult = JsonSerializer.Deserialize<StatisticsService.StatisticsResult>(jsonResult);
        deserializedResult.Should().NotBeNull();
        deserializedResult!.NumberOfApprovedLoans.Should().Be(1);
        deserializedResult.NumberOfDeclinedLoans.Should().Be(0);
        deserializedResult.TotalValueOfLoansWritten.Should().Be(500000);
        deserializedResult.MeanAverageLoanToValueRatio.Should().Be("50.00");
    }

    [Fact]
    public void GetStatistics_WithLargeNumbers_ShouldHandlePrecisionCorrectly()
    {
        // Arrange
        var applications = new List<LoanApplication>
        {
            new(1000000, 3000000, 950),   // LTV 33.33%
            new(1500000, 2250000, 950),   // LTV 66.67% - should be declined for large loan
            new(999999, 3000000, 900),    // LTV 33.33% - small loan, approved
        };

        var mockRepository = Substitute.For<ILoanRepository>();
        mockRepository.GetLoanApplications().Returns(applications);
        var service = new StatisticsService(mockRepository);

        // Act
        var (approved, declined, total, mean) = service.GetStatistics();

        // Assert
        approved.Should().Be(2);
        declined.Should().Be(1);
        total.Should().Be(3499999);
        // Mean LTV should be calculated correctly with proper rounding
        mean.Should().MatchRegex(@"\d+\.\d{2}");
    }

    [Theory]
    [InlineData(100000, 500000, "20.00")]
    [InlineData(500000, 1000000, "50.00")]
    [InlineData(750000, 1000000, "75.00")]
    [InlineData(333333, 1000000, "33.33")]
    public void GetStatistics_WithSingleApplication_ShouldCalculateLtvCorrectly(decimal loanAmount, decimal assetValue, string expectedMeanLtv)
    {
        // Arrange
        var applications = new List<LoanApplication>
        {
            new(loanAmount, assetValue, 800)
        };

        var mockRepository = Substitute.For<ILoanRepository>();
        mockRepository.GetLoanApplications().Returns(applications);
        var service = new StatisticsService(mockRepository);

        // Act
        var (_, _, _, mean) = service.GetStatistics();

        // Assert
        mean.Should().Be(expectedMeanLtv);
    }
}
