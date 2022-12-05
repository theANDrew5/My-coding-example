namespace Photoprint.Core.Models
{
    public interface IShippingServiceProviderSettings
    {
        bool UpdateShippingAddressesAutomatically { get; }
        bool RegisterOrderInProviderService { get; }
        bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; }
        bool SupportAddresesSynchronization { get; }
        bool ShowAddressTab { get; }
        bool MuteNotificationAfterAddressesUpdated { get; }
    }
}