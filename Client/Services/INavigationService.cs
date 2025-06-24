using Client.Utils.Enums;

using System;
using Client.Utils.Classes;

namespace Client.Services;

public interface INavigationService
{
    event EventHandler<NavigationEventArgs>? NavigationRequested;
    void NavigateTo(ViewModelType viewModelType);
}