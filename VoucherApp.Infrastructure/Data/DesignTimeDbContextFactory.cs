using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace VoucherApp.Infrastructure.Data
{
    /// <summary>
    /// Ta klasa jest u¿ywana przez narzêdzia Entity Framework Core (np. do tworzenia migracji)
    /// w czasie projektowania. Pozwala na utworzenie instancji AppDbContext
    /// z pominiêciem logiki startowej aplikacji WPF.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Budujemy œcie¿kê do pliku appsettings.json w projekcie startowym (VoucherApp)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../VoucherApp"))
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}