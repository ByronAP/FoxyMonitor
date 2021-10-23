using FoxyMonitor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FoxyMonitor.DbContexts
{
    public class AppDbContext : DbContext
    {
        internal DbSet<Account> Accounts { get; set; }
        internal DbSet<Alert> Alerts { get; set; }
        internal DbSet<PostAccountHistoricalDbItem> PostAccountHistoricalDbItems { get; set; }
        internal DbSet<PostPoolInfo> PostPools { get; set; }
        internal DbSet<PostPoolHistoricalDbItem> PostPoolHistoricalDbItems { get; set; }

        private readonly string _dataDirectory;
        private readonly string _dbFileName;
        private readonly string _localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private readonly ILogger<AppDbContext> _logger;

#if DEBUG
        public AppDbContext()
        {
            _dataDirectory = "FoxyMonitor\\Data";
            _dbFileName = "fmdata.dat";
        }
#endif
        public AppDbContext(ILogger<AppDbContext> logger, IOptions<AppConfig> appConfig)
        {
            _logger = logger;
            _dataDirectory = appConfig.Value.DataFolder;
            _dbFileName = appConfig.Value.DataFileName;

            if (!MigrateDbAsync().Result) App.Current.Shutdown();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var sqlDatabaseFullPath = Path.Combine(_localAppData, _dataDirectory, _dbFileName); ;

            _ = optionsBuilder.UseSqlite($"Filename={sqlDatabaseFullPath}", options =>
            {
                _ = options.CommandTimeout(15);
            });

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
        }

        private async Task<bool> MigrateDbAsync()
        {
            if (Database.GetPendingMigrations().Any())
            {
                try
                {
                    _logger?.LogInformation("Database has missing migrations or doesn't exist, applying migrations.");
                    await Database.MigrateAsync();
                    _logger?.LogInformation("Database migrations applied.");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to apply migrations, db is incompatible.");

                    _logger?.LogWarning(ex, "Attempting to retrieve existing accounts before db deletion.");
                    Account[] accounts = null;
                    try
                    {
                        accounts = Accounts.ToArray();
                    }
                    catch (Exception exx)
                    {
                        _logger?.LogError(exx, "Failed to retrieve accounts.");
                    }

                    _logger?.LogWarning("Deleting database due to incompatability.");
                    var isDeleted = await Database.EnsureDeletedAsync();
                    if (isDeleted)
                    {
                        _logger?.LogWarning("Database delete success.");
                        _logger?.LogWarning("Attempting to create new db.");
                        try
                        {
                            await Database.MigrateAsync();
                        }
                        catch (Exception exx)
                        {
                            _logger?.LogError(exx, "Failed to create new db.");
                            return false;
                        }

                        _logger?.LogWarning("Attempting to migrate accounts to new db.");
                        try
                        {
                            if (accounts != null)
                            {
                                foreach (var account in accounts)
                                {
                                    try
                                    {
                                        _ = Accounts.Add(account);
                                        await SaveChangesAsync();
                                    }
                                    catch (Exception exx)
                                    {
                                        _logger?.LogError(exx, "Faild to migrate account {ID}: {DisplayName}", account.Id, account.DisplayName);
                                    }
                                }
                            }
                        }
                        catch (Exception exx)
                        {
                            _logger?.LogError(exx, "Failed to migrate accounts.");
                        }

                        return true;
                    }
                    else
                    {
                        _logger?.LogError("Database delete failed.");
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
