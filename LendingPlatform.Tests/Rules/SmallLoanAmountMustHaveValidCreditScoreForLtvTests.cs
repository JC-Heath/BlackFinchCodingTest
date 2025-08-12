using FluentAssertions;
using LendingPlatform.ConsoleApp.Models;
using LendingPlatform.ConsoleApp.Rules;
using Xunit;

namespace LendingPlatform.Tests.Rules;

public class SmallLoanAmountMustHaveValidCreditScoreForLtvTests
{
    private readonly SmallLoanAmountMustHaveValidCreditScoreForLtv _rule = new();

    [Theory]
    [InlineData(500000, 1000000, 750)] // LTV = 50% (< 60%), Credit = 750 (minimum required)
    [InlineData(500000, 1000000, 800)] // LTV = 50% (< 60%), Credit = 800 (above minimum)
    [InlineData(500000, 700000, 800)] // LTV = 71.4% (< 80%), Credit = 800 (minimum required)
    [InlineData(500000, 700000, 850)] // LTV = 71.4% (< 80%), Credit = 850 (above minimum)
    [InlineData(500000, 600000, 900)] // LTV = 83.3% (< 90%), Credit = 900 (minimum required)
    [InlineData(500000, 600000, 950)] // LTV = 83.3% (< 90%), Credit = 950 (above minimum)
    public void IsSatisfied_WithSmallLoanAndValidCriteria_ShouldReturnTrue(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Arrange
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Act
        var result = _rule.IsSatisfied(application);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(500000, 1000000, 749)] // LTV = 50% (< 60%), Credit = 749 (below minimum)
    [InlineData(500000, 700000, 799)] // LTV = 71.4% (< 80%), Credit = 799 (below minimum)
    [InlineData(500000, 600000, 899)] // LTV = 83.3% (< 90%), Credit = 899 (below minimum)
    [InlineData(500000, 550000, 999)] // LTV = 90.9% (>= 90%), any credit score should fail
    [InlineData(500000, 500000, 999)] // LTV = 100% (>= 90%), any credit score should fail
    public void IsSatisfied_WithSmallLoanAndInvalidCriteria_ShouldReturnFalse(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Arrange
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Act
        var result = _rule.IsSatisfied(application);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(1000000, 1000000, 700)] // Large loan - should pass this rule
    [InlineData(1500000, 1000000, 600)] // Large loan - should pass this rule
    public void IsSatisfied_WithLargeLoan_ShouldReturnTrue(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Arrange
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Act
        var result = _rule.IsSatisfied(application);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ExecuteFailure_ShouldAddAppropriateDeclineReason()
    {
        // Arrange
        var application = new LoanApplication(500000, 550000, 999);
        application.DeclineReasons.Clear();

        // Act
        _rule.ExecuteFailure(application);

        // Assert
        application.DeclineReasons.Should().HaveCount(1);
        application.DeclineReasons.First().Should().Contain("Small loan amount must have a valid credit score for the LTV ratio");
    }

    [Theory]
    [InlineData(299999, 600000, 750)] // LTV = 50%, exactly at boundary for 750 credit requirement
    [InlineData(479999, 600000, 800)] // LTV = 80%, exactly at boundary for 800 credit requirement  
    [InlineData(539999, 600000, 900)] // LTV = 90%, exactly at boundary for 900 credit requirement
    public void IsSatisfied_WithLtvBoundaryConditions_ShouldBehaveProperly(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Arrange
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Act
        var result = _rule.IsSatisfied(application);

        // Assert
        result.Should().BeTrue();
    }
}
