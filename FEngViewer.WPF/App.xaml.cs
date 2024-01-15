using System.Configuration;
using System.Data;
using System.Windows;
using FEngViewer.WPF.Services;
using FEngViewer.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FEngViewer.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<ITestService, TestService>();

            // View models
            services.AddTransient<MainViewModel>();
            services.AddTransient<PackageViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
