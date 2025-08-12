using FluentAssertions;
using LendingPlatform.ConsoleApp.Models;
using LendingPlatform.ConsoleApp.Rules;
using Xunit;

namespace LendingPlatform.Tests.Rules;

public class ApplicantCreditScoreMustBeValidRuleTests
{
    private readonly ApplicantCreditScoreMustBeValidRule _rule = new();

    [Theory]
    [InlineData(1)] // Minimum valid score
    [InlineData(500)] // Mid-range score
    [InlineData(999)] // Maximum valid score
    [InlineData(750)] // Common boundary score
    public void IsSatisfied_WithValidCreditScore_ShouldReturnTrue(int creditScore)
    {
        // Arrange
        var application = new LoanApplication(500000, 1000000, creditScore);

        // Act
        var result = _rule.IsSatisfied(application);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)] // Below minimum
    [InlineData(1000)] // Above maximum
    [InlineData(-1)] // Negative score
    [InlineData(1500)] // Well above maximum
    public void IsSatisfied_WithInvalidCreditScore_ShouldReturnFalse(int creditScore)
    {
        // Arrange
        var application = new LoanApplication(500000, 1000000, creditScore);

        // Act
        var result = _rule.IsSatisfied(application);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ExecuteFailure_ShouldAddAppropriateDeclineReason()
    {
        // Arrange
        var application = new LoanApplication(500000, 1000000, 0);
        application.DeclineReasons.Clear();

        // Act
        _rule.ExecuteFailure(application);

        // Assert
        application.DeclineReasons.Should().HaveCount(1);
        application.DeclineReasons.First().Should().Contain("credit score must be between 1 and 999");
    }
}
