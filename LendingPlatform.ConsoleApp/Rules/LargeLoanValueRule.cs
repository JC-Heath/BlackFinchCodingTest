using LendingPlatform.ConsoleApp.Constants;
using LendingPlatform.ConsoleApp.Models;

namespace LendingPlatform.ConsoleApp.Rules;

public class LargeLoanValueRule : IRule<LoanApplication>
{
    private const int MinimumCreditScore = 950;
    private const int MaximumLoanToValuePercent = 60;

    public bool IsSatisfied(LoanApplication asset)
    {
        if (asset.LoanAmount >= LoanApplicationConstants.LargeLoanAmount)
        {
            // For large value loans, we need to check the credit score and loan-to-value ratio
            return asset is
                { ApplicantsCreditScore: >= MinimumCreditScore, LoanToValuePercent: <= MaximumLoanToValuePercent };
        }

        return true; // Not a large value loan, so no additional checks needed
    }

    public void ExecuteFailure(LoanApplication asset)
    {
        asset.DeclineReasons.Add("Large loan value must have a credit score of at least 950 and LTV percent of 60% or less.");
    }
}