using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Comparision;

public class ItemComparerTests
{
    [Fact]
    public void WhenItemsAreExactSame_ThenExactMatchReturned()
    {
        var check1 = new Check("H218", "C03");
        var check2 = new Check("H217", "C03");
        var item1 = new Item(1, [check1, check2]);
        var item2 = new Item(1, [check1, check2]);

        var result = item1.Compare(item2);

        result.Should().Be(ComparisionOutcome.ExactMatch);
    }

    [Fact]
    public void WhenItemNumberIsDifferent_ThenExactMatchReturned()
    {
        var check1 = new Check("H218", "C03");
        var check2 = new Check("H217", "C03");
        var item1 = new Item(1, [check1, check2]);
        var item2 = new Item(2, [check1, check2]);

        var result = item1.Compare(item2);

        result.Should().Be(ComparisionOutcome.Mismatch);
    }

    [Fact]
    public void WhenChecksCountIsDifferent_ThenExactMatchReturned()
    {
        var check1 = new Check("H218", "C03");
        var check2 = new Check("H217", "C03");
        var item1 = new Item(1, [check1, check2]);
        var item2 = new Item(1, [check1]);

        var result = item1.Compare(item2);

        result.Should().Be(ComparisionOutcome.Mismatch);
    }

    [Fact]
    public void WhenCheckCodeNotExistOnAgainst_ThenMismatchReturned()
    {
        var check1 = new Check("H218", "C03");
        var check2 = new Check("H217", "C03");
        var item1 = new Item(1, [check2]);
        var item2 = new Item(1, [check1]);

        var result = item1.Compare(item2);

        result.Should().Be(ComparisionOutcome.Mismatch);
    }

    [Fact]
    public void WhenListOfItemsAreSame_ThenExactMatchReturned()
    {
        var check1 = new Check("H218", "C03");
        var check2 = new Check("H217", "C03");
        var item1 = new Item(1, [check1, check2]);
        var item2 = new Item(2, [check1, check2]);
        var list1 = new List<Item> { item1, item2 };
        var list2 = new List<Item> { item1, item2 };

        var result = list1.Compare(list2);

        result.Should().Be(ComparisionOutcome.ExactMatch);
    }

    [Fact]
    public void WhenItemsCountAreDifferent_ThenMismatchReturned()
    {
        var check1 = new Check("H218", "C03");
        var check2 = new Check("H217", "C03");
        var item1 = new Item(1, [check1, check2]);
        var item2 = new Item(2, [check1, check2]);
        var list1 = new List<Item> { item1, item2 };
        var list2 = new List<Item> { item2 };

        var result = list1.Compare(list2);

        result.Should().Be(ComparisionOutcome.Mismatch);
    }

    [Fact]
    public void WhenItemIfDifferent_ThenMismatchReturned()
    {
        var check1 = new Check("H218", "C03");
        var check2 = new Check("H217", "C03");
        var item1 = new Item(1, [check1, check2]);
        var item2 = new Item(2, [check1, check2]);
        var list1 = new List<Item> { item1 };
        var list2 = new List<Item> { item2 };

        var result = list1.Compare(list2);

        result.Should().Be(ComparisionOutcome.Mismatch);
    }
}
