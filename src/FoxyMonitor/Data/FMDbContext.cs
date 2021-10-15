using FoxyMonitor.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FoxyMonitor.Data
{
    public class FMDbContext : DbContext
    {
        internal DbSet<Account> Accounts { get; set; }
        internal DbSet<Alert> Alerts { get; set; }
        internal DbSet<PostAccountHistoricalDbItem> PostAccountHistoricalDbItems { get; set; }
        internal DbSet<PostPoolInfo> PostPools { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dataDirectory = Utils.IOUtils.GetDataDirectory();

            var sqlDatabaseFullPath = Path.Combine(dataDirectory, Constants.DbFileName); ;

            _ = optionsBuilder.UseSqlite($"Filename={sqlDatabaseFullPath}", options =>
              {
                  _ = options.CommandTimeout(15);
              });

            base.OnConfiguring(optionsBuilder);
        }

        public FMDbContext()
        {
            if (!MigrateDbAsync().Result) App.Current.Shutdown();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
        }

        private async Task<bool> MigrateDbAsync()
        {
            if (Database.GetPendingMigrations().Any())
            {
                var logger = App.Host_Builder.Services.GetService<ILogger<FMDbContext>>();

                try
                {
                    logger.LogInformation("Database has missing migrations or doesn't exist, applying migrations.");
                    await Database.MigrateAsync();
                    logger.LogInformation("Database migrations applied.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to apply migrations, db is incompatible.");

                    logger.LogWarning(ex, "Attempting to retrieve existing accounts before db deletion.");
                    Account[] accounts = null;
                    try
                    {
                        accounts = Accounts.ToArray();
                    }
                    catch (Exception exx)
                    {
                        logger.LogError(exx, "Failed to retrieve accounts.");
                    }

                    logger.LogWarning("Deleting database due to incompatability.");
                    var isDeleted = await Database.EnsureDeletedAsync();
                    if (isDeleted)
                    {
                        logger.LogWarning("Database delete success.");
                        logger.LogWarning("Attempting to create new db.");
                        try
                        {
                            await Database.MigrateAsync();
                        }
                        catch (Exception exx)
                        {
                            logger.LogError(exx, "Failed to create new db.");
                            return false;
                        }

                        logger.LogWarning("Attempting to migrate accounts to new db.");
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
                                        logger.LogError(exx, "Faild to migrate account {ID}: {DisplayName}", account.Id, account.DisplayName);
                                    }
                                }
                            }
                        }
                        catch (Exception exx)
                        {
                            logger.LogError(exx, "Failed to migrate accounts.");
                        }

                        return true;
                    }
                    else
                    {
                        logger.LogError("Database delete failed.");
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
