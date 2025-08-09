using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web_Turismo_Triunvirato.DataAccess;
using Web_Turismo_Triunvirato.Models;
using System;

namespace Web_Turismo_Triunvirato.Services
{
    public class DbPromotionService : IPromotionService
    {
        private readonly ApplicationDbContext _context;

        public DbPromotionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddPromotionAsync(Promotion promotion)
        {
            // La nueva promoción se agrega directamente a la colección principal
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Promotion>> GetAllPromotionsAsync()
        {
            return await _context.Promotions.ToListAsync();
        }

        public async Task<IEnumerable<Promotion>> GetPromotionsByTypeAsync(ServiceType type)
        {
            return await _context.Promotions.Where(p => p.ServiceType == type && p.IsActive && p.EndDate >= DateTime.Today).ToListAsync();
        }

        public async Task<Promotion> GetPromotionByIdAsync(int id)
        {
            return await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdatePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePromotionAsync(int id)
        {
            var promotionToRemove = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id);
            if (promotionToRemove != null)
            {
                _context.Promotions.Remove(promotionToRemove);
                await _context.SaveChangesAsync();
            }
        }
    }
}