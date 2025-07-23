using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Enums;
using Client.Utils.Exceptions.Auth;
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
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}