using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Models;
using System;

namespace Web_Turismo_Triunvirato.Services
{
    public class InMemoryPromotionService : IPromotionService 
    {
        private readonly List<Promotion> _promotions = new List<Promotion>();
        private int _nextId = 1;

        public InMemoryPromotionService()
        {
            // Datos de prueba para promociones (adaptados a los nuevos campos)
            _promotions.Add(new Promotion
            {
                Id = _nextId++,
                ServiceType = ServiceType.Vuelos,
                Description = "Vuelo a Bariloche con 20% de descuento",
                DestinationName = "Bariloche",
                OriginName = "Buenos Aires",
                ImageUrl = "~/img/Bariloche.jpg",
                IsHotWeek = true,
                OriginalPrice = 1056067.00m,
                OfferPrice = 844853.60m, // 20% de descuento
                DiscountPercentage = 20,
                StartDate = new DateTime(2025, 7, 14), // Ejemplo: 10 días antes de hoy
                EndDate = new DateTime(2025, 8, 24), // Ejemplo: Un mes desde hoy
                IsActive = true
            });
            _promotions.Add(new Promotion
            {
                Id = _nextId++,
                ServiceType = ServiceType.Vuelos,
                Description = "Vuelo a Río de Janeiro",
                DestinationName = "Río de Janeiro",
                OriginName = "Buenos Aires",
                ImageUrl = "~/img/RiodeJaneiro.jpeg",
                IsHotWeek = false,
                OriginalPrice = 217460.00m,
                OfferPrice = 217460.00m,
                DiscountPercentage = 0,
                StartDate = new DateTime(2025, 7, 19), // Ejemplo: 5 días antes de hoy
                EndDate = new DateTime(2025, 9, 24), // Ejemplo: Dos meses desde hoy
                IsActive = true
            });
            _promotions.Add(new Promotion
            {
                Id = _nextId++,
                ServiceType = ServiceType.Vuelos,
                Description = "Vuelo a Barcelona con Hot Week",
                DestinationName = "Barcelona",
                OriginName = "Buenos Aires",
                ImageUrl = "~/img/Barcelona.jpeg",
                IsHotWeek = true,
                OriginalPrice = 932300.00m,
                OfferPrice = 745840.00m, // 20% de descuento
                DiscountPercentage = 20,
                StartDate = new DateTime(2025, 7, 17), // Ejemplo: 7 días antes de hoy
                EndDate = new DateTime(2025, 7, 31), // Ejemplo: 7 días después de hoy
                IsActive = true
            });
            _promotions.Add(new Promotion
            {
                Id = _nextId++,
                ServiceType = ServiceType.Vuelos,
                Description = "Vuelo a Miami",
                DestinationName = "Miami",
                OriginName = "Buenos Aires",
                ImageUrl = "~/img/Miami.jpeg",
                IsHotWeek = false,
                OriginalPrice = 675513.00m,
                OfferPrice = 675513.00m,
                DiscountPercentage = 0,
                StartDate = new DateTime(2025, 7, 22), // Ejemplo: 2 días antes de hoy
                EndDate = new DateTime(2025, 10, 24), // Ejemplo: Tres meses desde hoy
                IsActive = true
            });
            // Agrega más promociones de prueba para otros ServiceType si quieres
        }

        public Task AddPromotionAsync(Promotion promotion)
        {
            promotion.Id = _nextId++;
            if (promotion.OriginalPrice > 0)
            {
                promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
            }
            _promotions.Add(promotion);
            return Task.CompletedTask;
        }

        public Task DeletePromotionAsync(int id)
        {
            var promotionToRemove = _promotions.FirstOrDefault(p => p.Id == id);
            if (promotionToRemove != null)
            {
                _promotions.Remove(promotionToRemove);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Promotion>> GetAllPromotionsAsync()
        {
            return Task.FromResult(_promotions.AsEnumerable());
        }

        public Task<IEnumerable<Promotion>> GetPromotionsByTypeAsync(ServiceType type)
        {
            return Task.FromResult(_promotions.Where(p => p.ServiceType == type).AsEnumerable());
        }

        public Task<Promotion> GetPromotionByIdAsync(int id)
        {
            return Task.FromResult(_promotions.FirstOrDefault(p => p.Id == id));
        }

        public Task UpdatePromotionAsync(Promotion promotion)
        {
            var existingPromotion = _promotions.FirstOrDefault(p => p.Id == promotion.Id);
            if (existingPromotion != null)
            {
                existingPromotion.ServiceType = promotion.ServiceType;
                existingPromotion.Description = promotion.Description;
                existingPromotion.DestinationName = promotion.DestinationName;
                existingPromotion.OriginName = promotion.OriginName;
                existingPromotion.ImageUrl = promotion.ImageUrl;
                existingPromotion.IsHotWeek = promotion.IsHotWeek;
                existingPromotion.OriginalPrice = promotion.OriginalPrice;
                existingPromotion.OfferPrice = promotion.OfferPrice;
                if (existingPromotion.OriginalPrice > 0)
                {
                    existingPromotion.DiscountPercentage = Math.Round(((existingPromotion.OriginalPrice - existingPromotion.OfferPrice) / existingPromotion.OriginalPrice) * 100, 2);
                }
                existingPromotion.StartDate = promotion.StartDate;
                existingPromotion.EndDate = promotion.EndDate;
                existingPromotion.IsActive = promotion.IsActive;
            }
            return Task.CompletedTask;
        }
    }
}