// DataAccess/ApplicationDbContext.cs (o el path donde lo tengas)
using Web_Turismo_Triunvirato.Models;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MySqlConnector;


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

        // Nuevo: DbSet para las promociones de hoteles
        public DbSet<Promotion> Promotions { get; set; }

        public DbSet<FlightPromotion> FlightPromotions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Nuevo: Mapeo del modelo Promotion a la tabla view_promotionHoteles
            modelBuilder.Entity<Promotion>().ToTable("view_promotionHoteles");

            // (Tu código de mapeo de otras entidades si las tienes)

            base.OnModelCreating(modelBuilder);
        }

        // --- El método modificado para llamar al Stored Procedure ---
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

        // Método para ejecutar el ABM de FlightPromotion
        public async Task AbmFlightPromotionAsync(FlightPromotion promotionFlight, string typeExecuted)
        {

            string sql = "CALL SetActivePromotionFlights(@p_id, @p_servicetype, @p_description, @p_destinationname, @p_originname, @p_imageurl, @p_ishotweek, @p_originalprice, @p_offerprice, @p_discountpercentage, @p_startdate, @p_enddate, @p_isactive, @p_stars, @p_typeexecuted)";

            var parameters = new MySqlParameter[]
            {  
            new MySqlParameter("p_id", promotionFlight.Id == 0 ? DBNull.Value : (object)promotionFlight.Id),
            new MySqlParameter("p_servicetype","1"),
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
    }
}