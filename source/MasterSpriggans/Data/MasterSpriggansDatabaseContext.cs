using System;
using System.Threading.Tasks;
using MasterSpriggans.Data.Models;
using MasterSpriggans.Utils;
using Microsoft.EntityFrameworkCore;
using XIVApiLib;
using XIVApiLib.Models.Responses;

namespace MasterSpriggans.Data
{
    public class MasterSpriggansDatabaseContext : DbContext
    {
        public DbSet<XIVTitleModel> XIVTitles { get; set; }

        public MasterSpriggansDatabaseContext(DbContextOptions<MasterSpriggansDatabaseContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<XIVTitleModel>()
                        .HasKey(c => c.ID);
        }

        public static async Task SeedDatabase(MasterSpriggansDatabaseContext context, XIVApi xivApi)
        {
            //  Get the list of titles from the XIVApi
            Logger.Message("Downloading title list, this may take a moment...");
            TitleListResponseModel titleListResponse = await xivApi.GetTitlesList();

            if(titleListResponse.Error.HasValue)
            {
                throw new Exception(titleListResponse.Message);
            }
            else
            {
                Logger.Message("Inserting titles into database...");
                foreach(var title in titleListResponse.Results)
                {
                    await context.AddAsync(new XIVTitleModel() { ID = title.ID, Name = title.Name });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}