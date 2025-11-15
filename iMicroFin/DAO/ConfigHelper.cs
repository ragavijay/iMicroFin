namespace iMicroFin.DAO
{
    public static class ConfigHelper
    {
        private static IConfiguration? _configuration; // mark as nullable

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public static string GetConnectionString(string name = "MySqlConnection")
        {
            if (_configuration == null)
                throw new InvalidOperationException("ConfigHelper is not initialized. Call Initialize() first.");

            var connString = _configuration.GetConnectionString(name);

            if (string.IsNullOrEmpty(connString))
                throw new InvalidOperationException($"Connection string '{name}' not found in configuration.");

            return connString;
        }
    }
}