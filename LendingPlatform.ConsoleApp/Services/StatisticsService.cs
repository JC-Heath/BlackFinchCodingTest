using System.Text.Json.Serialization;

namespace LendingPlatform.ConsoleApp.Services;

public class StatisticsService(ILoanRepository loanRepository)
{
    public (int numberOfApprovedLoans, int numberOfDeclinedLoans, decimal totalValueOfLoansWritten, string meanAverageLoadToValueRatio) GetStatistics()
    {
        var applications = loanRepository.GetLoanApplications().ToList();
        
        if (applications.Count == 0)
        {
            return (0, 0, 0, "0.00");
        }

        var applicationsByStatus = applications
            .GroupBy(x => x.LoanApproved)
            .ToDictionary(g => g.Key, g => g.Count());

        var totalLoansWritten = applications
            .Where(x => x.LoanApproved)
            .Sum(x => x.LoanAmount);

        var meanLtv = applications.Average(x => x.LoanToValuePercent);

        return (
            applicationsByStatus.GetValueOrDefault(true, 0),
            applicationsByStatus.GetValueOrDefault(false, 0),
            totalLoansWritten,
            meanLtv.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
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