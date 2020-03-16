using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TeslaLib.Converters;

namespace TeslaLib.Models
{
    // Values as of August 2017 with a 2014 Model S.  Note the GPS_as_of and timestamp fields.  Looks like
    // Tesla dropped some digits from a time stamp originally, maybe.
    // {"response":{"shift_state":null,"speed":null,"power":0,"latitude":47.6506,"longitude":-122.119804,"heading":193,"gps_as_of":1503881736,"timestamp":1503881737541}}
    // As of Feb 2020:
    // {"response":{"gps_as_of":1581305038,"heading":194,"latitude":47.650627,"longitude":-122.11982,"native_latitude":47.650627,
    //  "native_location_supported":1,"native_longitude":-122.11982,"native_type":"wgs","power":0,"shift_state":null,"speed":null,
    //  "timestamp":1581305041023}}
    public class DriveStateStatus
    {
        [JsonProperty(PropertyName = "shift_state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ShiftState? ShiftState { get; set; }

        [JsonProperty(PropertyName = "speed")]
        public int? Speed { get; set; }

        /// <summary>
        /// Degrees N of the equator
        /// </summary>
        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// Degrees W of the prime meridian
        /// </summary>
        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// Integer compass heading (0-359)
        /// </summary>
        [JsonProperty(PropertyName = "heading")]
        public int Heading { get; set; }

        [JsonProperty(PropertyName = "gps_as_of")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime GpsAsOf { get; set; }

        [JsonProperty(PropertyName = "power")]
        public double? Power { get; set; }

        // There are new native_longitude and native_latitude settings.  I don't know why we would want those instead.
    }

}