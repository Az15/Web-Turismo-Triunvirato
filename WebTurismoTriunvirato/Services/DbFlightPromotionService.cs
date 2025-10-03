// Services/DbFlightPromotionService.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web_Turismo_Triunvirato.DataAccess;
using Web_Turismo_Triunvirato.Models;

namespace Web_Turismo_Triunvirato.Services
{
    public class DbFlightPromotionService : IFlightPromotionService
    {
        private readonly ApplicationDbContext _context;

        public DbFlightPromotionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FlightPromotion>> GetActiveFlightPromotionsAsync()
        {
            return await _context.FlightPromotions
                                 .Where(p => p.IsActive && p.EndDate >= System.DateTime.Today)
                                 .ToListAsync();
        }

        public async Task AddFlightPromotionAsync(FlightPromotion promotion)
        {
            _context.FlightPromotions.Add(promotion);
            await _context.SaveChangesAsync();
        }
    }
}