<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WarehouseInfo.ascx.cs" Inherits="Photoprint.WebSite.Admin.Controls.WarehouseInfo" %>
<%@ Import Namespace="Photoprint.Core" %>
<h3><%=RM.GetString("Shipping.Settings.WarehouseInfo.Title")%></h3>
<div id="warehouseInfo">
    <div class="abc">
        <div class="abc-a">
            <label for="<%=txtEmployeeName.ClientID %>"><%:RM.GetString("Shipping.Settings.WarehouseInfo.EmployeeName") %></label>
            <asp:TextBox runat="server" ID="txtEmployeeName" CssClass="text" />
            <asp:RequiredFieldValidator runat="server" ID="reqname" ControlToValidate="txtEmployeeName" Display="Dynamic" CssClass="validator" Text="<%$ RM: Validators.Required %>" Enabled="True"></asp:RequiredFieldValidator>
        </div>
        <div class="abc-b">
            <label for="<%=txtEmployeePhone.ClientID %>"><%:RM.GetString("Shipping.Settings.WarehouseInfo.EmployeePhone") %></label>
            <asp:TextBox runat="server" ID="txtEmployeePhone" CssClass="text" />
            <asp:RequiredFieldValidator runat="server" ID="reqPhone" ControlToValidate="txtEmployeePhone" Display="Dynamic" CssClass="validator" Text="<%$ RM: Validators.Required %>" Enabled="True"></asp:RequiredFieldValidator>
        </div>
        <% if (IsMailVisible)
           { %>
            <div class="abc-c">
                <label for="<%= txtEmployeeMail.ClientID %>"><%: RM.GetString("Shipping.Settings.WarehouseInfo.EmployeeMail") %></label>
                <asp:TextBox runat="server" ID="txtEmployeeMail" CssClass="text" />
                <asp:RequiredFieldValidator runat="server" ID="reqMail" ControlToValidate="txtEmployeeMail" Display="Dynamic" CssClass="validator" Text="<%$ RM: Validators.Required %>"></asp:RequiredFieldValidator>
            </div>
        <% } %>
    </div>
    <% if (IsCompanyVisible) {%>
        <div class="ab">
            <div class="ab-a">
                <label for="<%=txtEmployeeCompany.ClientID %>"><%: RM.GetString("Shipping.Settings.WarehouseInfo.EmployeeCompany") %></label>
                <asp:TextBox runat="server" ID="txtEmployeeCompany" CssClass="text" />
                <asp:RequiredFieldValidator runat="server" ID="reqCompany" ControlToValidate="txtEmployeeCompany" Display="Dynamic" CssClass="validator" Text="<%$ RM: Validators.Required %>"></asp:RequiredFieldValidator>
            </div>
        </div>
    <%} %>
    <asp:HiddenField  ID="adressState" runat="server"/>
    <div id="addreress">
        <h3><%=RM.GetString(RS.General.SubMenu.AddressTitle)%></h3>
        <div id="addressInput"></div>
    </div>
</div>	
<script>
   
    var finalData;
    
    const setFinalData = (data) => {
        finalData = data;
    }
    pixlpark.App.addressInputRender(document.getElementById("addressInput"), 
        {
            CountryId: 0,
            FullAddressMode: true,
            UseValidation: true,
            InitialAddress: <%= Newtonsoft.Json.JsonConvert.SerializeObject(Address) %>,
            SetAddressFinalData: setFinalData
        });
    
    const form = document.querySelector('form');
    const addressField = document.querySelector('#adressState');
    form.addEventListener('submit', (event) => {
        addressField.value = JSON.stringify(finalData);
    })
    document.addEventListener('DOMContentLoaded', function(){
		const mainPhoneInput = document.getElementById("txtEmployeePhone");
        pixlpark.App.initPhoneInputControl().then((PhoneInputControlImport) => {
            PhoneInputControlImport.process({ element: mainPhoneInput, phonePredefined: mainPhoneInput.value});
        })
    });

</script>
