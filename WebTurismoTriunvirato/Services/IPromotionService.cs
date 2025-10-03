using System.Collections.Generic;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Models; 

namespace Web_Turismo_Triunvirato.Services
{
    public interface IPromotionService
    {
        Task<IEnumerable<Promotion>> GetAllPromotionsAsync();
        Task<IEnumerable<Promotion>> GetPromotionsByTypeAsync(ServiceType type);
        Task<Promotion> GetPromotionByIdAsync(int id);
        Task AddPromotionAsync(Promotion promotion);
        Task UpdatePromotionAsync(Promotion promotion);
        Task DeletePromotionAsync(int id);
    }
}