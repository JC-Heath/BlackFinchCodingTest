namespace LendingPlatform.ConsoleApp.Rules;

public class RulesEngine<T>(IEnumerable<IRule<T>> rules)
    where T : class
{
    private readonly List<IRule<T>> _rules =
        rules.ToList() ?? throw new ArgumentNullException(nameof(rules), "Rules cannot be null.");
    
    public void AddRule(IRule<T> rule)
    {
        _rules.Add(rule);
    }

    public void Execute(T asset)
    {
        if (asset == null)
        {
            throw new ArgumentNullException(nameof(asset), "Asset cannot be null.");
        }

        foreach (var rule in _rules)
        {
            if (rule.IsSatisfied(asset))
            {
                rule.ExecuteSuccess(asset);
            }
            else
            {
                rule.ExecuteFailure(asset);
            }
        }
    }
}