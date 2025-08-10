// DataAccess/ApplicationDbContext.cs (o el path donde lo tengas)
using Web_Turismo_Triunvirato.Models;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MySqlConnector;
using System;

namespace Web_Turismo_Triunvirato.DataAccess // Ajusta el namespace a tu proyecto
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para cada una de tus tablas existentes
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<View_Index_Destination> Destinations { get; set; }
        public DbSet<View_Index_DestinationCarouselItem> View_DestinationCarouselItems { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<User> Users { get; set; }

        // Nuevo: DbSet para las promociones
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<FlightPromotion> FlightPromotions { get; set; }
        public DbSet<HotelPromotion> HotelPromotions { get; set; }
        public DbSet<BusPromotion> BusPromotions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Nuevo: Mapeo del modelo Promotion a la tabla view_promotionHoteles
            modelBuilder.Entity<Promotion>().ToTable("view_promotionHoteles");

            // Asegúrate de que las otras entidades están correctamente mapeadas si lo requieren
            modelBuilder.Entity<BusPromotion>().ToTable("BusPromotions");
            modelBuilder.Entity<HotelPromotion>().ToTable("HotelPromotions");
            modelBuilder.Entity<FlightPromotion>().ToTable("FlightPromotions");

            base.OnModelCreating(modelBuilder);
        }

        // --- Métodos para obtener datos (Stored Procedures) ---

        public async Task<List<View_Index_DestinationCarouselItem>> GetCarouselItemsAsync()
        {
            return await View_DestinationCarouselItems
                                 .FromSqlRaw("CALL GetActiveCarouselItems()")
                                 .ToListAsync();
        }

        public async Task<List<View_Index_Destination>> GetHotDestinyItemsAsync()
        {
            return await Destinations
                                 .FromSqlRaw("CALL GetActiveHotDestiny()")
                                 .ToListAsync();
        }

        public async Task<List<FlightPromotion>> GetActiveflightpromotionsItemsAsync()
        {
            return await FlightPromotions
                                 .FromSqlRaw("CALL GetActivePromotionFlights()")
                                 .ToListAsync();
        }

        public async Task<List<HotelPromotion>> GetActivePromotionHotelsItemsAsync()
        {
            return await HotelPromotions
                                 .FromSqlRaw("CALL GetActivePromotionHotels()")
                                 .ToListAsync();
        }

        // CORREGIDO: Renombrado el método para seguir una convención consistente
        public async Task<List<BusPromotion>> GetActivePromotionBusesItemsAsync()
        {
            return await BusPromotions
                                 .FromSqlRaw("CALL GetActivePromotionBuses()")
                                 .ToListAsync();
        }

        // --- Métodos para el ABM (Stored Procedures) ---

        // Método para gestionar el ABM de FlightPromotion
        public async Task AbmFlightPromotionAsync(FlightPromotion promotionFlight, string typeExecuted)
        {
            string sql = "CALL SetActivePromotionFlights(@p_id, @p_servicetype, @p_description, @p_destinationname, @p_originname, @p_imageurl, @p_ishotweek, @p_originalprice, @p_offerprice, @p_discountpercentage, @p_startdate, @p_enddate, @p_isactive, @p_stars, @p_typeexecuted)";

            var parameters = new MySqlParameter[]
            {
                // CORREGIDO: Los parámetros deben ser del tipo correcto. Se usa DbNull.Value para los nulos.
                new MySqlParameter("p_id", promotionFlight.Id > 0 ? (object)promotionFlight.Id : DBNull.Value),
                new MySqlParameter("p_servicetype", promotionFlight.ServiceType),
                new MySqlParameter("p_description", promotionFlight.Description),
                new MySqlParameter("p_destinationname", promotionFlight.DestinationName),
                new MySqlParameter("p_originname", promotionFlight.OriginName),
                new MySqlParameter("p_imageurl", promotionFlight.ImageUrl),
                new MySqlParameter("p_ishotweek", promotionFlight.IsHotWeek),
                new MySqlParameter("p_originalprice", promotionFlight.OriginalPrice),
                new MySqlParameter("p_offerprice", promotionFlight.OfferPrice),
                new MySqlParameter("p_discountpercentage", promotionFlight.DiscountPercentage),
                new MySqlParameter("p_startdate", promotionFlight.StartDate),
                new MySqlParameter("p_enddate", promotionFlight.EndDate),
                new MySqlParameter("p_isactive", promotionFlight.IsActive),
                new MySqlParameter("p_stars", promotionFlight.Stars),
                new MySqlParameter("p_typeexecuted", typeExecuted)
            };
            await Database.ExecuteSqlRawAsync(sql, parameters);
        }

        // Método para gestionar el alta, la baja y la modificación de promociones de hoteles
        public async Task AbmHotelPromotionAsync(HotelPromotion promotion, string typeExecuted)
        {
            string sql = "CALL SetActivePromotionHotels(@p_id, @p_servicetype, @p_description, @p_destinationname, @p_hotelname, @p_imageurl, @p_ishotweek, @p_originalprice, @p_offerprice, @p_discountpercentage, @p_startdate, @p_enddate, @p_isactive, @p_stars, @p_typeexecuted)";

            var parameters = new MySqlParameter[]
            {
                // CORREGIDO: Usa DbNull.Value para el ID en caso de INSERT
                new MySqlParameter("p_id", promotion.Id > 0 ? (object)promotion.Id : DBNull.Value),
                new MySqlParameter("p_servicetype", promotion.ServiceType),
                new MySqlParameter("p_description", promotion.Description),
                new MySqlParameter("p_destinationname", promotion.DestinationName),
                //new MySqlParameter("p_hotelname", promotion.HotelName),
                new MySqlParameter("p_imageurl", promotion.ImageUrl),
                new MySqlParameter("p_ishotweek", promotion.IsHotWeek),
                new MySqlParameter("p_originalprice", promotion.OriginalPrice),
                new MySqlParameter("p_offerprice", promotion.OfferPrice),
                new MySqlParameter("p_discountpercentage", promotion.DiscountPercentage),
                new MySqlParameter("p_startdate", promotion.StartDate),
                new MySqlParameter("p_enddate", promotion.EndDate),
                new MySqlParameter("p_isactive", promotion.IsActive),
                new MySqlParameter("p_stars", promotion.Stars),
                new MySqlParameter("p_typeexecuted", typeExecuted)
            };

            await Database.ExecuteSqlRawAsync(sql, parameters);
        }

        // NUEVO MÉTODO COMPLETO PARA BUSES
        public async Task AbmBusPromotionAsync(BusPromotion promotion, string typeExecuted)
        {
            // CORREGIDO: El nombre del SP y los parámetros deben coincidir con la tabla BusPromotions
            string sql = "CALL SetActivePromotionBuses(@p_id, @p_servicetype, @p_description, @p_destinationname, @p_originname, @p_buscompanyname, @p_category, @p_imageurl, @p_ishotweek, @p_originalprice, @p_offerprice, @p_discountpercentage, @p_startdate, @p_enddate, @p_isactive, @p_typeexecuted)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_id", promotion.Id > 0 ? (object)promotion.Id : DBNull.Value),
                new MySqlParameter("p_servicetype", promotion.ServiceType),
                new MySqlParameter("p_description", promotion.Description),
                new MySqlParameter("p_destinationname", promotion.DestinationName),
                new MySqlParameter("p_originname", promotion.OriginName),
                new MySqlParameter("p_buscompanyname", promotion.BusCompanyName),
                new MySqlParameter("p_category", promotion.Category),
                new MySqlParameter("p_imageurl", promotion.ImageUrl),
                new MySqlParameter("p_ishotweek", promotion.IsHotWeek),
                new MySqlParameter("p_originalprice", promotion.OriginalPrice),
                new MySqlParameter("p_offerprice", promotion.OfferPrice),
                new MySqlParameter("p_discountpercentage", promotion.DiscountPercentage),
                new MySqlParameter("p_startdate", promotion.StartDate),
                new MySqlParameter("p_enddate", promotion.EndDate),
                new MySqlParameter("p_isactive", promotion.IsActive),
                new MySqlParameter("p_typeexecuted", typeExecuted)
            };

            await Database.ExecuteSqlRawAsync(sql, parameters);
        }


        // NUEVO MÉTODO para obtener una promoción de bus por ID
        public async Task<BusPromotion> GetBusPromotionByIdAsync(int id)
        {
            return await BusPromotions.FirstOrDefaultAsync(p => p.Id == id);
        }

        // NUEVO MÉTODO para obtener una promoción de hotel por ID
        public async Task<HotelPromotion> GetHotelPromotionByIdAsync(int id)
        {
            return await HotelPromotions.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}