using Business.DTO;
using Business.Settings;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly SqlContext _context;
        private readonly RestaurantSettings _settings;

        public ConfigurationController(SqlContext context, RestaurantSettings settings)
        {
            _context = context;
            _settings = settings;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllSettings()
        {
            var configs = await _context.RestaurantConfigurations.ToListAsync();
            return Ok(configs);
        }

        [HttpGet("{key}")]
        public async Task<ActionResult> GetSetting(string key)
        {
            var config = await _context.RestaurantConfigurations
                .FirstOrDefaultAsync(c => c.Key == key);

            if (config == null)
            {
                return NotFound();
            }

            return Ok(config);
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> UpdateSetting(string key, [FromBody] UpdateConfigurationDTO dto)
        {
            var config = await _context.RestaurantConfigurations
                .FirstOrDefaultAsync(c => c.Key == key);

            if (config == null)
            {
                return NotFound();
            }

            config.Value = dto.Value;
            config.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _settings.ReloadAsync(_context);

            return Ok(new { message = "Configurazione aggiornata" });
        }

        [HttpPost]
        public async Task<ActionResult> CreateSetting(CreateConfigurationDTO dto)
        {
            var exists = await _context.RestaurantConfigurations
                .AnyAsync(c => c.Key == dto.Key);

            if (exists)
            {
                return BadRequest("Chiave già esistente");
            }

            var config = new RestaurantConfiguration
            {
                Key = dto.Key,
                Value = dto.Value,
                Description = dto.Description,
                UpdatedAt = DateTime.UtcNow
            };

            _context.RestaurantConfigurations.Add(config);
            await _context.SaveChangesAsync();
            await _settings.ReloadAsync(_context);

            return CreatedAtAction(nameof(GetSetting), new { key = config.Key }, config);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteSetting(string key)
        {
            var config = await _context.RestaurantConfigurations
                .FirstOrDefaultAsync(c => c.Key == key);

            if (config == null)
            {
                return NotFound();
            }

            _context.RestaurantConfigurations.Remove(config);
            await _context.SaveChangesAsync();
            await _settings.ReloadAsync(_context);

            return NoContent();
        }

        [HttpPost("reload")]
        public async Task<IActionResult> ReloadSettings()
        {
            await _settings.ReloadAsync(_context);
            return Ok(new { message = "Configurazione ricaricata" });
        }
    }
}
