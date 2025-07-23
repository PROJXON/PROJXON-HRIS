using Client.ViewModels;

namespace Client.Services;

public interface IViewModelFactory
{
    T Create<T>() where T : ViewModelBase;
}