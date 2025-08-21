using System;
using System.Linq;
using System.Threading.Tasks;
using Client.Utils.Classes;
using Client.Utils.Enums;

namespace Client.Services;

public class NavigationService : INavigationService
{
    public event Func<object?, NavigationEventArgs, Task>? NavigationRequested;

    public async Task NavigateTo(ViewModelType viewModelType)
    {
        if (NavigationRequested is not null)
        {
            var handlers = NavigationRequested.GetInvocationList()
                .Cast<Func<object?, NavigationEventArgs, Task>>();

            foreach (var handler in handlers)
            {
                await handler(this, new NavigationEventArgs(viewModelType));
            }
        }
    }
    
    public async Task NavigateTo(ViewModelType viewModel, int id)
    {
        if (NavigationRequested is not null)
        {
            var handlers = NavigationRequested.GetInvocationList()
                .Cast<Func<object?, NavigationEventArgs, Task>>();

            foreach (var handler in handlers)
            {
                await handler(this, new NavigationEventArgs(viewModel, id));
            }
        }
    }
}