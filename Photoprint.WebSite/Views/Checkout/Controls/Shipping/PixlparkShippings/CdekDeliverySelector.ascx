<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CdekDeliverySelector.ascx.cs" Inherits="Photoprint.WebSite.Controls.CdekDeliverySelector" %>

<script type="text/javascript" src="<%= FileHelper.JS.CdekWidgetScriptUrl %>" id="ISDEKscript"></script>
<script type="text/javascript" src="<%= FileHelper.JS.CdekWidgetUrl %>"></script>

<fieldset onsubmit="return false;">
    <div id="cdek-widget" style="height: 600px;"></div>
    <input hidden name="cdekWidgetResult" id="cdekWidgetResult" />
    <script type="text/javascript">
       
        $.ajax({
            url: "/api/shippings/cdek?isdek_action=getCityById"+"&postalId="+<%= CurrentPostal.Id %>+"&cityId=" + <%= CityFromId %>, type: "GET",
             success: function (response) {
                 var widgetCdek = new CDEKWidget(
                     "cdekWidgetResult",
                     null,
                    <%= CurrentPostal?.Id %>,
                    <%= CurrentLanguage?.Id ?? 0 %>,
                    response,
                    null,
                    <%= ProductsListJson %>,
                    true,
                    <%= CurrentPostal?.IsShippingPricePaidSeparately.ToString().ToLower() %>,
                    "<%= YandexMapApiKey%>",
                    <%= CurrentPhotolab.Id %> ,
                    <%= LoggedInUser.Id%>);

        widgetCdek.startWidget("<%= FileHelper.JS.CdekTemplateUrl %>");
            }
        });
    </script>
</fieldset>
