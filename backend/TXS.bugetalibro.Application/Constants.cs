namespace TXS.bugetalibro.Application
{
    public class Constants
    {
        public const string ApplicationIdentifier = "bugetalibro";

        public static class Environment
        {
            public const string Testing = nameof(Testing);
            public const string Development = nameof(Development);
            public const string Production = nameof(Production);
        }

        public static class ConnectionStringKeys
        {
            public const string Database = nameof(Database);
        }
    }
}
