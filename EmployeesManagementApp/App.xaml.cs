using EmployeesManagement.Common;
using EmployeesManagement.Service;
using EmployeesManagementApp.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace EmployeesManagementApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => { 
                    services.AddSingleton<IEmployeeService, EmployeeService>();
                    services.AddSingleton<IDataRepository>(o => {
                        return new ApiDataRepository(
                            System.Configuration.ConfigurationManager.AppSettings["apiAddress"],
                            System.Configuration.ConfigurationManager.AppSettings["apiKey"]);
                    });
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<ReportService>();
                })
                .Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            AppHost!.Services.GetRequiredService<MainWindow>().Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();

            base.OnExit(e);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
