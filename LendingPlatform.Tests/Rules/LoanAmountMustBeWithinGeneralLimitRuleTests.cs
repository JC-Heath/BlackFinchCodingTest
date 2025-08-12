using FluentAssertions;
using LendingPlatform.ConsoleApp.Models;
using LendingPlatform.ConsoleApp.Rules;
using Xunit;

namespace LendingPlatform.Tests.Rules;

public class LoanAmountMustBeWithinGeneralLimitRuleTests
{
    private readonly LoanAmountMustBeWithinGeneralLimitRule _rule = new();

    [Theory]
    [InlineData(100000)] // Minimum valid amount
    [InlineData(500000)] // Mid-range amount
    [InlineData(1500000)] // Maximum valid amount
    public void IsSatisfied_WithValidLoanAmount_ShouldReturnTrue(decimal loanAmount)
    {
        // Arrange
        var application = new LoanApplication(loanAmount, 1000000, 800);

        // Act
        var result = _rule.IsSatisfied(application);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(99999)] // Below minimum
    [InlineData(1500001)] // Above maximum
    [InlineData(0)] // Zero amount
    [InlineData(50000)] // Well below minimum
    public void IsSatisfied_WithInvalidLoanAmount_ShouldReturnFalse(decimal loanAmount)
    {
        // Arrange
        var application = new LoanApplication(loanAmount, 1000000, 800);

        // Act
        var result = _rule.IsSatisfied(application);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ExecuteFailure_ShouldAddAppropriateDeclineReason()
    {
        // Arrange
        var application = new LoanApplication(50000, 1000000, 800);
        application.DeclineReasons.Clear(); // Clear any existing reasons

        // Act
        _rule.ExecuteFailure(application);

        // Assert
        application.DeclineReasons.Should().HaveCount(1);
        application.DeclineReasons.First().Should().Contain("Loan amount must be between");
        application.DeclineReasons.First().Should().Contain("£100,000.00");
        application.DeclineReasons.First().Should().Contain("£1,500,000.00");
    }
}
