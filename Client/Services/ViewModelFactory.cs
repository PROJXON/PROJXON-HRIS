using System;
using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Services;

public class ViewModelFactory(IServiceProvider serviceProvider) : IViewModelFactory
{
    public T Create<T>() where T : ViewModelBase
    {
        return serviceProvider.GetRequiredService<T>();
    }
}