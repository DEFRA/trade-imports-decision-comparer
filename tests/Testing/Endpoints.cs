namespace Defra.TradeImportsDecisionComparer.Testing;

public static class Endpoints
{
    public static class Decisions
    {
        public static class Alvs
        {
            private const string Root = "/alvs-decisions";

            public static string Put(string mrn) => $"{Root}/{mrn}";
        }

        public static class Btms
        {
            private const string Root = "/btms-decisions";

            public static string Put(string mrn) => $"{Root}/{mrn}";
        }
    }
}
