using System.Text;

namespace Photoprint.Core.Models
{
	public class ShippingServiceProviderImportResult
	{
		public int DeletedAddressesCount { get; }
		public int UpdatedAddressesCount { get; }
        public int InsertedAddressesCount { get; }

		public int ReceivedAddressCount { get; }
		public int DuplicateAddressCount { get; }
		public int ObsoleteAddressCount { get; }
		public int DisableSyncAddressesCount { get; }


		public string ErrorMessage { get; }
        public string ErrorLog { get; }

        public string FinalLog => string.IsNullOrWhiteSpace(ErrorLog)
            ? "Sync results\nTOTAL:" +
              $"\n\tDeleted addresses: {DeletedAddressesCount}" +
              $"\n\tUpdated addresses: {UpdatedAddressesCount}" +
              $"\n\tAdded addresses: {InsertedAddressesCount}" +
              "\nDETAILS:" +
              $"\n\tReceived addresses: {ReceivedAddressCount}" +
              $"\n\tDuplicates addresses found: {DuplicateAddressCount}" +
              $"\n\tObsolete addresses found: {ObsoleteAddressCount}" +
              $"\n\tSynchronisation disabled addresses: {DisableSyncAddressesCount}"
            : null;
	    public ShippingServiceProviderImportResult() { }

        public ShippingServiceProviderImportResult(string errorMessage, string errorLog)
		{
			ErrorMessage = errorMessage;
			ErrorLog = errorLog;
		}

		public ShippingServiceProviderImportResult(int deletedAddressesCount, int updatedAddressesCount,
            int insertedAddressesCount, int receivedAddressesCount, int duplicateAddressesCount,
            int obsoleteAddressesCount, int disabledAddressesCount)
		{
			InsertedAddressesCount = insertedAddressesCount;
			DeletedAddressesCount = deletedAddressesCount;
			UpdatedAddressesCount = updatedAddressesCount;
			ReceivedAddressCount = receivedAddressesCount;
			DuplicateAddressCount = duplicateAddressesCount;
			ObsoleteAddressCount = obsoleteAddressesCount;
			DisableSyncAddressesCount = disabledAddressesCount;
        }

	}
}