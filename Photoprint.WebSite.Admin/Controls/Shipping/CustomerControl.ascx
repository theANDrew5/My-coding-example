<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomerControl.ascx.cs" Inherits="Photoprint.WebSite.Admin.Controls.CustomerControl" %>
<h2><%=RM.GetString(RS.Order.Delivery.CustomerTitle)%></h2>

<fieldset>
    <ol>
        <li>
			<div class="abc">
				<div class="abc-a">
					<label for="<%= txtFirstName.ClientID %>"><%: RM.GetString(RS.Order.Delivery.FirstNameLabel)%></label>
					<asp:TextBox runat="server" ID="txtFirstName" CssClass="text" ValidationGroup="Shipping" />
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Validators.FirstNameReq %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqFirstName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtFirstName" />
				</div>
				<asp:PlaceHolder ID="plhdMiddleName" runat="server">
					<div class="abc-b">
                        <label for="<%= txtMiddleName.ClientID %>"><%: RM.GetString(RS.Order.Delivery.MiddleNameLabel)%></label>
                        <asp:TextBox runat="server" ID="txtMiddleName" CssClass="text" ValidationGroup="Shipping" />
						<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Validators.MiddleNameReq %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqMiddleName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtMiddleName" />
					</div>
				</asp:PlaceHolder>
				<div class="<%= MiddleNameRequired ? "abc-c" : "abc-b" %>">
					<label for="<%= txtLastName.ClientID %>"><%: RM.GetString(RS.Order.Delivery.LastNameLabel)%></label>
					<asp:TextBox runat="server" ID="txtLastName" CssClass="text" ValidationGroup="Shipping" />
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Validators.LastNameReq %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqLastName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtLastName" />
				</div>
			</div>
		</li>
		<li>
			<div class="abc">
                <% if (ShowPhone) {%>
                    <div class="abc-a">
                        <label for="<%= txtPhone.ClientID %>"><%: RM.GetString(RS.Order.Delivery.Phone)%></label>
                        <asp:TextBox runat="server" ID="txtPhone" CssClass="text" />
                    </div>
                <%} %>
                <% if (AdditionalPhoneEnabled) {%>
                <div class="abc-b">
					<label for="<%= txtAdditionalPhone.ClientID %>"><%: RM.GetString(RS.Order.Delivery.AdditonalPhone)%></label>
					<asp:TextBox runat="server" ID="txtAdditionalPhone" CssClass="text" />
				</div>
                <%} %>
                <script>
				    document.addEventListener('DOMContentLoaded', function(){
						const mainPhoneInput = document.getElementById("txtPhone");
						const aditionalPhoneInput = document.getElementById("txtAdditionalPhone");
                        pixlpark.App.initPhoneInputControl().then((PhoneInputControlImport) => {
                            PhoneInputControlImport.process({ element: mainPhoneInput, phonePredefined: mainPhoneInput.value});
                        })
                        if (aditionalPhoneInput != undefined) {
                            pixlpark.App.initPhoneInputControl().then((PhoneInputControlImport) => {
                                PhoneInputControlImport.process({ element: aditionalPhoneInput});
                            });
                        }
                    });
                </script>
			</div>
		</li>
    </ol>
</fieldset>
