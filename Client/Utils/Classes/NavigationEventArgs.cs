using System;
using Client.Utils.Enums;

namespace Client.Utils.Classes;

public class NavigationEventArgs(ViewModelType viewModelType, int id = 0) : EventArgs
{
    public ViewModelType ViewModelType { get; } = viewModelType;
    public int EntityId { get; } = id;
}