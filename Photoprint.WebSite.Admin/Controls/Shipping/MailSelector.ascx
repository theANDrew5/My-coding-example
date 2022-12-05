<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="MailSelector.ascx.cs" Inherits="Photoprint.WebSite.Admin.Controls.MailShippingSelector" %>

<h2><%=RM.GetString(RS.Order.Delivery.Title)%></h2>

<fieldset>
    <asp:HiddenField ID="ddlTarget" runat="server"/>
    <ol>
		<li>
			<div class="abc">
                <div class="abc-c">
					<asp:PlaceHolder ID="plhdCountrySelector" runat="server" Visible="True">
						<label for="<%=ddlCountries.ClientID %>"><%= RM.GetString(RS.Order.Delivery.Country) %></label>
						<asp:DropDownList ID="ddlCountries" runat="server" ValidationGroup="Shipping" AutoPostBack="True" onChange="$('#ddlTarget').val('ddlCountries');"/>
					</asp:PlaceHolder>
					<asp:PlaceHolder ID="plhdCountryInput" runat="server" Visible="False">
						<label for="<%= txtCountry.ClientID %>"><%= RM.GetString(RS.Order.Delivery.Country) %></label>
						<asp:TextBox ID="txtCountry" runat="server" ValidationGroup="Shipping" CssClass="text" />
					</asp:PlaceHolder>
				</div>
                <div class="abc-b">
                    <asp:PlaceHolder ID="plhdRegionSelector" runat="server" Visible="True">
                        <label for="<%= ddlRegions.ClientID %>"><%= RM.GetString(RS.Order.Delivery.State) %></label>
                        <asp:DropDownList ID="ddlRegions" runat="server" ValidationGroup="Shipping" AutoPostBack="True" onChange="$('#ddlTarget').val('ddlRegions');"/>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhdRegionInput" runat="server" Visible="False">
                        <label for="<%= txtRegion.ClientID %>"><%= RM.GetString(RS.Order.Delivery.State) %></label>
                        <asp:TextBox ID="txtRegion" runat="server" ValidationGroup="Shipping" CssClass="text" />
                    </asp:PlaceHolder>
                </div>
                <div class="abc-a">
                    <asp:PlaceHolder ID="plhdCitySelector" runat="server" Visible="True">
                        <label for="<%= ddlCities.ClientID %>"><%= RM.GetString(RS.Order.Delivery.City) %></label>
                        <asp:DropDownList ID="ddlCities" runat="server" ValidationGroup="Shipping" AutoPostBack="True" onChange="$('#ddlTarget').val('ddlCities');"/>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhdCityInput" runat="server" Visible="False">
                        <label for="<%= txtCity.ClientID %>"><%= RM.GetString(RS.Order.Delivery.City) %></label>
                        <asp:TextBox ID="txtCity" runat="server" ValidationGroup="Shipping" CssClass="text" />
                    </asp:PlaceHolder>
                </div>
			</div>
		</li>
        <asp:PlaceHolder ID="plhdSelectorAddresses" runat="server">
            <li>
                <label for="<%=ddlAddresses.ClientID%>"><%=RM.GetString(RS.Order.Delivery.Storage)%></label>
                <asp:DropDownList ID="ddlAddresses" runat="server" AutoPostBack="true" ValidationGroup="Shipping" onChange="$('#ddlTarget').val('ddlAddresses');" />
            </li>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhdManualAddress" runat="server">
			<% if (!UseAddressLines)
               { %>
                <li>
                    <div class="abc">
                        <div class="abc-a">
                            <label for="<%= txtAddressStreet.ClientID %>"><%= RM.GetString(RS.Order.Delivery.AddressStreet) %></label>
                            <asp:TextBox ID="txtAddressStreet" runat="server" ValidationGroup="Shipping" CssClass="text" />
                        </div>
                        <div class="abc-b">
                            <label for="<%= txtAddressHouse.ClientID %>"><%= RM.GetString(RS.Order.Delivery.AddressHouse) %></label>
                            <asp:TextBox ID="txtAddressHouse" runat="server" ValidationGroup="Shipping" CssClass="text" />
                        </div>
                        <div class="abc-c">
                            <label for="<%= txtAddressFlat.ClientID %>"><%= RM.GetString(RS.Order.Delivery.AddressFlat) %></label>
                            <asp:TextBox ID="txtAddressFlat" runat="server" ValidationGroup="Shipping" CssClass="text" />
                        </div>
                    </div>
                </li>
            <% }
               else { %>
                <li>
                    <div class="ab">
                        <div class="ab-a">
                            <label for="<%= txtAddressLine1.ClientID %>"><%= RM.GetString(RS.Order.Delivery.AddressLine1) %></label>
                            <asp:TextBox ID="txtAddressLine1" runat="server" ValidationGroup="Shipping" CssClass="text" />
                        </div>
                    </div>
                    <div class="ab">
                        <div class="ab-a">
                            <label for="<%= txtAddressLine2.ClientID %>"><%= RM.GetString(RS.Order.Delivery.AddressLine2) %></label>
                            <asp:TextBox ID="txtAddressLine2" runat="server" ValidationGroup="Shipping" CssClass="text" />
                        </div>
                    </div>
                </li>
            <% } %>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhdPostalCode" runat="server">
            <li>
                <div class="abc">
                    <div class="abc-a">
                        <label for="<%= txtPostalCode.ClientID %>"><%= RM.GetString(RS.Order.Delivery.PostalCode) %></label>
                        <asp:TextBox ID="txtPostalCode" runat="server" CssClass="text" />
                        <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Validators.PostalCodeReq %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPostalCode" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPostalCode" />
                    </div>
                </div>
            </li>
        </asp:PlaceHolder>
    </ol>
</fieldset>