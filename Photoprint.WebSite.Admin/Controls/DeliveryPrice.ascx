<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeliveryPrice.ascx.cs" Inherits="Photoprint.WebSite.Admin.Controls.DeliveryPrice" %>
<%@ Import Namespace="Photoprint.Core" %>

<% if (IsVisibleTitle) {%>
    <h2><%=RM.GetString(RS.Shipping.Price.DeliveryPrice)%></h2>
<%}%>

<fieldset>
	<ol id="pricelist">
        <li class="checkbox">
            <span class="line">
                <label class="el-switch el-switch-green">
                    <asp:CheckBox id ="chkFree" runat="server"/>
                    <span class="el-switch-style"></span>
                    <%=RM.GetString(RS.Shipping.Price.Free)%>
                </label>
            </span>
            <script>
                const chkFree = document.querySelector('#chkFree');
                const changeDisplay = () => {
                    document.querySelectorAll('.priceRule').forEach(element => {
                        element.style.display = chkFree.checked ? 'none' : 'block';
                    });
                };
                chkFree.addEventListener('change', changeDisplay);
                document.addEventListener('readystatechange', event => { 
                    if (event.target.readyState === "interactive")
                        changeDisplay();
                });
            </script>
        </li>
        <%
			var fixedList = new List<ShippingPrice>(FixedPriceList);
			if (fixedList.Count == 0) fixedList.Add(new ShippingPrice(0, 0, 0, 0));
		%>
		<% for (var i = 0; i < fixedList.Count; i++){%>
			<li class="priceRule">
				<span class="line">
					<%=RM.GetString(RS.Shipping.Price.Until)%> 
					<input name="txtWeight<%=i%>" class='text shortest center weight' value='<%=fixedList[i].Weight.MetricToPhotolabUnits(CurrentFrontend, MeasurementType.Weight) %>' />
                    <%:CurrentFrontend.GetMeasurementString(MeasurementType.Weight)%>
					<%=RM.GetString(RS.Shipping.Price.PriceIs)%>
					<span style="padding-left:10px;">
						<%=Utility.GetPrefixCurrencySymbol(CurrentFrontend) %> 
						<input name="txtPrice<%=i%>" class='text shortest' value='<%=fixedList[i].Price %>' />
						<%=Utility.GetPostfixCurrencySymbol(CurrentFrontend) %>
					</span>
				</span>
				<a class="semilink add-rule" href="#add-rule" style="margin-left:20px"><%=RM.GetString(RS.Shipping.Price.RuleAdd)%></a>
				<a class="semilink delete delete-rule" href="#remove-rule" style="margin-left:20px;font-size:16px;display:none;"><%=RM.GetString(RS.Shipping.Price.RuleRemove)%></a>
			</li>
		<%} %>
        <li class="priceRule">
			<span class="line">
		        <%
                   var isFixed = OverWeightPrice != null ? (bool?)(OverWeightPrice.AdditionalOneKgPrice < 0.001) : null;
		           var weight = OverWeightPrice?.Weight ?? 0;
		           var weightAdditional = OverWeightPrice?.AdditionalWeight ?? 0;
		           var priceFixed = OverWeightPrice?.Price ?? 0;
		           var priceFloat = OverWeightPrice?.AdditionalOneKgPrice ?? 0;	
		        %>
				<%=RM.GetString(RS.Shipping.Price.OverWeightPrefix)%> <strong id="maxWeight"><%: weight.MetricToPhotolabUnits(CurrentFrontend, MeasurementType.Weight)%></strong> 
				<%:CurrentFrontend.GetMeasurementString(MeasurementType.Weight)%>
				<%=RM.GetString(RS.Shipping.Price.PriceIs)%>
				<span class="hidden" style="<%: isFixed != null ? "display:none" : "display:inline" %>">
					<a class="semilink fixedPriceButton" href="#add-rule"><%=RM.GetString(RS.Shipping.Price.OverWeightFixedLabel)%></a>
				</span>
				<span id="fixedPriceArea" style="<%: isFixed.HasValue && isFixed.Value ? "display:inline" : "display:none" %>">
					<%=RM.GetString(RS.Shipping.Price.OverWeightFixedLabel)%> 
					<span style="padding-left:10px;">
						<%=Utility.GetPrefixCurrencySymbol(CurrentFrontend) %> 
						<input name="overWeightFixedPrice" class='text shortest' value='<%: priceFixed %>' />
						<%=Utility.GetPostfixCurrencySymbol(CurrentFrontend) %>
					</span>
					<span class="note">(<a href="#" class="semilink floatPriceButton"><%=RM.GetString(RS.Shipping.Price.OverWeightFloatLabel)%></a>)</span>
				</span>
				<span class="hidden" style="<%: isFixed != null ? "display:none" : "display:inline" %>">
					<%=RM.GetString(RS.General.Or)%>
					<a class="semilink floatPriceButton" href="#add-rule"><%=RM.GetString(RS.Shipping.Price.OverWeightFloatLabel)%></a>
				</span>
				<span id="floatPriceArea" style="<%: isFixed.HasValue && !isFixed.Value ? "display:inline" : "display:none" %>">
					<%=RM.GetString(RS.Shipping.Price.OverWeightFloatLabel)%>
					<input name="overWeightAdditionalWeight" class='text shortest center' value='<%: weightAdditional.MetricToPhotolabUnits(CurrentFrontend, MeasurementType.Weight) %>' />
					<%: CurrentFrontend.GetMeasurementString(MeasurementType.Weight)%>
					+ 
					<span style="padding-left:10px;">
						<%=Utility.GetPrefixCurrencySymbol(CurrentFrontend) %> 
						<input name="overWeightFloatPrice" class='text shortest' value='<%: priceFloat %>' />
						<%=Utility.GetPostfixCurrencySymbol(CurrentFrontend) %>
					</span>
					<span class="note">(<a href="#" class="semilink fixedPriceButton"><%=RM.GetString(RS.Shipping.Price.OverWeightFixedLabel)%></a>)</span>
				</span>
			</span>
		</li>
        <li class="checkbox">
            <span class="line">
                <label class="el-switch el-switch-green">
                    <input type="checkbox" id="isConstrainMaxWeight" name="isConstrainMaxWeight" <%:MaximumWeight > 0 ? "checked": string.Empty %>/>
                    <span class="el-switch-style"></span>
                    <%=RM.GetString(RS.Shipping.Price.MaximumWeight)%>
                </label>
            </span>  
            <span id="maxWeightConstrain" class="line" style="<%: MaximumWeight > 0 ? "display:inline" : "display:none" %>">
                <input name="txtMaxWeightConstrain" type="text" class="text shortest" value="<%: MaximumWeight %>"/>
                <%: CurrentFrontend.GetMeasurementString(MeasurementType.Weight)%>
            </span>          
        </li>
	</ol>
