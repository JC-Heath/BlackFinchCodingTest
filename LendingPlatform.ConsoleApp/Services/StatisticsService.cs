using System.Text.Json.Serialization;

namespace LendingPlatform.ConsoleApp.Services;

public class StatisticsService(ILoanRepository loanRepository)
{
    private Dictionary<bool, int> TotalNumberOfApplicationsBySuccessStatus => loanRepository.GetLoanApplications()
        .GroupBy(x => x.LoanApproved)
        .ToDictionary(g => g.Key, g => g.Count());

    private decimal TotalNumberOfLoansWrittenToDate => loanRepository.GetLoanApplications()
        .Where(x => x.LoanApproved)
        .Sum(x => x.LoanAmount);

    private decimal MeanLoanToValueRatio =>
        loanRepository.GetLoanApplications()
            .Average(x => x.LoanToValuePercent);
    

    public (int numberOfApprovedLoans, int numberOfDeclinedLoans, decimal totalValueOfLoansWritten, string meanAverageLoadToValueRatio) GetStatistics()
    {
        if (TotalNumberOfApplicationsBySuccessStatus.Count == 0)
        {
            return (0, 0, 0, "0.00");
        }
        return (
            TotalNumberOfApplicationsBySuccessStatus.GetValueOrDefault(true, 0),
            TotalNumberOfApplicationsBySuccessStatus.GetValueOrDefault(false, 0),
            TotalNumberOfLoansWrittenToDate,
            MeanLoanToValueRatio.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
        );
    }

    public class StatisticsResult
    {
        public int NumberOfApprovedLoans { get; set; }
        public int NumberOfDeclinedLoans { get; set; }
        public decimal TotalValueOfLoansWritten { get; set; }
        public string MeanAverageLoanToValueRatio { get; set; } = "0.00";
    }

    public StatisticsResult GetStatisticsAsObject()
    {
        var (approved, declined, total, mean) = GetStatistics();
        return new StatisticsResult
        {
            NumberOfApprovedLoans = approved,
            NumberOfDeclinedLoans = declined,
            TotalValueOfLoansWritten = total,
            MeanAverageLoanToValueRatio = mean
        };
    }

    public string GetStatisticsAsJson()
    {
        var stats = GetStatisticsAsObject();
        return System.Text.Json.JsonSerializer.Serialize(stats);
    }
}