namespace Defra.TradeImportsDecisionComparer.Comparer.Utils;

public interface IRandom
{
    double NextDouble();
}

public class RandomShared : IRandom
{
    public double NextDouble() => Random.Shared.NextDouble();
}
