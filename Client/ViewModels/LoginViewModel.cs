using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Client.Services;
using Client.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Client.ViewModels;

public partial class LoginViewModel(IAuthenticationService authService)
    : ViewModelBase
{
    [ObservableProperty]
    private bool _isLoggingIn;

    [RelayCommand]
    private async Task LoginWithGoogleAsync()
    {
        IsLoggingIn = true;
        try
        {
            var success = await authService.LoginAsync();
            // No further action here; AuthenticationChanged event will drive navigation.
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}