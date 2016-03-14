using Microsoft.Azure.Insights.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductsApp.Models
{
    public class Metric
    {

        [JsonProperty(PropertyName = "Values", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<MetricValue> Values { get; set; }

        [JsonProperty(PropertyName = "Name", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Properties", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, string> Properties { get; set; }

        [JsonProperty(PropertyName = "ResourceId", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ResourceId { get; set; }

        [JsonProperty(PropertyName = "StartTime", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime StartTime { get; set; }

        [JsonProperty(PropertyName = "EndTime", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime EndTime { get; set; }

        [JsonProperty(PropertyName = "TimeGrain", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TimeSpan TimeGrain { get; set; }

        [JsonProperty(PropertyName = "Unit", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(StringEnumConverter))]
        public Unit Unit { get; set; }
    }
}