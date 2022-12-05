using System.Collections.Generic;
using System.Web.Http;
using Photoprint.Core.Models;
using Photoprint.Core.Models.Novaposhta;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Admin.API
{
    public class NovaposhtaController : BaseApiController
    {
        private readonly INovaposhtaV2Services _novaposhtaV2Services;

        public NovaposhtaController(IAuthenticationService authenticationService, INovaposhtaV2Services novaposhtaV2Services) : base(authenticationService)
        {
            _novaposhtaV2Services = novaposhtaV2Services;
        }

        [Route("api/novaposhta/senders/{apiKey}")]
        [HttpGet]
        public IEnumerable<NovaposhtaInfo> GetSenders(string apiKey)
        {
            var senders = _novaposhtaV2Services.GetSenders(apiKey);
            return senders;
        }

        [Route("api/novaposhta/contacts/{apiKey}/{senderRef}")]
        [HttpGet]
        public IEnumerable<NovaposhtaInfo> GetContacts(string apiKey, string senderRef)
        {
            var senders = _novaposhtaV2Services.GetPersonContacts(apiKey, senderRef);
            return senders;
        }

        [Route("api/novaposhta/addresses/{apiKey}/{senderRef}")]
        [HttpGet]
        public IEnumerable<NovaposhtaInfo> GetAddresses(string apiKey, string senderRef)
        {
            var addresses = _novaposhtaV2Services.GetPersonAddresses(apiKey, senderRef);
            return addresses;
        }

        [Route("api/novaposhta/checksettings/")]
        [HttpPost]
        public AddressValidationResult GetAddresses(NovaposhtaSettings settings)
        {
            var validity = _novaposhtaV2Services.IsSettingsValid(settings);
            return validity;
        }

        [Route("api/novaposhta/warehouses/{apiKey}/{cityName}")]
        [HttpGet]
        public IEnumerable<NovaposhtaInfo> GetWarehouses(string apiKey, string cityName)
        {
            var warehouses = _novaposhtaV2Services.GetWarehouses(apiKey, cityName);
            return warehouses;
        } 
    }
}