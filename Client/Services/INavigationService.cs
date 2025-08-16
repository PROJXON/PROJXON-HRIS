using Client.Utils.Enums;

using System;
using System.Threading.Tasks;
using Client.Utils.Classes;

namespace Client.Services;

public interface INavigationService
{
    event Func<object?, NavigationEventArgs, Task>? NavigationRequested;
    Task NavigateTo(ViewModelType viewModelType);
}