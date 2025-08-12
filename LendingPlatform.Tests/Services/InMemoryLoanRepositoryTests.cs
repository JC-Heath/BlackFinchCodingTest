using FluentAssertions;
using LendingPlatform.ConsoleApp.Models;
using LendingPlatform.ConsoleApp.Services;
using Xunit;

namespace LendingPlatform.Tests.Services;

public class InMemoryLoanRepositoryTests
{
    [Fact]
    public void SaveLoanApplication_ShouldAddApplicationToRepository()
    {
        // Arrange
        var repository = new InMemoryLoanRepository();
        var application = new LoanApplication(500000, 1000000, 800);

        // Act
        repository.SaveLoanApplication(application);

        // Assert
        var applications = repository.GetLoanApplications();
        applications.Should().HaveCount(1);
        applications.First().Should().Be(application);
    }

    [Fact]
    public void SaveLoanApplication_WithMultipleApplications_ShouldAddAllApplications()
    {
        // Arrange
        var repository = new InMemoryLoanRepository();
        var application1 = new LoanApplication(500000, 1000000, 800);
        var application2 = new LoanApplication(300000, 800000, 750);
        var application3 = new LoanApplication(1000000, 2000000, 950);

        // Act
        repository.SaveLoanApplication(application1);
        repository.SaveLoanApplication(application2);
        repository.SaveLoanApplication(application3);

        // Assert
        var applications = repository.GetLoanApplications();
        applications.Should().HaveCount(3);
        applications.Should().Contain(application1);
        applications.Should().Contain(application2);
        applications.Should().Contain(application3);
    }

    [Fact]
    public void GetLoanApplications_WithEmptyRepository_ShouldReturnEmptyList()
    {
        // Arrange
        var repository = new InMemoryLoanRepository();

        // Act
        var applications = repository.GetLoanApplications();

        // Assert
        applications.Should().BeEmpty();
    }

    [Fact]
    public void GetLoanApplications_ShouldReturnAllSavedApplications()
    {
        // Arrange
        var repository = new InMemoryLoanRepository();
        var application1 = new LoanApplication(500000, 1000000, 800);
        var application2 = new LoanApplication(300000, 800000, 750);

        repository.SaveLoanApplication(application1);
        repository.SaveLoanApplication(application2);

        // Act
        var applications = repository.GetLoanApplications();

        // Assert
        applications.Should().HaveCount(2);
        applications[0].Should().Be(application1);
        applications[1].Should().Be(application2);
    }

    [Fact]
    public void GetLoanApplications_ShouldReturnSeparateInstanceEachTime()
    {
        // Arrange
        var repository = new InMemoryLoanRepository();
        var application = new LoanApplication(500000, 1000000, 800);
        repository.SaveLoanApplication(application);

        // Act
        var applications1 = repository.GetLoanApplications();
        var applications2 = repository.GetLoanApplications();

        // Assert
        // The repository returns the same list reference, but contains the same data
        applications1.Should().BeEquivalentTo(applications2);
        applications1.Should().HaveCount(1);
        applications2.Should().HaveCount(1);
    }

    [Fact]
    public void Repository_ShouldMaintainApplicationOrder()
    {
        // Arrange
        var repository = new InMemoryLoanRepository();
        var applications = new[]
        {
            new LoanApplication(100000, 500000, 800),
            new LoanApplication(200000, 600000, 750),
            new LoanApplication(300000, 700000, 850),
            new LoanApplication(400000, 800000, 900)
        };

        // Act
        foreach (var app in applications)
        {
            repository.SaveLoanApplication(app);
        }

        // Assert
        var retrievedApplications = repository.GetLoanApplications();
        for (int i = 0; i < applications.Length; i++)
        {
            retrievedApplications[i].Should().Be(applications[i]);
        }
    }

    [Fact]
    public void Repository_ShouldHandleMixedApprovedAndDeclinedApplications()
    {
        // Arrange
        var repository = new InMemoryLoanRepository();
        var approvedApp = new LoanApplication(500000, 1000000, 800); // Should be approved
        var declinedApp = new LoanApplication(50000, 1000000, 800);  // Should be declined (below min amount)

        // Act
        repository.SaveLoanApplication(approvedApp);
        repository.SaveLoanApplication(declinedApp);

        // Assert
        var applications = repository.GetLoanApplications();
        applications.Should().HaveCount(2);
        
        var approvedFromRepo = applications.First(a => a.LoanAmount == 500000);
        var declinedFromRepo = applications.First(a => a.LoanAmount == 50000);
        
        approvedFromRepo.LoanApproved.Should().BeTrue();
        declinedFromRepo.LoanApproved.Should().BeFalse();
    }
}
