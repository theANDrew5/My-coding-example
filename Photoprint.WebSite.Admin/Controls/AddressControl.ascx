<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressControl.ascx.cs" Inherits="Photoprint.WebSite.Admin.Controls.AddressControl" %>
<%@ Import Namespace="Photoprint.Core" %>
<%@ Import Namespace="Newtonsoft.Json.Linq" %>


<asp:HiddenField  ID="adressState" runat="server"/>
<div id="addressInput"></div>
<script>
   
    var finalData = {};
    
    const setFinalData = (data) => {
        finalData = data;
    }

    pixlpark.App.addressSelectorRender(document.getElementById("addressInput"), 
        {
            CanEdit: <%: CanEdit.ToString().ToLower()%>,
            CountryId: 0,
            FullAddressMode: <%: FullAddressMode.ToString().ToLower()%>,
            UseValidation: <%: UseValidation.ToString().ToLower()%>,
            InitialAddress: <%= JObject.FromObject(new AddressDTO(Address))%>,
            ContractorAddressAvailable: <%:ContractorAddressAvailable.ToString().ToLower()%>,
            ContractorAddressNotEqual: <%: ContractorAddressNotEqual.ToString().ToLower()%>,
            InitialContractorAddress: <%= JObject.FromObject(new AddressDTO(ContractorAddress))%>,
            SetSelectorFinalData: setFinalData,
            IsNewDelivery: <%: IsNewDelivery.ToString().ToLower()%>
        });
    
    const form = document.querySelector('form');
    const addressField = document.querySelector('#adressState');
    form.addEventListener('submit', (event) => {
        addressField.value = JSON.stringify(finalData);
    })


</script>