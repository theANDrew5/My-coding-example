﻿using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetOpenAuth.Messaging;

namespace Photoprint.WebSite.API.Controllers
{
    public class YandexDeliveryController : BaseApiController
    {
        private readonly IYandexDeliveryClient _yandexDeliveryClient;
        public YandexDeliveryController(IAuthenticationService authenticationService, IYandexDeliveryClient yandexDeliveryClient) : base(authenticationService)
        {
            _yandexDeliveryClient = yandexDeliveryClient;
        }
        public class DeliveryOptionsRequest
        {
            public string Term { get; set; }
            public string DeliveryType { get; set; }
            public string Token { get; set; }
            public string SenderId { get; set; }
        }

        public class DeliveryOptionsDto
        {
            public string Error { get; set; }
            public string TariffId { get; set; }
            public string Type { get; set; }
            public string Tag { get; set; }
            public string PartnerId { get; set; }
            public string PartnerName { get; set; }
            public string LogoUrl { get; set; }
            public string Delivery { get; set; }
            public string DeliveryForCustomer { get; set; }
            public string DeliveryForSender { get; set; }
            public List<string> PickupPointIds { get; set; }
            public string Cost { get; set; }
        }

        public class PickupPointsRequest
        {
            public List<string> Ids;
            public string token;
        }

        [HttpPost]
        [Route("api/shippings/yadex/getDeliveryOptions")]
        public HttpResponseMessage GetDeliveryOptions(DeliveryOptionsRequest data)
        {
            var addresses = _yandexDeliveryClient.GetAddressesByTerm(data.Term, data.Token);
            if (addresses == null) return Request.CreateResponse(HttpStatusCode.NotFound, new { Error = "Город не найден" });

            var model = new
            {
                senderId = data.SenderId,
                to = new { geoId = addresses[0].GeoId },
                deliveryType = data.DeliveryType
            };

            var deliveryOptions = _yandexDeliveryClient.GetDeliveryOptions(model, data.Token);
            if (deliveryOptions == null) return Request.CreateResponse(HttpStatusCode.NotFound, new { Error = "Варианты доставки не найдены" });

            var response = new List<DeliveryOptionsDto>(20);
            foreach (var opt in deliveryOptions)
            {
                var index = response.FindIndex(x => x.TariffId == opt.TariffId);
                if (index >= 0)
                {
                    response[index].PickupPointIds.AddRange(opt.PickupPointIds);
                    continue;
                }
                response.Add(
                    new DeliveryOptionsDto
                    {
                        Cost = opt.Cost.DeliveryForSender.ToString(),
                        TariffId = opt.TariffId,
                        Delivery = opt.Cost.Delivery.ToString(),
                        DeliveryForCustomer = opt.Cost.DeliveryForCustomer.ToString(),
                        DeliveryForSender = opt.Cost.DeliveryForSender.ToString(),
                        LogoUrl = opt.Delivery.Partner.LogoUrl,
                        PartnerId = opt.Delivery.Partner.Id.ToString(),
                        PartnerName = opt.Delivery.Partner.Name,
                        PickupPointIds = opt.PickupPointIds,
                        Type = opt.Delivery.Type.ToJSON()
                    }
                    );
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        [Route("api/shippings/yadex/getPickupPoints")]
        public HttpResponseMessage GetPickupPoints(PickupPointsRequest data)
        {
            List<YandexDeliveryPickupPointResponse> result = new List<YandexDeliveryPickupPointResponse>();
            var length = data.Ids.Count;
            if (length > 100)
            {
                var count = Convert.ToInt32(Math.Ceiling((double)length / 100));
                var parts = Enumerable.Range(0, count)
                      .Select(i => data.Ids.Skip(i * 99).Take(99).ToArray()).ToList();

                foreach (var part in parts)
                {
                    var response = _yandexDeliveryClient.GetPickupPoints(part, data.token);
                    result.AddRange(_yandexDeliveryClient.GetPickupPoints(part, data.token) ?? new List<YandexDeliveryPickupPointResponse>());
                }
            }
            else
            {
                result.AddRange(_yandexDeliveryClient.GetPickupPoints(data.Ids, data.token) ?? new List<YandexDeliveryPickupPointResponse>());
            }

            if (result.Count == 0) return Request.CreateResponse(HttpStatusCode.OK, new { Error = "Пункты выдачи заказов не найдены" });
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
