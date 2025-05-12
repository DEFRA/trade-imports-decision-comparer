using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Comparision;

public static class ItemComparer
{
    public static ComparisionOutcome Compare(this List<Item> items, List<Item> against)
    {
        if (items.Count != against.Count)
        {
            return ComparisionOutcome.Mismatch;
        }

        var outcomes = new HashSet<ComparisionOutcome>();
        foreach (var item in items)
        {
            var againstItem = against.Find(x => x.ItemNumber == item.ItemNumber);

            if (againstItem is null)
            {
                outcomes.Add(ComparisionOutcome.Mismatch);
                break;
            }

            outcomes.Add(item.Compare(againstItem));
        }

        return outcomes.Max();
    }

    public static ComparisionOutcome Compare(this Item item, Item against)
    {
        if (!item.ItemNumber.Equals(against.ItemNumber))
        {
            return ComparisionOutcome.Mismatch;
        }

        if (item.Checks.Count != against.Checks.Count)
        {
            return ComparisionOutcome.Mismatch;
        }

        var outcomes = new HashSet<ComparisionOutcome>();
        foreach (var check in item.Checks)
        {
            var againstCheck = against.Checks.Find(x => x.CheckCode == check.CheckCode);

            if (againstCheck is null)
            {
                outcomes.Add(ComparisionOutcome.Mismatch);
                break;
            }

            outcomes.Add(check.Compare(againstCheck));
        }

        return outcomes.Max();
    }
}
