using FluentAssertions;
using LendingPlatform.ConsoleApp.Models;
using LendingPlatform.ConsoleApp.Rules;
using Xunit;

namespace LendingPlatform.Tests.Rules;

public class LargeLoanValueRuleTests
{
    private readonly LargeLoanValueRule _rule = new();

    [Theory]
    [InlineData(1000000, 2000000, 950)] // LTV = 50%, Credit = 950
    [InlineData(1000000, 1666667, 999)] // LTV = 60%, Credit = 999
    [InlineData(1500000, 2500000, 950)] // LTV = 60%, Credit = 950
    public void IsSatisfied_WithLargeLoanAndValidCriteria_ShouldReturnTrue(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Arrange
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Act
        var result = _rule.IsSatisfied(application);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(1000000, 1600000, 950)] // LTV > 60%, Credit = 950
    [InlineData(1000000, 2000000, 949)] // LTV = 50%, Credit < 950
    [InlineData(1000000, 1500000, 800)] // LTV > 60%, Credit < 950
    public void IsSatisfied_WithLargeLoanAndInvalidCriteria_ShouldReturnFalse(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Arrange
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Act
        var result = _rule.IsSatisfied(application);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(500000, 1000000, 700)] // Small loan - should pass this rule
    [InlineData(999999, 1000000, 600)] // Just below large loan threshold
    public void IsSatisfied_WithSmallLoan_ShouldReturnTrue(decimal loanAmount, decimal assetValue, int creditScore)
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
        var application = new LoanApplication(1000000, 1500000, 900);
        application.DeclineReasons.Clear();

        // Act
        _rule.ExecuteFailure(application);

        // Assert
        application.DeclineReasons.Should().HaveCount(1);
        application.DeclineReasons.First().Should().Contain("Large loan value must have a credit score of at least 950 and LTV percent of 60% or less");
    }
}
