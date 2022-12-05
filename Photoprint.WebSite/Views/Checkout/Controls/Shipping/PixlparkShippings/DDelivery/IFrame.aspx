<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="IFrame.aspx.cs" Inherits="Photoprint.WebSite.Views.Checkout.Controls.Shipping.PixlparkShippings.DDelivery.IFrame" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <link href='//fonts.googleapis.com/css?family=PT+Sans:400,400italic,700,700italic&subset=latin,cyrillic-ext' rel='stylesheet' type='text/css'>
	<link href="<%= GetStyleUrl() %>" rel="stylesheet" />
</head>
<body>
		<script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
		<script type="application/javascript">
			var ddCaptionConfig = {
				caption1:"Ячейка",
				caption2:"Живой пункт",
				caption3:"Наличными",
				caption4:"Банковскими картами",
				caption5:"Предоплата"
			}
		</script>
		
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/modernizr.custom.76185.js"></script>
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/jquery.mCustomScrollbar.concat.min.js"></script>
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/jquery.custom-radio-checkbox.min.js"></script>
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/jquery.formtips.js"></script>
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/jquery.maskedinput.js"></script>
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/jquery.JSON.min.js"></script>
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/ddelivery.iframe.js"></script>
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/ddelivery.map.js"></script>
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/ddelivery.header.js"></script>
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/ddelivery.courier.js"></script>
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/ddelivery.contact_form.js"></script>
		<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/ddelivery.type_form.js"></script>
		<script src="//api-maps.yandex.ru/2.0/?load=package.full&lang=ru-RU" async="async" type="text/javascript"></script>
		
		<% const int width = 1000; const int height = 650;%>
		<style type="text/css">
			.map-popup{ width: <%= width %>px; height: <%= height %>px;}
			.map-canvas{ width: <%= width %>px; height: <%= height-90 %>px; }
			.map-popup .map-popup__main{ height: <%= height-90 %>px;}
		</style>
		<div id="ddelivery"></div>
		<div id="ddelivery_loader">
			<div class="map-popup">
				<div class="map-popup__head">
					<p>DDelivery. Доставка в удобную Вам точку.</p>

					<div class="map-popup__head__close">&nbsp;</div>
				</div>
				<div class="map-popup__main">
					<div class="map-popup__main__overlay">&nbsp;</div>
					<div class="map-popup__main__delivery">
						<div class="loader">
							<p>Подождите пожалуйста, мы ищем лучшие предложения</p>
							<img src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/tems/default/img/ajax_loader_horizont.gif"/>
						</div>
						<div>
							<p class="load_error">
								Произошла ошибка, <a href="javascript:void(0)">повторить запрос</a>
							</p>
						</div>
					</div>
				</div>
				<div class="map-popup__bott">
					<a href="http://ddelivery.ru/" target="blank">Сервис доставки DDelivery.ru</a>
				</div>
			</div>
		</div>

		<script>
			window.shippingId = parseInt("<%: Request.QueryString["sid"] %>");
			$(function(){
				DDeliveryIframe.init("/api/shippings/ddelivery", "/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/tems/default/");
			});
		</script>
</body>
</html>
