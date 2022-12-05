<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PicpointSelector.ascx.cs" Inherits="Photoprint.WebSite.Admin.Controls.PicpointSelector" %>
<asp:HiddenField runat="server" ID="selectedAddress"/>
<fieldset>
    <ol>
    <li>
        <div id="plugin_container"></div>
    </li>
    <li>
        <div id="pluginData">
            <!-- ko if: addressReady -->
            <div data-bind="with: selectedAddress">
                <div>
                    <span>
                        <span style="font-weight: 700"><%= RM.GetString(RS.Order.Delivery.Description)%></span>
                        <span data-bind="html: Description"></span>
                    </span>
                </div>
                <div>
                    <span>
                        <span style="font-weight: 700"><%= RM.GetString(RS.Order.Delivery.ShippingAddress)%></span>
                        <span data-bind="html: AddressLine"></span>
                    </span>
                </div>
            </div>
            <!-- /ko -->
        </div>
    </li>
    </ol>
</fieldset>
<script>
    function pickPointPlugin() {
        const self = this;
        self.pluginUrl = 'https://pickpoint.ru/select/postamat.js';
        self.ikn = '<%: Settings.IKN%>';
        self.shippingId = '<%: CurrentPostal.Id%>';
        self.addressReady = ko.observable(false);
        self.selectedAddress = ko.observable({});
        
        self.onPickPointSelected = (result) => {
            const selectedAddress = document.querySelector('#selectedAddress');
            const address = 
            {
                ShippingId: self.shippingId,
                Country: result.country,
                City: result.cityname,
                AddressLine: result.address,
                Region: result.region,
                House: result.house,
                Latitude: result.latitude.toString(),
                Longitude: result.longitude.toString(),
                PostalCode: result.postcode,
                Description: result.name,
                DeliveryProperties: {
                    pickpointAddressInfo: {
                        PostamatCode: result.id,
                        PostamatId: result.bdid,
                        CityId: result.cityid
                    }
                }
            }
            selectedAddress.value = JSON.stringify(address);
            self.selectedAddress(address);
            self.addressReady(true);
        }
    
        const script = document.createElement('script');
        script.src = self.pluginUrl;
        script.async = true;
        script.onload = () => {
            const pickpointPluginController = window.PickPoint;
            pickpointPluginController?.siteShowWithCallback(
                'plugin_container',
                self.onPickPointSelected, {
                    city: '',
                    hideCloseButton: true,
                    hideFilterPanel: true,
                    disableFilters: true,
                    noheader: true,
                    ikn: self.ikn
                }
            );
        }
        const firstScriptTag = document.getElementsByTagName("script")[0];
	    if (firstScriptTag.parentNode !== null)
		    firstScriptTag.parentNode.insertBefore(script, firstScriptTag);

    }
    $(document).ready(() => {
        ko.applyBindings(new pickPointPlugin(), document.querySelector('#pluginData'));
    });
</script>