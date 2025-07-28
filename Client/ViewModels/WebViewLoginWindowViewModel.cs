using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace Client.ViewModels;

public partial class WebViewLoginWindowViewModel : ViewModelBase
{
    [RelayCommand]
    private async Task WebView_NavigationCompleted()
    {
        
    }
}