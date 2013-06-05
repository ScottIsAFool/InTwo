using Cimbalino.Phone.Toolkit.Services;

namespace InTwo.Model
{
    public interface IExtendedNavigationService : INavigationService
    {
        bool IsNetworkAvailable { get; }
        bool IsNetworkAvailableSilent { get; }
    }
}
