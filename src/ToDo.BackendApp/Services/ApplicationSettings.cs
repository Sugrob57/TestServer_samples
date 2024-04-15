using Microsoft.Extensions.Configuration;

namespace ToDo.BackendApp.Services
{
    public class ApplicationSettings
    {
        public ApplicationSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetSetting(string key)
        {
            return _configuration.GetSection($"AppSettings:{key}").Value;
        }

        private readonly IConfiguration _configuration;
    }
}