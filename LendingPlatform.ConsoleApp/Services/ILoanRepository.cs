using LendingPlatform.ConsoleApp.Models;

namespace LendingPlatform.ConsoleApp.Services;

public interface ILoanRepository
{
    void SaveLoanApplication(LoanApplication loanApplication);
    List<LoanApplication> GetLoanApplications();
}

public class InMemoryLoanRepository : ILoanRepository
{
    private readonly List<LoanApplication> _loanApplications = [];

    public void SaveLoanApplication(LoanApplication loanApplication)
    {
        _loanApplications.Add(loanApplication);
    }

    public List<LoanApplication> GetLoanApplications()
    {
        return _loanApplications;
    }
}