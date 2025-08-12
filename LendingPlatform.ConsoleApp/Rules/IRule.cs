namespace LendingPlatform.ConsoleApp.Rules;

public interface IRule<in T>
    where T : class
{
    bool IsSatisfied(T asset);

    void ExecuteSuccess(T asset)
    {
        // Default implementation can be overridden if needed
    }
    void ExecuteFailure(T asset)
    {
        // Default implementation can be overridden if needed
    }
}