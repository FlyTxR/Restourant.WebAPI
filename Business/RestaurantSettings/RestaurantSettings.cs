using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Business.Settings
{
    public class RestaurantSettings
    {
        private Dictionary<string, string> _settings = new();

        public async Task LoadFromDatabaseAsync(SqlContext context)
        {
            var configs = await context.RestaurantConfigurations.ToListAsync();
            _settings = configs.ToDictionary(c => c.Key, c => c.Value);
        }

        public string GetValue(string key, string defaultValue = "")
        {
            return _settings.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if (_settings.TryGetValue(key, out var value) && int.TryParse(value, out var result))
            {
                return result;
            }
            return defaultValue;
        }

        public string[] GetArray(string key, string separator = ",")
        {
            if (_settings.TryGetValue(key, out var value))
            {
                return value.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => s.Trim())
                           .ToArray();
            }
            return Array.Empty<string>();
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            if (_settings.TryGetValue(key, out var value) && bool.TryParse(value, out var result))
            {
                return result;
            }
            return defaultValue;
        }

        public async Task ReloadAsync(SqlContext context)
        {
            await LoadFromDatabaseAsync(context);
        }

        public IReadOnlyDictionary<string, string> GetAll()
        {
            return _settings;
        }
    }

}
