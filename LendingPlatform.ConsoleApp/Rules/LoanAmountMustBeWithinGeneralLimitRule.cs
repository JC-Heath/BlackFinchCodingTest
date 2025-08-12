using LendingPlatform.ConsoleApp.Models;

namespace LendingPlatform.ConsoleApp.Rules;

public class LoanAmountMustBeWithinGeneralLimitRule : IRule<LoanApplication>
{
    private const decimal MinimumLoanAmount = 100_000m;
    private const decimal MaximumLoanAmount = 1_500_000m;

    public bool IsSatisfied(LoanApplication asset)
    {
        return asset.LoanAmount is >= MinimumLoanAmount and <= MaximumLoanAmount;
    }

    public void ExecuteFailure(LoanApplication asset)
    {
        asset.DeclineReasons.Add($"Loan amount must be between {MinimumLoanAmount:C} and {MaximumLoanAmount:C}.");
    }
}