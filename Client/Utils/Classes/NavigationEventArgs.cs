using System;
using Client.Utils.Enums;

namespace Client.Utils.Classes;

public class NavigationEventArgs(ViewModelType viewModelType) : EventArgs
{
    public ViewModelType ViewModelType { get; } = viewModelType;
}