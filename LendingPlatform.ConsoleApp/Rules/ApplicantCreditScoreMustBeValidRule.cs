using LendingPlatform.ConsoleApp.Models;

namespace LendingPlatform.ConsoleApp.Rules;

public class ApplicantCreditScoreMustBeValidRule : IRule<LoanApplication>
{
    private const int MinimumCreditScore = 1;
    private const int MaximumCreditScore = 999;
    public bool IsSatisfied(LoanApplication asset)
    {
        // Check if the credit score is within the valid range
        return asset.ApplicantsCreditScore is >= MinimumCreditScore and <= MaximumCreditScore;
    }
    
    public void ExecuteFailure(LoanApplication asset)
    {
        asset.DeclineReasons.Add($"Applicant's credit score must be between {MinimumCreditScore} and {MaximumCreditScore}.");
    }
}