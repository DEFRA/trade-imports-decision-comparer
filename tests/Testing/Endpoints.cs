using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace Defra.TradeImportsDecisionComparer.Testing;

public static class Endpoints
{
    public static class Decisions
    {
        private static string Root(string? prefix = null) => $"/{prefix}decisions";

        public static string Get(string mrn) => $"{Root()}/{mrn}";

        public static class Alvs
        {
            private static string AlvsPrefix => nameof(Alvs).ToLower() + "-";

            public static string Put(string mrn) => $"{Root(AlvsPrefix)}/{mrn}";
        }

        public static class Btms
        {
            private static string BtmsPrefix => nameof(Btms).ToLower() + "-";

            public static string Put(string mrn) => $"{Root(BtmsPrefix)}/{mrn}";
        }
    }

    public static class OutboundErrors
    {
        private static string Root(string? prefix = null) => $"/{prefix}outbound-errors";

        public static string Get(string mrn) => $"{Root()}/{mrn}";

        public static class Alvs
        {
            private static string AlvsPrefix => nameof(Alvs).ToLower() + "-";

            public static string Put(string mrn) => $"{Root(AlvsPrefix)}/{mrn}";
        }

        public static class Btms
        {
            private static string BtmsPrefix => nameof(Btms).ToLower() + "-";

            public static string Put(string mrn) => $"{Root(BtmsPrefix)}/{mrn}";
        }
    }

    public static class Comparisons
    {
        private static string Root => "/comparisons";

        public static string Get(string mrn) => $"{Root}/{mrn}";
    }

    public static class Parity
    {
        private static string Root => "/parity";

        public static string Get(DateTime? start, DateTime? end)
        {
            var queryString = QueryString.Empty;

            if (start.HasValue)
            {
                queryString.Add(nameof(start), start.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (end.HasValue)
            {
                queryString.Add(nameof(end), end.Value.ToString(CultureInfo.InvariantCulture));
            }

            return $"{Root}?{queryString.Value}";
        }
    }
}
