//using esyasoft.mobility.CHRGUP.service.core.Models;
//using Microsoft.EntityFrameworkCore;


//namespace esyasoft.mobility.CHRGUP.service.persistence.Data
//{
//    public class AppDbContext : DbContext
//    {
//        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//        public DbSet<Admin> admins { get; set; }
//        public DbSet<Charger> chargers { get; set; }
//        public DbSet<ChargingSession> chargingSessions { get; set; }
//        public DbSet<Driver> drivers { get; set; }
//        public DbSet<Fault> faults { get; set; }
//        public DbSet<Location> locations { get; set; }
//        public DbSet<Log> logs { get; set; }
//        public DbSet<Manager> managers { get; set; }
//        public DbSet<Supervisor> supervisors { get; set; }
//        public DbSet<Vehicle> vehicles { get; set; }
//        public DbSet<ChargerConfig> chargerConfigs { get; set; }
//        public DbSet<Reservation> reservations { get; set; }
//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Vehicle>(entity =>
//            {
//                entity.HasIndex(v => v.RegistrationNumber)
//                      .IsUnique();

//                entity.HasIndex(v => v.VIN)
//                      .IsUnique();
//            });
//        }

//        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
//        {
//            builder.Properties<DateTime>()
//                .HaveColumnType("timestamp without time zone");

//            builder.Properties<DateTime?>()
//                .HaveColumnType("timestamp without time zone");

//            builder.Properties<Enum>()
//                .HaveConversion<string>();
//        }
//        public override int SaveChanges()
//        {
//            ApplyUtcDateTimes();
//            return base.SaveChanges();
//        }

//        public override async Task<int> SaveChangesAsync(
//            CancellationToken cancellationToken = default)
//        {
//            ApplyUtcDateTimes();
//            return await base.SaveChangesAsync(cancellationToken);
//        }
//        private void ApplyUtcDateTimes()
//        {
//            var entries = ChangeTracker.Entries()
//                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

//            foreach (var entry in entries)
//            {
//                foreach (var property in entry.Properties)
//                {
//                    if (property.Metadata.ClrType == typeof(DateTime) ||
//                        property.Metadata.ClrType == typeof(DateTime?))
//                    {
//                        if (property.CurrentValue is DateTime dt)
//                        {
                            
//                            property.CurrentValue =
//                                DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
//                        }
//                    }
//                }
//            }
//        }

//    }
//}
