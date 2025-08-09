// Services/IFlightPromotionService.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Models;

namespace Web_Turismo_Triunvirato.Services
{
    public interface IFlightPromotionService
    {
        Task<IEnumerable<FlightPromotion>> GetActiveFlightPromotionsAsync();
        Task AddFlightPromotionAsync(FlightPromotion promotion);
        // Puedes agregar más métodos (Update, Delete, GetById) más adelante
    }
}