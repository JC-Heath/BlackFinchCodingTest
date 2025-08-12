using LendingPlatform.ConsoleApp.Rules;

namespace LendingPlatform.ConsoleApp.Models;

public class LoanApplication
{
    public decimal LoanAmount { get; private set; }
    public decimal SecuredAssetValue { get; private set; }
    public int ApplicantsCreditScore { get; private set; }
    public decimal LoanToValuePercent => SecuredAssetValue == 0 ? 0 : LoanAmount / SecuredAssetValue * 100;
    public bool LoanApproved { get; set; }
    public List<string> DeclineReasons { get; } = [];

    public LoanApplication(
        decimal loanAmount,
        decimal securedAssetValue,
        int applicantsCreditScore)
    {
        LoanAmount = loanAmount;
        SecuredAssetValue = securedAssetValue;
        ApplicantsCreditScore = applicantsCreditScore;

        var engine = new RulesEngine<LoanApplication>([
            new ApplicantCreditScoreMustBeValidRule(),
            new LoanAmountMustBeWithinGeneralLimitRule(),
            new LargeLoanValueRule(),
            new SmallLoanAmountMustHaveValidCreditScoreForLtv()
        ]);

        engine.Execute(this);

        LoanApproved = DeclineReasons.Count == 0;
    }
}