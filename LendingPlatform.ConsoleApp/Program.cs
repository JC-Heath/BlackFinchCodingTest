using LendingPlatform.ConsoleApp.Models;
using LendingPlatform.ConsoleApp.Services;

// TODO would be DI..
ILoanRepository loanRepository = new InMemoryLoanRepository();
var statisticsService = new StatisticsService(loanRepository);

Console.WriteLine("Welcome to the Lending Platform!");
Console.WriteLine("Press 'q' to quit the application.");
Console.WriteLine("Press 'o' for loans output...");
Console.WriteLine("Press i to input a new loan application...");

// TODO: Would refactor into CQRS for readability..
do
{
    var input = Console.ReadLine()?.Trim().ToLower();
    if (string.IsNullOrEmpty(input)) continue;
    
    switch (input[0])
    {
        case 'q':
            Console.WriteLine("Exiting the application. Goodbye!");
            return;

        case 'i':
            Console .Write("Enter the loan amount: ");
            decimal loanAmount;
            while (!decimal.TryParse(Console.ReadLine(), out loanAmount) || loanAmount <= 0)
            {
                Console.Write("Invalid input. Please enter a valid loan amount: ");
            }
            
            Console.Write("Enter the secured asset value: ");
            decimal securedAssetValue;
            while (!decimal.TryParse(Console.ReadLine(), out securedAssetValue) || securedAssetValue <= 0)
            {
                Console.Write("Invalid input. Please enter a valid secured asset value: ");
            }
            
            int applicantsCreditScore;
            Console.Write("Enter the applicant's credit score (1-999): ");
            while (!int.TryParse(Console.ReadLine(), out applicantsCreditScore))
            {
                Console.Write("Invalid input. Please enter a valid credit score (0-999): ");
            }
            
            var loanApplication = new LoanApplication(
                loanAmount, securedAssetValue, applicantsCreditScore);
            
            loanRepository.SaveLoanApplication(loanApplication);
            
            Console.WriteLine("Loan application saved!");
            break;
        case 'o':
            Console.WriteLine(statisticsService.GetStatisticsAsJson());
            break;

        default:
            Console.WriteLine("Invalid option. Please press 'q' to quit, 'o' for loans output, or 'i' to input a new loan application.");
            break;
    }
} while (true);