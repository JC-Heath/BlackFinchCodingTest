using FluentAssertions;
using LendingPlatform.ConsoleApp.Models;
using LendingPlatform.ConsoleApp.Rules;
using Xunit;

namespace LendingPlatform.Tests.Rules;

public class RulesEngineTests
{
    [Fact]
    public void RulesEngine_WithNullRules_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RulesEngine<LoanApplication>(null!));
    }

    [Fact]
    public void Execute_WithNullAsset_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rules = new List<IRule<LoanApplication>>();
        var engine = new RulesEngine<LoanApplication>(rules);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => engine.Execute(null!));
    }

    [Fact]
    public void Execute_WithValidAsset_ShouldExecuteAllRules()
    {
        // Arrange
        var mockRule1 = new TestRule(true);
        var mockRule2 = new TestRule(false);
        var rules = new List<IRule<LoanApplication>> { mockRule1, mockRule2 };
        var engine = new RulesEngine<LoanApplication>(rules);
        var application = new LoanApplication(500000, 1000000, 800);

        // Act
        engine.Execute(application);

        // Assert
        mockRule1.WasExecuted.Should().BeTrue();
        mockRule2.WasExecuted.Should().BeTrue();
        mockRule1.ExecutedSuccess.Should().BeTrue();
        mockRule2.ExecutedFailure.Should().BeTrue();
    }

    [Fact]
    public void AddRule_ShouldAddRuleToEngine()
    {
        // Arrange
        var initialRules = new List<IRule<LoanApplication>>();
        var engine = new RulesEngine<LoanApplication>(initialRules);
        var newRule = new TestRule(true);
        var application = new LoanApplication(500000, 1000000, 800);

        // Act
        engine.AddRule(newRule);
        engine.Execute(application);

        // Assert
        newRule.WasExecuted.Should().BeTrue();
    }

    [Fact]
    public void Execute_WithEmptyRulesList_ShouldNotThrow()
    {
        // Arrange
        var rules = new List<IRule<LoanApplication>>();
        var engine = new RulesEngine<LoanApplication>(rules);
        var application = new LoanApplication(500000, 1000000, 800);

        // Act & Assert
        engine.Invoking(e => e.Execute(application)).Should().NotThrow();
    }

    private class TestRule : IRule<LoanApplication>
    {
        private readonly bool _isSatisfied;
        public bool WasExecuted { get; private set; }
        public bool ExecutedSuccess { get; private set; }
        public bool ExecutedFailure { get; private set; }

        public TestRule(bool isSatisfied)
        {
            _isSatisfied = isSatisfied;
        }

        public bool IsSatisfied(LoanApplication asset)
        {
            WasExecuted = true;
            return _isSatisfied;
        }

        public void ExecuteSuccess(LoanApplication asset)
        {
            ExecutedSuccess = true;
        }

        public void ExecuteFailure(LoanApplication asset)
        {
            ExecutedFailure = true;
        }
    }
}
