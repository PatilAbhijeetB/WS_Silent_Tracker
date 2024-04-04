using Emageia.Workshiftly.Entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.IO;

namespace Emageia.Workshiftly.Domain.Concrete
{
    /// <summary>
    /// The databse context
    /// </summary>
    public class ClientDataStoreDbContext : DbContext
    {
        #region DbSets

        public DbSet<LoginCredentialsDataModel> LoginCredentials { get; set; }
        public DbSet<LogAppData> LogAppDatas { get; set; }
        public DbSet<Screenshot> Screenshots { get; set; }
        public DbSet<ActivietyWindows> ActivietyWindows { get; set; }
        public DbSet<ScreenshotMeta> ScreenshotMetas { get; set; }
        public DbSet<WorkStatusLog> WorkStatusLogs { get; set; }
        public DbSet<DeviceConfiguration> DeviceSettings { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>

        public string DbPath;
        //public ClientDataStoreDbContext(DbContextOptions<ClientDataStoreDbContext> options) : base(options)
        //{
        //    var folder = Environment.SpecialFolder.LocalApplicationData;
        //    var path = Environment.GetFolderPath(folder);
        //    DbPath = System.IO.Path.Join(path, "blogging.db");
        //}

        public ClientDataStoreDbContext()
        {
            

            var paths = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var subFolderPath = System.IO.Path.Combine(paths, "Workshiftly Client");
            if (!Directory.Exists(subFolderPath))
            {
                Directory.CreateDirectory(subFolderPath);
            }

            DbPath = System.IO.Path.Combine(subFolderPath, "source.db");
          


           Database.EnsureCreated();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //@"Data Source=e:\Sample.db"

            string dbPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //    SqliteConnection connection = InitializeSQLiteConnection(dbPath);


            if (!optionsBuilder.IsConfigured)
            {
                 optionsBuilder.UseSqlite($@"Data Source={DbPath}");
          //      optionsBuilder.UseSqlite("Data Source = \"C:\\Users\\Niroshan\\Documents\\Workshiftly Client\\bloggingsss.db\"; Password = Test123");
                //    optionsBuilder.UseSqlite(connection);

                Batteries.Init();
               
            }
            base.OnConfiguring(optionsBuilder);
        }
        private SqliteConnection InitializeSQLiteConnection(string dbFilePath)
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = dbFilePath,
                Password = "Test123"// PRAGMA key is being sent from EF Core directly after opening the connection
            };
            var noie = connectionString.ToString();
            //var oji =new SqliteConnection()
            return new SqliteConnection(connectionString.ToString());

        }
        #endregion


        #region Model createe


        /// <summary>
        /// Confger DB structer and relationship
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // LogingCredentials
            // Set Id as primary key
            modelBuilder.Entity<LoginCredentialsDataModel>().HasKey(a => a.Id);
            modelBuilder.Entity<LogAppData>().HasKey(a => a.Id);
            modelBuilder.Entity<ActivietyWindows>().HasKey(a => a.rowId);
            modelBuilder.Entity<ScreenshotMeta>().HasKey(a => a.id);
            modelBuilder.Entity<WorkStatusLog>().HasKey(a => a.rowId);
            modelBuilder.Entity<Screenshot>().HasKey(a => a.rowId);
            modelBuilder.Entity<DeviceConfiguration>().HasKey(a => a.id);
            //TODO: Set up limits  WorkStatusLog
            //modeBuilder.Entitiy<LoginCredentialsDataModel>().Property(a => a.FirstName).HasMaxLength(50);
        }
        #endregion
    }
}
