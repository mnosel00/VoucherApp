using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using VoucherApp.Core.Interfaces;
using VoucherApp.Infrastructure.Data;
using VoucherApp.Infrastructure.Services;

namespace VoucherApp
{
    public partial class App : Application
    {
        private readonly IHost _host;
        private IServiceScope? _scope; // Pole do przechowywania zasięgu

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                .Build();
        }

        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IVoucherService, VoucherService>();
            // Przywracamy Transient, ponieważ cyklem życia zarządza teraz _scope
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await _host.StartAsync();

                // Utwórz i przypisz zasięg do pola klasy, aby nie został zniszczony
                _scope = _host.Services.CreateScope();
            
                var context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.Migrate();

                var mainWindow = _scope.ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił krytyczny błąd podczas uruchamiania aplikacji:\n\n{ex.ToString()}", 
                                "Błąd Aplikacji", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            // Prawidłowe zwolnienie zasobów
            _scope?.Dispose(); 

            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }

            base.OnExit(e);
        }
    }
}
