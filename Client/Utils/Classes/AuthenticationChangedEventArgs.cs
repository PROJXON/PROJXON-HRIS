using System;

namespace Client.Utils.Classes;

public class AuthenticationChangedEventArgs(bool isAuthenticated) : EventArgs
{
    public bool IsAuthenticated { get; } = isAuthenticated;
}