using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace Defra.TradeImportsDecisionComparer.Testing;

public static class Endpoints
{
    public static class Decisions
    {
        private static string Root(string? prefix = null) => $"/{prefix}decisions";

        public static string Get(string mrn) => $"{Root()}/{mrn}";

        public static string Comparison(string mrn) => $"{Get(mrn)}/comparison";

        public static string Parity(DateTime? start, DateTime? end)
        {
            var queryString = QueryString.Empty;

            if (start.HasValue)
            {
                queryString = queryString.Add(nameof(start), start.Value.ToString("O", CultureInfo.InvariantCulture));
            }

            if (end.HasValue)
            {
                queryString = queryString.Add(nameof(end), end.Value.ToString("O", CultureInfo.InvariantCulture));
            }

            return $"{Root()}/parity{queryString.Value}";
        }

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
}
