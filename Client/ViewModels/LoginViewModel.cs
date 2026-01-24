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
    
    // Add this property
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [RelayCommand]
    private async Task LoginWithGoogleAsync()
    {
        IsLoggingIn = true;
        ErrorMessage = string.Empty; // Clear previous errors
        try
        {
            var success = await authService.LoginAsync();
            // No further action here; AuthenticationChanged event will drive navigation.
        }
        catch (Exception ex)
        {
            // If it's our custom Auth Exception, use the UserMessage
            if (ex is Client.Utils.Exceptions.ClientExceptionBase clientEx)
            {
                ErrorMessage = clientEx.UserMessage;
            }
            else
            {
                ErrorMessage = "An unexpected error occurred. Please try again.";
            }
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}