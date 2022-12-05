<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourierSelector.ascx.cs" Inherits="Photoprint.WebSite.Admin.Controls.CourierShippingSelector" %>

<h2><%=RM.GetString(RS.Order.Delivery.Title)%></h2>
<fieldset class="profile">
	<ol>
		<li>
            <div class="abc">
                <div class="abc-c">
					<label for="<%=txtCountry.ClientID %>"><%=RM.GetString(RS.Order.Delivery.Country)%></label>
					<asp:TextBox ID="txtCountry" runat="server" CssClass="text" />
				</div>
                <div class="abc-b">
                    <label for="<%=txtRegion.ClientID %>"><%=RM.GetString(RS.Order.Delivery.State)%></label>
                    <asp:TextBox ID="txtRegion" runat="server" CssClass="text" />
                </div>
                <div class="abc-c">
                    <% if (HasAdditionalAddresses)
                       {%>
                        <label for="<%=ddlCities.ClientID %>"><%=RM.GetString(RS.Order.Delivery.City)%></label>
                        <controls:DecoratedDropDownList ID="ddlCities" runat="server" AutoPostBack="true" onChange="$('#ddlTarget').val('ddlCities');" />
                        <asp:CustomValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" ID="cstDdlCity" runat="server" OnServerValidate="cstDddlCity_ServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlCities" />
                    <%   }
                       else
                       { %>
                        <asp:HiddenField runat="server" ID="txtAddressId"/>
                        <div class="abc-a">
                            <label for="<%=txtCity.ClientID %>"><%=RM.GetString(RS.Order.Delivery.City)%></label>
                            <asp:TextBox ID="txtCity" runat="server" CssClass="text" />
                        </div>
                    <%   }%>
                </div>
			</div>
        </li>
        <asp:PlaceHolder ID="plhdAddress" runat="server">
            <li>
                <div class="abc">
                    <div class="abc-a">
                        <label for="<%=txtStreet.ClientID %>"><%=RM.GetString(RS.Order.Delivery.AddressStreet)%></label>
                        <asp:TextBox ID="txtStreet" runat="server" CssClass="text" />
                    </div>
                    <div class="abc-b">
                        <label for="<%=txtHouse.ClientID %>"><%=RM.GetString(RS.Order.Delivery.AddressHouse)%></label>
                        <asp:TextBox ID="txtHouse" runat="server" CssClass="text" />
                    </div>
                    <div class="abc-c">
                        <label for="<%=txtFlat.ClientID %>"><%=RM.GetString(RS.Order.Delivery.AddressFlat)%></label>
                        <asp:TextBox ID="txtFlat" runat="server" CssClass="text" />
                    </div>
                </div>
            </li>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhdAddressLines" runat="server" Visible="false">
            <li>
                <div class="ab">
                    <div class="ab-a">
                        <label for="<%=txtAddressLine1.ClientID%>"><%=RM.GetString(RS.Order.Delivery.AddressLine1)%></label>
                        <asp:TextBox ID="txtAddressLine1" runat="server" ValidationGroup="Shipping" CssClass="text" />					    
                        <asp:RequiredFieldValidator Display="Dynamic" ForeColor="" CssClass="validator" ID="reqAddressLine1" ValidationGroup="Shipping" runat="server" ControlToValidate="txtAddressLine1" />
                    </div>
                    <div class="ab-b">
                        <label for="<%=txtAddressLine2.ClientID%>"><%=RM.GetString(RS.Order.Delivery.AddressLine2)%></label>
                        <asp:TextBox ID="txtAddressLine2" runat="server" ValidationGroup="Shipping" CssClass="text" />
                    </div>
                </div>
            </li>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhdPostalCode" runat="server">
            <li>
                <div class="abc">
                    <div class="abc-a">
                        <label for="<%=txtPostalCode.ClientID %>"><%=RM.GetString(RS.Order.Delivery.PostalCode)%></label>
                        <asp:TextBox ID="txtPostalCode" runat="server" CssClass="text" />
                        <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPostalCode" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPostalCode" />

                    </div>
                </div>
            </li>	
        </asp:PlaceHolder>
        <li>
			<div class="ab">
				<div class="ab-a">
					<label for="<%=txtDescription.ClientID %>"><%=RM.GetString(RS.Order.Delivery.Additionally)%></label>
					<asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="text" />			
				</div>
			</div>
		</li>  
        <li>
            <div class="abc">
                <div class="abc-a">
                    <label for="<%=txtCompanyName.ClientID%>"><%=RM.GetString(RS.Order.Delivery.CompanyNameLabel)%></label>
                    <asp:TextBox ID="txtCompanyName" runat="server" ValidationGroup="Shipping" CssClass="text" />	
                </div>
            </div>
        </li>
	</ol>
</fieldset>