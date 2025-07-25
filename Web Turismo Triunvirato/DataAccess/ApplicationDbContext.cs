// Data/ApplicationDbContext.cs
using Web_Turismo_Triunvirato.Models;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;



namespace Web_Turismo_Triunvirato.Data // Ajusta el namespace a tu proyecto
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para cada una de tus tablas
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Activities> Activities { get; set; }
        public DbSet<View_Index_Destination> Destinations { get; set; }

        public DbSet<View_Index_DestinationCarouselItem> View_DestinationCarouselItems { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
       
        // public DbSet<Trip> Trips { get; set; }
        public DbSet<User> Users { get; set; }

        // Si tienes tablas de unión explícitas, puedes agregarlas aquí
        // public DbSet<TripPassengers> TripPassengers { get; set; }
        // public DbSet<TripDestinations> TripDestinations { get; set; }
        // public DbSet<TripAccommodations> TripAccommodations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        //public class TripPassengers
        //{
        //    public int TripId { get; set; }
        //    public Trip Trip { get; set; }
        //    public int PassengerId { get; set; }
        //    public Passenger Passenger { get; set; }
        //}

        //public class TripDestinations
        //{
        //    public int TripId { get; set; }
        //    public Trip Trip { get; set; }
        //    public int DestinationId { get; set; }
        //    public View_Destination Destination { get; set; }
        //}

        //public class TripAccommodations
        //{
        //    public int TripId { get; set; }
        //    public Trip Trip { get; set; }
        //    public int AccommodationId { get; set; }
        //    public Accommodation Accommodation { get; set; }
        //}


        // --- El método modificado para llamar al Stored Procedure ---
        public async Task<List<View_Index_DestinationCarouselItem>> GetCarouselItemsAsync()
        {
            // ¡¡¡USAR EL NUEVO DBSET AQUÍ!!!
            return await View_DestinationCarouselItems
                             .FromSqlRaw("CALL GetActiveCarouselItems()")
                             .ToListAsync();
        }

        public async Task<List<View_Index_Destination>> GetHotDestinyItemsAsync()
        {
            // ¡¡¡USAR EL NUEVO DBSET AQUÍ!!!
            return await Destinations
                             .FromSqlRaw("CALL GetActiveHotDestiny()")
                             .ToListAsync();
        }

    }


}