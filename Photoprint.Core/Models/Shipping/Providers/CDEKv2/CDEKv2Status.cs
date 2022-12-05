using Newtonsoft.Json;
using System;

namespace Photoprint.Core.Models
{
    public class CDEKv2Status
    {
        [JsonProperty("code")]
        public CDEKv2StatusCode Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("date_time")]
        public DateTime Date { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }

    public enum CDEKv2StatusCode
    {
        ACCEPTED,
        PROCESSING,
        READY,
        REMOVED,
        INVALID,
        CREATED,
        WAITING,
        SUCCESSFUL,
        
        RECEIVED_AT_SHIPMENT_WAREHOUSE,
        READY_FOR_SHIPMENT_IN_SENDER_CITY,
        RETURNED_TO_SENDER_CITY_WAREHOUSE,
        TAKEN_BY_TRANSPORTER_FROM_SENDER_CITY,
        
        SENT_TO_TRANSIT_CITY,
        ACCEPTED_IN_TRANSIT_CITY,
        
        ACCEPTED_AT_TRANSIT_WAREHOUSE,
        RETURNED_TO_TRANSIT_WAREHOUSE,
        
        READY_FOR_SHIPMENT_IN_TRANSIT_CITY,
        TAKEN_BY_TRANSPORTER_FROM_TRANSIT_CITY,
        
        SENT_TO_SENDER_CITY,
        SENT_TO_RECIPIENT_CITY,
        
        ACCEPTED_IN_SENDER_CITY,
        ACCEPTED_IN_RECIPIENT_CITY,

        SHIPPED_TO_DESTINATION,
        PASSED_TO_TRANSIT_CARRIER,

        ACCEPTED_AT_RECIPIENT_CITY_WAREHOUSE,
        ACCEPTED_AT_PICK_UP_POINT,

        POSTOMAT_POSTED,
        
        TAKEN_BY_COURIER,
        RETURNED_TO_RECIPIENT_CITY_WAREHOUSE,

        DELIVERED,
        NOT_DELIVERED,

        READY_FOR_APPOINTMENT,
        APPOINTED_COURIER,
        DONE,
        PROBLEM_DETECTED,
        PROCESSING_REQUIRED,
    }
}
