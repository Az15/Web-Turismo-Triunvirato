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
        public DbSet<Entidad> Entidades { get; set; }

         public DbSet<Imagen> Imagen { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeo de entidades a tablas/vistas
            modelBuilder.Entity<Promotion>().ToTable("view_promotionHoteles");
            modelBuilder.Entity<BusPromotion>().ToTable("BusPromotions");
            modelBuilder.Entity<HotelPromotion>().ToTable("HotelPromotions");
            modelBuilder.Entity<FlightPromotion>().ToTable("FlightPromotions");
            modelBuilder.Entity<PackagePromotion>().ToTable("PackagePromotions");
            modelBuilder.Entity<Imagen>().ToTable("imagenes");


            base.OnModelCreating(modelBuilder);
        }

        public async Task EliminarImagenAsync(int idImagen)
        {
            var imagen = await Imagen.FindAsync(idImagen);
            if (imagen != null)
            {
                Imagen.Remove(imagen);
                await SaveChangesAsync();
            }
        }

        public async Task<List<Imagen>> GetImagenesByEntidadAsync(int idEntidad, int idObjeto)
        {
            // Usando EF Core nativo (asegúrate de que el SP devuelva todas las columnas de la tabla 'imagenes')
            return await this.Imagen
                .FromSqlRaw("CALL sp_imagenes_get({0}, {1})", idEntidad, idObjeto)
                .ToListAsync();
        }                  
        // --- Métodos para obtener datos (Stored Procedures) ---

        public async Task<List<FlightPromotion>> GetImageAllItemsAsync()
        {
            return await FlightPromotions
                                 .FromSqlRaw("CALL GetImageAll()")
                                 .ToListAsync();
        }

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
        public async Task<int> AbmFlightPromotionAsync(FlightPromotion promotionFlight, string typeExecuted)
        {
            int serviceType = 3; // Forzado para Vuelos
            int returnedId = promotionFlight.Id;

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SetActivePromotionFlights";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Agregar parámetros
                command.Parameters.Add(new MySqlParameter("p_id", promotionFlight.Id > 0 ? (object)promotionFlight.Id : DBNull.Value));
                command.Parameters.Add(new MySqlParameter("p_servicetype", serviceType));
                command.Parameters.Add(new MySqlParameter("p_description", (object)promotionFlight.Description ?? DBNull.Value));
                command.Parameters.Add(new MySqlParameter("p_whatsapp_id", promotionFlight.Whatsapp_Id));
                command.Parameters.Add(new MySqlParameter("p_destinationname", (object)promotionFlight.DestinationName ?? DBNull.Value));
                command.Parameters.Add(new MySqlParameter("p_originname", (object)promotionFlight.OriginName ?? DBNull.Value));
                command.Parameters.Add(new MySqlParameter("p_imageurl", (object)promotionFlight.ImageUrl ?? DBNull.Value));
                command.Parameters.Add(new MySqlParameter("p_ishotweek", promotionFlight.IsHotWeek));
                command.Parameters.Add(new MySqlParameter("p_originalprice", promotionFlight.OriginalPrice));
                command.Parameters.Add(new MySqlParameter("p_offerprice", promotionFlight.OfferPrice));
                command.Parameters.Add(new MySqlParameter("p_discountpercentage", promotionFlight.DiscountPercentage));
                command.Parameters.Add(new MySqlParameter("p_startdate", promotionFlight.StartDate));
                command.Parameters.Add(new MySqlParameter("p_enddate", promotionFlight.EndDate));
                command.Parameters.Add(new MySqlParameter("p_isactive", promotionFlight.IsActive));
                command.Parameters.Add(new MySqlParameter("p_stars", (object)promotionFlight.Stars ?? 0));
                command.Parameters.Add(new MySqlParameter("p_typeexecuted", typeExecuted));

                if (this.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                    await this.Database.GetDbConnection().OpenAsync();

                // Ejecutamos y leemos el SELECT que arroja el SP
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        returnedId = reader.GetInt32(0); // Captura el "SELECT id" del SP
                    }
                }
            }
            return returnedId;
        }

        public async Task AbmFlightPromotionAsync(int id, string typeExecuted)
        {
           string sql = "CALL DeleteActivePromotionFlights(@p_id)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_id", id),
                //new MySqlParameter("p_typeexecuted", typeExecuted)
            };
            await Database.ExecuteSqlRawAsync(sql, parameters);
        }

        // Método para gestionar el alta, la baja y la modificación de promociones de hoteles
        public async Task<int> AbmHotelPromotionAsync(HotelPromotion promotion, string typeExecuted)
        {
            int idGenerado = 0;
            if (!int.TryParse(promotion.ServiceType, out int serviceType)) serviceType = 4; // Asumiendo 4 para Hoteles

            var connection = Database.GetDbConnection();
            bool wasClosed = connection.State == System.Data.ConnectionState.Closed;

            if (wasClosed) await connection.OpenAsync();

            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CALL SetActivePromotionHotels(@p_id, @p_servicetype, @p_description, @p_whatsapp_Id, @p_destinationname, @p_hotelname, @p_imageurl, @p_ishotweek, @p_originalprice, @p_offerprice, @p_discountpercentage, @p_startdate, @p_enddate, @p_stars , @p_isactive, @p_typeexecuted)";

                    command.Parameters.Add(new MySqlParameter("p_id", promotion.Id > 0 ? (object)promotion.Id : DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_servicetype", serviceType));
                    command.Parameters.Add(new MySqlParameter("p_description", promotion.Description));
                    command.Parameters.Add(new MySqlParameter("p_whatsapp_Id", promotion.Whatsapp_Id));
                    command.Parameters.Add(new MySqlParameter("p_destinationname", promotion.DestinationName));
                    command.Parameters.Add(new MySqlParameter("p_hotelname", promotion.HotelName));
                    command.Parameters.Add(new MySqlParameter("p_imageurl", promotion.ImageUrl ?? (object)DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_ishotweek", promotion.IsHotWeek));
                    command.Parameters.Add(new MySqlParameter("p_originalprice", promotion.OriginalPrice));
                    command.Parameters.Add(new MySqlParameter("p_offerprice", promotion.OfferPrice));
                    command.Parameters.Add(new MySqlParameter("p_discountpercentage", promotion.DiscountPercentage));
                    command.Parameters.Add(new MySqlParameter("p_startdate", promotion.StartDate));
                    command.Parameters.Add(new MySqlParameter("p_enddate", promotion.EndDate));
                    command.Parameters.Add(new MySqlParameter("p_stars", promotion.Stars));
                    command.Parameters.Add(new MySqlParameter("p_isactive", promotion.IsActive));
                    command.Parameters.Add(new MySqlParameter("p_typeexecuted", typeExecuted));

                    if (typeExecuted == "INSERT")
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                idGenerado = reader.GetInt32(0);
                                promotion.Id = idGenerado;
                            }
                        }
                    }
                    else
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            finally
            {
                if (wasClosed) await connection.CloseAsync();
            }
            return idGenerado;
        }


        public async Task AbmHotelPromotionAsync(int id, string typeExecuted)
        {
            string sql = "CALL DeleteActiveHotelPromotions(@p_id)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_id", id),
                //new MySqlParameter("p_typeexecuted", typeExecuted)
            };
            await Database.ExecuteSqlRawAsync(sql, parameters);
        }

        // Método para gestionar el ABM de BusPromotion
        // Método para gestionar el ABM de BusPromotion
        public async Task<int> AbmBusPromotionAsync(BusPromotion promotion, string typeExecuted)
        {
            int idGenerado = 0;

            // 1. Validar ServiceType (Buses suele ser 2)
            if (!int.TryParse(promotion.ServiceType, out int serviceType))
            {
                serviceType = 2;
            }

            // 2. Ejecutar el procedimiento usando Command para recuperar el ID
            var connection = Database.GetDbConnection();
            bool wasClosed = connection.State == System.Data.ConnectionState.Closed;

            if (wasClosed) await connection.OpenAsync();

            try
            {
                using (var command = connection.CreateCommand())
                {
                    // Definimos el llamado al SP de Buses
                    command.CommandText = "CALL SetActivePromotionBuses(@p_id, @p_servicetype, @p_description, @p_whatsapp_id, @p_destinationname, @p_originname, @p_buscompanyname, @p_category, @p_imageurl, @p_ishotweek, @p_originalprice, @p_offerprice, @p_discountpercentage, @p_startdate, @p_enddate, @p_isactive, @p_typeexecuted)";

                    // Agregamos los parámetros uno por uno (necesario para este enfoque)
                    command.Parameters.Add(new MySqlParameter("p_id", promotion.Id > 0 ? (object)promotion.Id : DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_servicetype", serviceType));
                    command.Parameters.Add(new MySqlParameter("p_description", promotion.Description ?? (object)DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_whatsapp_id", promotion.Whatsapp_Id > 0 ? (object)promotion.Whatsapp_Id : DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_destinationname", promotion.DestinationName ?? (object)DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_originname", promotion.OriginName ?? (object)DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_buscompanyname", promotion.BusCompanyName ?? (object)DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_category", promotion.Category ?? (object)DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_imageurl", promotion.ImageUrl ?? (object)DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_ishotweek", promotion.IsHotWeek));
                    command.Parameters.Add(new MySqlParameter("p_originalprice", promotion.OriginalPrice));
                    command.Parameters.Add(new MySqlParameter("p_offerprice", promotion.OfferPrice));
                    command.Parameters.Add(new MySqlParameter("p_discountpercentage", promotion.DiscountPercentage));
                    command.Parameters.Add(new MySqlParameter("p_startdate", promotion.StartDate));
                    command.Parameters.Add(new MySqlParameter("p_enddate", promotion.EndDate));
                    command.Parameters.Add(new MySqlParameter("p_isactive", promotion.IsActive));
                    command.Parameters.Add(new MySqlParameter("p_typeexecuted", typeExecuted));

                    if (typeExecuted == "INSERT")
                    {
                        // Ejecutamos el Reader para capturar el SELECT LAST_INSERT_ID() que hace tu SP
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                idGenerado = reader.GetInt32(0);
                                promotion.Id = idGenerado;
                            }
                        }
                    }
                    else
                    {
                        // Si es UPDATE o DELETE, no esperamos ID de vuelta
                        await command.ExecuteNonQueryAsync();
                        idGenerado = promotion.Id;
                    }
                }
            }
            finally
            {
                // Cerramos la conexión si nosotros la abrimos
                if (wasClosed) await connection.CloseAsync();
            }

            return idGenerado;
        }

        public async Task AbmBusPromotionAsync(int id, string typeExecuted)
        {
            string sql = "CALL DeleteActiveBusPromotions(@p_id)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_id", id),
                //new MySqlParameter("p_typeexecuted", typeExecuted)
            };
            await Database.ExecuteSqlRawAsync(sql, parameters);
        }

        
        public async Task<int> AbmPackagePromotionAsync(PackagePromotion promotion, string typeExecuted)
        {
            int idGenerado = 0;

            // 1. Validar ServiceType
            if (!int.TryParse(promotion.ServiceType, out int serviceType))
            {
                serviceType = 3;
            }



            // 2. Ejecutar el procedimiento
            var connection = Database.GetDbConnection();
            bool wasClosed = connection.State == System.Data.ConnectionState.Closed;

            if (wasClosed) await connection.OpenAsync();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            try
            {
                using (var command = connection.CreateCommand())
            {
                command.CommandText = "CALL SetActivePromotionPackages(@p_id, @p_servicetype, @p_packagetype, @p_description, @p_whatsapp_id, @p_companyname, @p_destinationname, @p_originname, @p_imageurl, @p_ishotweek, @p_originalprice, @p_offerprice, @p_discountpercentage, @p_startdate, @p_enddate, @p_hotelname, @p_isactive, @p_typeexecuted)";

                command.Parameters.Add(new MySqlParameter("p_id", promotion.Id > 0 ? (object)promotion.Id : DBNull.Value));
                command.Parameters.Add(new MySqlParameter("p_servicetype", serviceType));
                command.Parameters.Add(new MySqlParameter("p_packagetype", promotion.PackageType));
                command.Parameters.Add(new MySqlParameter("p_description", promotion.Description));
                command.Parameters.Add(new MySqlParameter("p_whatsapp_id", promotion.Whatsapp_Id));
                command.Parameters.Add(new MySqlParameter("p_companyname", promotion.CompanyName ?? (object)DBNull.Value));
                command.Parameters.Add(new MySqlParameter("p_destinationname", promotion.DestinationName));
                command.Parameters.Add(new MySqlParameter("p_originname", promotion.OriginName));
                command.Parameters.Add(new MySqlParameter("p_imageurl", promotion.ImageUrl ?? (object)DBNull.Value));
                command.Parameters.Add(new MySqlParameter("p_ishotweek", promotion.IsHotWeek));
                command.Parameters.Add(new MySqlParameter("p_originalprice", promotion.OriginalPrice));
                command.Parameters.Add(new MySqlParameter("p_offerprice", promotion.OfferPrice));
                command.Parameters.Add(new MySqlParameter("p_discountpercentage", promotion.DiscountPercentage));
                command.Parameters.Add(new MySqlParameter("p_startdate", promotion.StartDate));
                command.Parameters.Add(new MySqlParameter("p_enddate", promotion.EndDate));
                command.Parameters.Add(new MySqlParameter("p_hotelname", promotion.HotelName ?? (object)DBNull.Value));
                command.Parameters.Add(new MySqlParameter("p_isactive", promotion.IsActive));
                command.Parameters.Add(new MySqlParameter("p_typeexecuted", typeExecuted));

                if (typeExecuted == "INSERT")
                {
                    // Usamos un bloque USING para que el Reader se cierre INMEDIATAMENTE
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            idGenerado = reader.GetInt32(0);
                            promotion.Id = idGenerado;
                        }
                    }
                    // Aquí el Reader ya se cerró, la conexión queda libre.
                }
                else
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
           }
            finally
            {
                // IMPORTANTE: Si tú la abriste, tú la cierras para que EF la tome limpia
                if (wasClosed) await connection.CloseAsync();
            }
            return idGenerado;
        }


        public async Task InsertarImagenGenericaAsync(string url, int entidadId, int referenciaId)
        {
            // 1. Definimos la llamada al nuevo SP genérico
            string sql = "CALL sp_InsertarImagenGenerica(@p_url, @p_entidad_id, @p_objeto_id)";

            // 2. Creamos los parámetros
            var parameters = new MySqlParameter[]
            {
        new MySqlParameter("p_url", url ?? (object)DBNull.Value),
        new MySqlParameter("p_entidad_id", entidadId),
        new MySqlParameter("p_objeto_id", referenciaId)
            };

            // 3. Ejecutamos el procedimiento
            // Usamos ExecuteSqlRawAsync ya que es una operación de inserción
            await Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task AbmPackagePromotionAsync(int id, string typeExecuted)
        {
            string sql = "CALL DeleteActivePackagePromotions(@p_id)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_id", id),
                //new MySqlParameter("p_typeexecuted", typeExecuted)
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

        public async Task AbmEncomiendaCompanyAsync(int id, string typeExecuted)
        {
            string sql = "CALL DeleteActiveEncomiendaCompanies(@p_id)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_id", id),
                //new MySqlParameter("p_typeexecuted", typeExecuted)
            };
            await Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task<int> AbmActivityAsync(ActivitiesPromotion activity, string typeExecuted)
        {
            int idGenerado = activity.Id;

            var connection = Database.GetDbConnection();
            bool wasClosed = connection.State == System.Data.ConnectionState.Closed;
            if (wasClosed) await connection.OpenAsync();

            try
            {
                using (var command = connection.CreateCommand())
                {
                    // Nota: Agregué @p_isactive al final por si tu SP lo requiere
                    command.CommandText = "CALL SetActivePromotionActivities(@p_id, @p_title, @p_description, @p_whatsapp_id, @p_location, @p_imageurl, @p_typeexecuted, @p_isactive)";

                    command.Parameters.Add(new MySqlParameter("p_id", activity.Id > 0 ? (object)activity.Id : DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_title", activity.Title ?? (object)DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_description", activity.Description ?? (object)DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_whatsapp_id", activity.Whatsapp_Id));
                    command.Parameters.Add(new MySqlParameter("p_location", activity.Location ?? (object)DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_imageurl", activity.ImageUrl ?? (object)DBNull.Value));
                    command.Parameters.Add(new MySqlParameter("p_typeexecuted", typeExecuted));
                    command.Parameters.Add(new MySqlParameter("p_isactive", activity.IsActive));

                    if (typeExecuted == "INSERT")
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                idGenerado = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    else
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            finally
            {
                if (wasClosed) await connection.CloseAsync();
            }
            return idGenerado;
        }

        // Método específico para el borrado (como en Packages)
        public async Task DeleteActivityAsync(int id, string typeExecuted)
        {
            string sql = "CALL DeleteActiveActivitiesPromotion(@p_id)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_id", id),
        
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
 
        public async Task<List<WhatsappMessage>> GetAllWhatsappMessagesAsync()
        {
            // Usa FromSqlRaw para ejecutar el SP y mapear el resultado a la entidad.
            return await WhatsappMessages
                .FromSqlRaw("CALL get_all_whatsapp_messages()")
                .ToListAsync();
        }

        public async Task<List<WhatsappMessage>> GetActiveWhatsappMessagesAsync()
        {
            // Llama al SP que filtra por mensajes activos.
            return await WhatsappMessages
                .FromSqlRaw("CALL get_active_whatsapp_messages()")
                .ToListAsync();
        }

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

        public async Task CreateUserAsync(User user)
        {
            await Database.ExecuteSqlInterpolatedAsync($"CALL create_user({user.Email}, {user.Password}, {user.Name}, {user.Surname}, {user.Country}, {user.Rol})");
        }

      
        public async Task<User> LoginUserAsync(string email, string password)
        {
            // Solución: Convierte el resultado a una lista y usa FirstOrDefault()
            return (await Users
                .FromSqlInterpolated($"CALL login_user({email}, {password})")
                .ToListAsync())
                .FirstOrDefault();
        }

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
