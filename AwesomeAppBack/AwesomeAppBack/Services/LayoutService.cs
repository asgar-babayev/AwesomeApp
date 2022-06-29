using AwesomeAppBack.DAL;
using AwesomeAppBack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwesomeAppBack.Services
{
    public class LayoutService
    {
        private readonly Context _context;
        public LayoutService(Context context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            return await _context.Settings.ToDictionaryAsync(p => p.Key, p => p.Value);
        }
    }
}
