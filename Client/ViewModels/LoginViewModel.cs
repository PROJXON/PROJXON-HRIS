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
        
        var dialog = new NativeWebDialog
        {
            Title = "Google OAuth Login",
            CanUserResize = true,
            Source = new Uri("https://docs.avaloniaui.net/")
        };

        // Create TaskCompletionSource so we can wait until window is closed.
        var tcs = new TaskCompletionSource();
        dialog.Closing += (s, e) => tcs.SetResult();
        
        dialog.Resize(500, 500);
        
        dialog.Show();

        await tcs.Task;

        try
        {
            var success = await authService.LoginAsync();
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}