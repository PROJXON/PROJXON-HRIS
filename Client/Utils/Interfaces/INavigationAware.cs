using System.Threading.Tasks;

namespace Client.Utils.Interfaces;

public interface INavigationAware
{
    Task OnNavigatedTo(NavigationReason reason);
}

public enum NavigationReason
{
    FirstLoad,
    Reselect,
    Return
}