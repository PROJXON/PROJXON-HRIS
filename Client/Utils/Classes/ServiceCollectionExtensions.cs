using System;
using Client.Models.EmployeeManagement;
using Client.Services;
using Client.Utils.Interfaces;
using Client.ViewModels;
using Client.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Utils.Classes;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection, IConfigurationRoot configuration, string baseUrl)
    {
        collection.AddSingleton<IConfiguration>(configuration);
        collection.AddSingleton<INavigationService, NavigationService>();
        collection.AddSingleton<IAuthenticationService, AuthenticationService>();
        collection.AddSingleton<IUserPreferencesService, UserPreferencesService>();
        collection.AddLogging();
        
        collection.AddHttpClient("OAuth");
        collection.AddHttpClient<IApiClient, ApiClient>("Api", client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("User-Agent", "HRIS-App/1.0");
        });
        
        collection.AddScoped<IEmployeeRepository, EmployeeRepository>();
        
        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<LoginViewModel>();
        collection.AddTransient<DashboardViewModel>();
        collection.AddTransient<EmployeesListViewModel>();
        
        collection.AddTransient<PortalSelectionViewModel>();
        collection.AddTransient<HRDashboardViewModel>();
        collection.AddTransient<InternDashboardViewModel>();
        
        collection.AddTransient<MainWindow>();
        collection.AddTransient<LoginView>();
        collection.AddTransient<DashboardView>();
        collection.AddTransient<EmployeesListView>();
        
        collection.AddTransient<PortalSelectionView>();
        collection.AddTransient<HRDashboardView>();
    }
}