using System.Threading.Tasks;

namespace Client.Utils.Interfaces;

public interface INavigationAware
{
    Task OnNavigatedToAsync() => Task.CompletedTask;
    Task OnNavigatedFromAsync() => Task.CompletedTask;
}

public enum NavigationReason
{
    FirstLoad,
    Reselect,
    Return
}