namespace iMicroFin.DAO
{
    public static class ConfigHelper
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetConnectionString(string name = "MySqlConnection")
        {
            return _configuration.GetConnectionString(name);
        }
    }
}
