using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wize.settings.data.V1.Interfaces;
using wize.settings.data.V1.Models;

namespace wize.settings.data.V1.Repositories
{
    public class WizeSettingRepository : IWizeSettingRepository
    {
        private readonly ILogger<WizeSettingRepository> _logger;
        private readonly WizeContext _context;

        public WizeSettingRepository(ILogger<WizeSettingRepository> logger, WizeContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<WizeSetting>> GetByTypeAsync(string name)
        {
            var settings = await _context.WizeSettings.Where(w => w.Type == name).ToListAsync();
            return settings;
        }

        public async Task PutAsync(List<WizeSetting> model)
        {
            foreach(var setting in model)
            {
                var existing = await _context.WizeSettings.Where(w => w.Type == setting.Type && w.Name == setting.Name).SingleOrDefaultAsync();
                if(existing == null)
                {
                    await _context.WizeSettings.AddAsync(setting);
                }
                else
                {
                    if(existing.Name.ToLower().Contains("password") && string.IsNullOrEmpty(setting.Value))
                    {
                        continue;
                    }
                    existing.Value = setting.Value;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
