// DataAccess/ApplicationDbContext.cs
using Web_Turismo_Triunvirato.Models;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MySqlConnector;
using System;
using System.Data;
using System.Diagnostics;

namespace Web_Turismo_Triunvirato.DataAccess
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

        // DbSets para las promociones

        public DbSet<WhatsappMessage> WhatsappMessages { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<FlightPromotion> FlightPromotions { get; set; }
        public DbSet<HotelPromotion> HotelPromotions { get; set; }
        public DbSet<BusPromotion> BusPromotions { get; set; }
        // NUEVO: DbSet para las promociones de paquetes
        public DbSet<PackagePromotion> PackagePromotions { get; set; }
        public DbSet<EncomiendaCompany> EncomiendaCompanies { get; set; }
        public DbSet<ActivitiesPromotion> Activities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeo de entidades a tablas/vistas
            modelBuilder.Entity<Promotion>().ToTable("view_promotionHoteles");
            modelBuilder.Entity<BusPromotion>().ToTable("BusPromotions");
            modelBuilder.Entity<HotelPromotion>().ToTable("HotelPromotions");
            modelBuilder.Entity<FlightPromotion>().ToTable("FlightPromotions");
            modelBuilder.Entity<PackagePromotion>().ToTable("PackagePromotions"); // NUEVO: Mapeo de PackagePromotion

            base.OnModelCreating(modelBuilder);
        }

        // --- Métodos para obtener datos (Stored Procedures) ---

        public async Task<List<FlightPromotion>> GetViewFlightpromotionsItemsAsync()
        {
            return await FlightPromotions
                                 .FromSqlRaw("CALL GetViewFlights()")
                                 .ToListAsync();
        }
        public async Task<List<HotelPromotion>> GetViewHotelspromotionsItemsAsync()
        {
            return await HotelPromotions
                                 .FromSqlRaw("CALL GetViewHotels()")
                                 .ToListAsync();
        }

        public async Task<List<BusPromotion>> GetViewBusspromotionsItemsAsync()
        {
            return await BusPromotions
                                 .FromSqlRaw("CALL GetViewBuses()")
                                 .ToListAsync();
        }

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

        public async Task<List<BusPromotion>> GetActivePromotionBusesItemsAsync()
        {
            return await BusPromotions
                                         .FromSqlRaw("CALL GetActivePromotionBuses()")
                                         .ToListAsync();
        }

        // NUEVO: Método para obtener promociones de paquetes activas
        public async Task<List<PackagePromotion>> GetActivePromotionPackagesItemsAsync()
        {
            return await PackagePromotions
                                         .FromSqlRaw("CALL GetActivePromotionPackages()")
                                         .ToListAsync();
        }

        // --- Métodos para el ABM (Stored Procedures) ---

        // Método para gestionar el ABM de FlightPromotion
        public async Task AbmFlightPromotionAsync(FlightPromotion promotionFlight, string typeExecuted)
        {
            // CORREGIDO: Conversión segura del ServiceType a un entero
            int serviceType;
            if (!int.TryParse(promotionFlight.ServiceType, out serviceType))
            {
                throw new ArgumentException("El ServiceType del vuelo debe ser un valor numérico.", nameof(promotionFlight.ServiceType));
            }


            string sql = "CALL SetActivePromotionFlights(@p_id, @p_servicetype, @p_description,@p_whatsapp_id, @p_destinationname, @p_originname, @p_imageurl, @p_ishotweek, @p_originalprice, @p_offerprice, @p_discountpercentage, @p_startdate, @p_enddate, @p_isactive, @p_stars, @p_typeexecuted)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_id", promotionFlight.Id > 0 ? (object)promotionFlight.Id : DBNull.Value),
                new MySqlParameter("p_Whatsapp_Id", promotionFlight.Whatsapp_Id),
                new MySqlParameter("p_servicetype", MySqlDbType.Int32) { Value = serviceType },
                new MySqlParameter("p_description", promotionFlight.Description),
                new MySqlParameter("p_whatsapp_id", promotionFlight.Whatsapp_Id),
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
            // CORREGIDO: Conversión segura del ServiceType a un entero
            int serviceType;
            if (!int.TryParse(promotion.ServiceType, out serviceType))
            {
                throw new ArgumentException("El ServiceType del hotel debe ser un valor numérico.", nameof(promotion.ServiceType));
            }

            string sql = "CALL SetActivePromotionHotels(@p_id, @p_servicetype, @p_description, @p_destinationname, @p_hotelname, @p_imageurl, @p_ishotweek, @p_originalprice, @p_offerprice, @p_discountpercentage, @p_startdate, @p_enddate, @p_isactive, @p_stars, @p_typeexecuted)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_id", promotion.Id > 0 ? (object)promotion.Id : DBNull.Value),
                // CORREGIDO: Se usa la variable 'serviceType' que ya es un int
                new MySqlParameter("p_servicetype", MySqlDbType.Int32) { Value = serviceType },
                new MySqlParameter("p_description", promotion.Description),
                new MySqlParameter("p_description", promotion.Description),
                new MySqlParameter("p_whatsapp_id", promotion.Whatsapp_Id),
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

        // Método para gestionar el ABM de BusPromotion
        public async Task AbmBusPromotionAsync(BusPromotion promotion, string typeExecuted)
        {
            // CORREGIDO: Conversión segura del ServiceType a un entero
            int serviceType;
            if (!int.TryParse(promotion.ServiceType, out serviceType))
            {
                throw new ArgumentException("El ServiceType del bus debe ser un valor numérico.", nameof(promotion.ServiceType));
            }

            string sql = "CALL SetActivePromotionBuses(@p_id, @p_servicetype, @p_description, @p_destinationname, @p_originname, @p_buscompanyname, @p_category, @p_imageurl, @p_ishotweek, @p_originalprice, @p_offerprice, @p_discountpercentage, @p_startdate, @p_enddate, @p_isactive, @p_typeexecuted)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_id", promotion.Id > 0 ? (object)promotion.Id : DBNull.Value),
                // CORREGIDO: Se usa la variable 'serviceType' que ya es un int
                new MySqlParameter("p_servicetype", MySqlDbType.Int32) { Value = serviceType },
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

        // NUEVO: Método para gestionar el ABM de PackagePromotion
        public async Task AbmPackagePromotionAsync(PackagePromotion promotion, string typeExecuted)
        {
            // Valida y convierte ServiceType a un valor entero
            if (!int.TryParse(promotion.ServiceType, out int serviceType))
            {
                throw new ArgumentException("El ServiceType del paquete debe ser un valor numérico.", nameof(promotion.ServiceType));
            }

            // Define la cadena SQL para llamar al Stored Procedure
            // He corregido la llamada al SP para usar solo los parámetros necesarios.
            string sql = "CALL SetActivePromotionPackages(@p_id, @p_servicetype, @p_packagetype, @p_description, " +
                "@p_companyname, @p_destinationname, @p_originname, @p_imageurl, @p_ishotweek, @p_originalprice, " +
                "@p_offerprice, @p_discountpercentage, @p_startdate, @p_enddate, @p_isactive, @p_hotelname, @p_typeexecuted)";

            var parameters = new MySqlParameter[]
            {
                // El orden de estos parámetros debe ser idéntico al de la cadena SQL y el Stored Procedure
                new MySqlParameter("p_id", promotion.Id > 0 ? (object)promotion.Id : DBNull.Value),
                new MySqlParameter("p_servicetype", MySqlDbType.Int32) { Value = serviceType },
                new MySqlParameter("p_packagetype", promotion.PackageType),
                new MySqlParameter("p_description", promotion.Description),
                new MySqlParameter("p.CompanyName", promotion.CompanyName ?? (object)DBNull.Value),
                new MySqlParameter("p_destinationname", promotion.DestinationName),
                new MySqlParameter("p_originname", promotion.OriginName),
                new MySqlParameter("p_imageurl", promotion.ImageUrl ?? (object)DBNull.Value),
                new MySqlParameter("p_ishotweek", promotion.IsHotWeek),
                new MySqlParameter("p_originalprice", promotion.OriginalPrice),
                new MySqlParameter("p_offerprice", promotion.OfferPrice),
                new MySqlParameter("p_discountpercentage", promotion.DiscountPercentage),
                new MySqlParameter("p_startdate", promotion.StartDate),
                new MySqlParameter("p_enddate", promotion.EndDate),
                new MySqlParameter("p_isactive", promotion.IsActive),
                new MySqlParameter("p_hotelname", promotion.HotelName ?? (object)DBNull.Value),
                new MySqlParameter("p_typeexecuted", typeExecuted)
            };

            await Database.ExecuteSqlRawAsync(sql, parameters);
        }


        public async Task AbmEncomiendaCompanyAsync(EncomiendaCompany company, string type)
        {
            await Database.ExecuteSqlRawAsync(
                "CALL SetEncomiendaCompany(@p_id, @p_companyname, @p_companyurl, @p_imageurl, @p_createdat, @p_typeexecuted)",
                new MySqlParameter("@p_id", company.Id),
                new MySqlParameter("@p_companyname", company.CompanyName),
                new MySqlParameter("@p_companyurl", company.CompanyUrl),
                new MySqlParameter("@p_imageurl", company.ImageUrl),
                //new MySqlParameter("@p_createdat", company.CreatedAt),
                new MySqlParameter("@p_typeexecuted", type)
            );
        }


        public async Task AbmActivityAsync(ActivitiesPromotion activity, string typeExecuted)
        {
            // Cambiamos el nombre del SP
            string sql = "CALL SetActivePromotionActivities(@p_id, @p_title, @p_description, @p_location, @p_imageurl, @p_typeexecuted)";

            var parameters = new MySqlParameter[]
            {
        new MySqlParameter("p_id", activity.Id > 0 ? (object)activity.Id : DBNull.Value),
        new MySqlParameter("p_title", activity.Title),
        new MySqlParameter("p_description", activity.Description),
        new MySqlParameter("p_location", activity.Location),
        new MySqlParameter("p_imageurl", activity.ImageUrl),
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

        // NUEVO: Método para obtener una promoción de paquete por ID
        public async Task<PackagePromotion> GetPackagePromotionByIdAsync(int id)
        {
            return await PackagePromotions.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreateWhatsappMessageAsync(string title, string messageTemplate, bool isActive)
        {
            await Database.ExecuteSqlInterpolatedAsync(
                $"CALL create_whatsapp_message({title}, {messageTemplate}, {isActive})"
            );
        }
        /// <summary>
        /// Obtiene todos los mensajes de WhatsApp activos e inactivos.
        /// </summary>
        public async Task<List<WhatsappMessage>> GetAllWhatsappMessagesAsync()
        {
            // Usa FromSqlRaw para ejecutar el SP y mapear el resultado a la entidad.
            return await WhatsappMessages
                .FromSqlRaw("CALL get_all_whatsapp_messages()")
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene los mensajes de WhatsApp que están marcados como activos.
        /// </summary>
        public async Task<List<WhatsappMessage>> GetActiveWhatsappMessagesAsync()
        {
            // Llama al SP que filtra por mensajes activos.
            return await WhatsappMessages
                .FromSqlRaw("CALL get_active_whatsapp_messages()")
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene un mensaje de WhatsApp específico por su ID.
        /// </summary>

        public async Task<WhatsappMessage> GetWhatsappMessageByIdAsync(int id)
        {
            // Usa FromSqlInterpolated para la llamada al SP
            // y SingleOrDefault() para obtener el primer resultado o null.
            // Esto evita que Entity Framework agregue LIMIT 1.
            return await Task.Run(() => WhatsappMessages
                .FromSqlInterpolated($"CALL get_whatsapp_message_by_id({id})")
                .AsEnumerable() // Convierte a IEnumerable para que la operación se realice en memoria
                .SingleOrDefault());
        }
        public async Task UpdateWhatsappMessageAsync(WhatsappMessage message)
        {
            // Usa ExecuteSqlInterpolatedAsync para el comando y pasa el modelo directamente
            await Database.ExecuteSqlInterpolatedAsync(
                $"CALL update_whatsapp_message({message.Id}, {message.Title}, {message.Message_Template}, {message.Is_Active})"
            );
        }

        public async Task<List<ActivitiesPromotion>> GetActiveActivitiesAsync()
        {
            // Usa FromSqlRaw para ejecutar el SP y mapear el resultado a la entidad.
            return await Activities
                .FromSqlRaw("CALL get_active_activities()")
                .ToListAsync();
        }


        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// </summary>
        public async Task CreateUserAsync(User user)
        {
            await Database.ExecuteSqlInterpolatedAsync($"CALL create_user({user.Email}, {user.Password}, {user.Name}, {user.Surname}, {user.Country}, {user.Rol})");
        }

        /// <summary>
        /// Intenta logear un usuario por email y contraseña.
        /// </summary>
        /// <returns>El objeto User si el login es exitoso, de lo contrario, null.</returns>
        public async Task<User> LoginUserAsync(string email, string password)
        {
            // Solución: Convierte el resultado a una lista y usa FirstOrDefault()
            return (await Users
                .FromSqlInterpolated($"CALL login_user({email}, {password})")
                .ToListAsync())
                .FirstOrDefault();
        }

        /// <summary>
        /// Lista todos los usuarios de la base de datos.
        /// </summary>
        /// <returns>Una lista de objetos User.</returns>
        public async Task<List<User>> ListUsersAsync()
        {
            return await Users
                .FromSqlRaw("CALL list_users()")
                .ToListAsync();
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            // Solución: Convierte el resultado a una lista y usa FirstOrDefault()
            return (await Users
                .FromSqlInterpolated($"CALL get_user_by_email({email})")
                .ToListAsync())
                .FirstOrDefault();
        }
        public async Task UpdateUserAsync(User user)
        {
            await Database.ExecuteSqlInterpolatedAsync($"CALL update_user({user.Id}, {user.Name}, {user.Surname}, {user.Country}, {user.Password})");
        }
    }
}
