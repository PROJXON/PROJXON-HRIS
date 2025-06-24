using System;
using Client.Utils.Classes;
using Client.Utils.Enums;

namespace Client.Services;

public class NavigationService : INavigationService
{
    public event EventHandler<NavigationEventArgs>? NavigationRequested;

    public void NavigateTo(ViewModelType viewModelType)
    {
        NavigationRequested?.Invoke(this, new NavigationEventArgs(viewModelType));
    }
}