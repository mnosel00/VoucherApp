using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using VoucherApp.Core.Interfaces;
using VoucherApp.Infrastructure.Data;
using VoucherApp.Infrastructure.Services;

namespace VoucherApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        public App()
        {
            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=.;Database=VoucherDb;Trusted_Connection=True;TrustServerCertificate=True;"));

            services.AddScoped<IVoucherService, VoucherService>();

            // Rejestrujemy ViewModel i MainWindow jako Transient.
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();

            Services = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Automatyczna migracja bazy przy starcie (dla wygody deweloperskiej)
            using (var scope = Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();

                var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
        }
    }

}
