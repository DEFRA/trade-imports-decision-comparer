using System.Diagnostics;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests;

public static class AsyncWaiter
{
    private static readonly TimeSpan s_defaultDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan s_defaultTimeout = TimeSpan.FromSeconds(30);

    public static async Task<bool> WaitForAsync(Func<Task<bool>> condition)
    {
        var timer = Stopwatch.StartNew();

        while (true)
        {
            try
            {
                if (await condition())
                    return true;
            }
            catch
            {
                // ignored
            }

            if (timer.Elapsed > s_defaultTimeout)
                return false;

            await Task.Delay(s_defaultDelay);
        }
    }
}
