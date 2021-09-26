using System;
using Newtonsoft.Json;

namespace DeliveryAPI.Core.Models
{
    /// <summary>
    /// Class to determine the time-frame an order is active.
    /// </summary>
    public class AccessWindow
    {
        private readonly double expireTime = 120000;

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        [JsonConstructor]
        public AccessWindow(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public AccessWindow()
        {
            StartTime = DateTime.UtcNow;
            EndTime = StartTime.AddMilliseconds(expireTime);
        }
    }
}
