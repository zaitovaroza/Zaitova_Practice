using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using Zaitova.ViewModels;
using ZaitovaLibrary.Data;
using ZaitovaLibrary.Services;

namespace Zaitova
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
            }

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IPartnerRepository, PartnerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPartnerTypeRepository, PartnerTypeRepository>();

            services.AddScoped<IDiscountCalculator, DiscountCalculator>();
            services.AddScoped<IPartnerService, PartnerService>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<PartnerEditViewModel>();
        }
    }
}