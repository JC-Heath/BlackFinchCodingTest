using FluentAssertions;
using LendingPlatform.ConsoleApp.Models;
using Xunit;

namespace LendingPlatform.Tests.Entities;

public class LoanApplicationTests
{
    [Theory]
    [InlineData(100000, 200000, 800, true)] // Valid small loan with good credit
    [InlineData(1500000, 3000000, 950, true)] // Valid large loan at maximum
    [InlineData(500000, 1000000, 750, true)] // Small loan with 50% LTV and 750 credit
    public void LoanApplication_WithValidInputs_ShouldBeApproved(decimal loanAmount, decimal assetValue, int creditScore, bool expectedApproval)
    {
        // Act
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Assert
        application.LoanApproved.Should().Be(expectedApproval);
        application.DeclineReasons.Should().BeEmpty();
    }

    [Theory]
    [InlineData(99999, 200000, 800)] // Below minimum loan amount
    [InlineData(1500001, 3000000, 950)] // Above maximum loan amount
    [InlineData(0, 200000, 800)] // Zero loan amount
    public void LoanApplication_WithInvalidLoanAmount_ShouldBeDeclined(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Act
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Assert
        application.LoanApproved.Should().BeFalse();
        application.DeclineReasons.Should().Contain(reason => 
            reason.Contains("Loan amount must be between"));
    }

    [Theory]
    [InlineData(100000, 200000, 0)] // Below minimum credit score
    [InlineData(100000, 200000, 1000)] // Above maximum credit score
    [InlineData(100000, 200000, -1)] // Negative credit score
    public void LoanApplication_WithInvalidCreditScore_ShouldBeDeclined(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Act
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Assert
        application.LoanApproved.Should().BeFalse();
        application.DeclineReasons.Should().Contain(reason => 
            reason.Contains("credit score must be between"));
    }

    [Theory]
    [InlineData(1000000, 1500000, 949)] // Large loan with insufficient credit score
    [InlineData(1000000, 1600000, 950)] // Large loan with LTV > 60%
    [InlineData(1000000, 1500000, 950)] // Large loan with LTV exactly 66.67%
    public void LoanApplication_LargeLoanWithInvalidCriteria_ShouldBeDeclined(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Act
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Assert
        application.LoanApproved.Should().BeFalse();
        application.DeclineReasons.Should().Contain(reason => 
            reason.Contains("Large loan value"));
    }

    [Theory]
    [InlineData(500000, 1000000, 749)] // Small loan, LTV < 60%, credit score < 750
    [InlineData(500000, 700000, 799)] // Small loan, LTV < 80%, credit score < 800
    [InlineData(500000, 600000, 899)] // Small loan, LTV < 90%, credit score < 900
    [InlineData(500000, 550000, 999)] // Small loan, LTV >= 90%, any credit score
    public void LoanApplication_SmallLoanWithInvalidCriteria_ShouldBeDeclined(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Act
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Assert
        application.LoanApproved.Should().BeFalse();
        application.DeclineReasons.Should().Contain(reason => 
            reason.Contains("Small loan amount"));
    }

    [Theory]
    [InlineData(500000, 1000000, 50)] // 50% LTV
    [InlineData(400000, 600000, 66.67)] // 66.67% LTV
    [InlineData(450000, 500000, 90)] // 90% LTV
    public void LoanApplication_CalculatesLoanToValueCorrectly(decimal loanAmount, decimal assetValue, decimal expectedLtv)
    {
        // Act
        var application = new LoanApplication(loanAmount, assetValue, 800);

        // Assert
        application.LoanToValuePercent.Should().BeApproximately(expectedLtv, 0.01m);
    }

    [Fact]
    public void LoanApplication_WithZeroAssetValue_ShouldHaveZeroLTV()
    {
        // Act
        var application = new LoanApplication(100000, 0, 800);

        // Assert
        application.LoanToValuePercent.Should().Be(0);
        // Note: Zero asset value doesn't automatically decline the loan based on current business rules
        // The loan may still be approved if it meets other criteria
    }

    [Theory]
    [InlineData(1000000, 2000000, 950)] // Large loan: LTV = 50%, credit = 950
    [InlineData(1500000, 2500000, 999)] // Large loan: LTV = 60%, credit = 999
    public void LoanApplication_LargeLoanAtBoundaryConditions_ShouldBeApproved(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Act
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Assert
        application.LoanApproved.Should().BeTrue();
        application.DeclineReasons.Should().BeEmpty();
    }

    [Theory]
    [InlineData(500000, 1000000, 750)] // Small loan: LTV = 50%, credit = 750 (boundary)
    [InlineData(500000, 700000, 800)] // Small loan: LTV = 71.43%, credit = 800 (boundary)
    [InlineData(500000, 600000, 900)] // Small loan: LTV = 83.33%, credit = 900 (boundary)
    public void LoanApplication_SmallLoanAtBoundaryConditions_ShouldBeApproved(decimal loanAmount, decimal assetValue, int creditScore)
    {
        // Act
        var application = new LoanApplication(loanAmount, assetValue, creditScore);

        // Assert
        application.LoanApproved.Should().BeTrue();
        application.DeclineReasons.Should().BeEmpty();
    }
}
