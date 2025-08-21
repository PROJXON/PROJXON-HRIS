using System.Threading.Tasks;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Client.ViewModels;

public abstract class ViewModelBase : ObservableObject, INavigationAware
{
    public async virtual Task OnNavigatedToAsync()
    {
        await Task.CompletedTask;
    }
    
    public async virtual Task OnNavigatedFromAsync()
    {
        await Task.CompletedTask;
    }
}