</fieldset>

<script type="text/javascript">
    $('#isConstrainMaxWeight').click(function () {
        $('#maxWeightConstrain').toggle();
    });

    $(".fixedPriceButton").click(function () {
        $("#fixedPriceArea").show();
        $("#floatPriceArea").hide();
        $("#fixedPriceArea input:first").focus();
        $("#pricelist .hidden").hide();
        $("#floatPriceArea input").val('');
        return false;
    });
    $(".floatPriceButton").click(function () {
        $("#fixedPriceArea").hide();
        $("#floatPriceArea").show();
        $("#floatPriceArea input:first").focus();
        $("#pricelist .hidden").hide();
        $("#fixedPriceArea input").val('');
        return false;
    });
    
    function updatePriceListState() {
        var index = $("#pricelist li").length - 2;
        $("#pricelist .add-rule, #pricelist .delete-rule").hide();
        if (index > 1) { $("#pricelist li:nth-child(" + index + ")").find('.delete-rule').show(); }
        $("#pricelist li:nth-child(" + index + ")").find('.add-rule').show();

        var maxWeight = 0;
        $("#pricelist .weight").each(function (i, value) {
            var weight = parseFloat($(value).val().replace(",", "."));
            if (maxWeight <= weight) maxWeight = weight;
        });
        $("#maxWeight").html(maxWeight);
    }

    $(document).on('change', '#pricelist .weight', updatePriceListState);
    $(document).on('click', '#pricelist .add-rule', function () {
        var clone = $(this).closest('li').clone();
        var index = $("#pricelist li").length - 3;
        $(this).closest('li').after(clone);

        clone.find("input[name^=txtWeight]").attr("name", "txtWeight" + index);
        clone.find("input[name^=txtPrice]").attr("name", "txtPrice" + index);

        updatePriceListState();
        return false;
    });
    $(document).on('click', "#pricelist .delete-rule", function () {
        $(this).closest('li').remove();
        updatePriceListState();
        return false;
    });

    updatePriceListState();
</script>