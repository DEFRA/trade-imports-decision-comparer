using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Comparision;

public class CheckComparerTests
{
    [Fact]
    public void WhenChecksAreExactSame_ThenExactMatchReturned()
    {
        var check1 = new Check("H218", "C03");
        var check2 = new Check("H218", "C03");

        var result = check1.Compare(check2);

        result.Should().Be(ComparisionOutcome.ExactMatch);
    }

    [Fact]
    public void WhenCheckCodeDifferenent_ThenMismatchReturned()
    {
        var check1 = new Check("H219", "C03");
        var check2 = new Check("H218", "C03");

        var result = check1.Compare(check2);

        result.Should().Be(ComparisionOutcome.Mismatch);
    }

    [Fact]
    public void WhenDecisionCodeIsDifferent_ThenMismatchReturned()
    {
        var check1 = new Check("H218", "D03");
        var check2 = new Check("H218", "C03");

        var result = check1.Compare(check2);

        result.Should().Be(ComparisionOutcome.Mismatch);
    }

    [Fact]
    public void WhenDecisionCodeIsDifferentBySameGroup_ThenGroupMatchReturned()
    {
        var check1 = new Check("H218", "C04");
        var check2 = new Check("H218", "C03");

        var result = check1.Compare(check2);

        result.Should().Be(ComparisionOutcome.GroupMatch);
    }
}
