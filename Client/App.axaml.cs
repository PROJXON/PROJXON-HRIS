using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Exceptions.ApplicationState;
using Client.ViewModels;
using Client.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Client;

public partial class App : Application
{
    private static IServiceProvider? ServiceProvider { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DisableAvaloniaDataAnnotationValidation();
        ConfigureServices();

        var vm = ServiceProvider?.GetRequiredService<MainWindowViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins

            var mainWindow = new MainWindow
            {
                DataContext = vm
            };

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices()
    {
        var collection = new ServiceCollection();

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory);

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "Client.appsettings.json";
        
        var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            builder.AddJsonStream(stream);
        }

        builder.AddEnvironmentVariables();

        var configuration = builder.Build();

        var cloudSyncUrl = configuration["CloudSyncUrl"];
        var clientId = configuration["Auth:ClientId"];

        if (string.IsNullOrWhiteSpace(cloudSyncUrl) || string.IsNullOrWhiteSpace(clientId))
        {
            AppConfig.IsDemoMode = true;
            cloudSyncUrl = string.IsNullOrWhiteSpace(cloudSyncUrl) ? "http://localhost/" : cloudSyncUrl;
        }

        collection.AddCommonServices(configuration, cloudSyncUrl);
        
        RegisterSecureStorage(collection);

        ServiceProvider = collection.BuildServiceProvider();
    }

    private static void RegisterSecureStorage(IServiceCollection services)
    {
        const string applicationName = "Projxon HRIS";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            services.AddSingleton<ISecureTokenStorage>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<WindowsCredentialStorage>>();
                return new WindowsCredentialStorage(logger, applicationName);
            });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            services.AddSingleton<ISecureTokenStorage>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<LinuxSecretServiceStorage>>();
                return new LinuxSecretServiceStorage(logger, applicationName);
            });
        }
        else
        {
            services.AddSingleton<ISecureTokenStorage>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<SecureTokenStorage>>();
                return new SecureTokenStorage(logger);
            });
        }
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}