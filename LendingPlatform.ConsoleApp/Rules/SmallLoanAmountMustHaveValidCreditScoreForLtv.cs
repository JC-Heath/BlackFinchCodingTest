using LendingPlatform.ConsoleApp.Models;

namespace LendingPlatform.ConsoleApp.Rules;

public class SmallLoanAmountMustHaveValidCreditScoreForLtv : IRule<LoanApplication>
{
    public bool IsSatisfied(LoanApplication asset)
    {
        if (asset.LoanAmount < Constants.LoanApplicationConstants.LargeLoanAmount)
        {
            // For small value loans, we need to check the credit score and loan-to-value ratio

            return asset.LoanToValuePercent switch
            {
                < 60 => asset.ApplicantsCreditScore >= 750,
                < 80 => asset.ApplicantsCreditScore >= 800,
                < 90 => asset.ApplicantsCreditScore >= 900,
                >= 90 => false // Too high LTV for small loan amounts
            };
        }

        return true; // Not a small value loan, so no additional checks needed
    }
    
    public void ExecuteFailure(LoanApplication asset)
    {
        asset.DeclineReasons.Add("Small loan amount must have a valid credit score for the LTV ratio.");
    }
}