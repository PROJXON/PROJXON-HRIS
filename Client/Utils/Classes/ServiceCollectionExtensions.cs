using Client.Services;
using Client.ViewModels;
using Client.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Utils.Classes;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection, IConfigurationRoot configuration)
    {
        collection.AddSingleton<IConfiguration>(configuration);
        collection.AddSingleton<INavigationService, NavigationService>();
        collection.AddSingleton<IAuthenticationService, AuthenticationService>();

        collection.AddLogging();
        
        collection.AddHttpClient();

        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<LoginViewModel>();
        collection.AddTransient<DashboardViewModel>();
        collection.AddTransient<EmployeesListViewModel>();

        collection.AddTransient<MainWindow>();
        collection.AddTransient<LoginView>();
        collection.AddTransient<DashboardView>();
        collection.AddTransient<EmployeesListView>();
    }
}