using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Microsoft.Azure;
using Microsoft.Azure.Insights;
using Microsoft.Azure.Insights.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Text;

namespace ProductsApp.Controllers
{
    public class InsightsController : ApiController
    {
        private static string token;

        public IHttpActionResult Post([FromBody] Arguments args)
        {
            if (token == null)
                token = Operations.GetAuthorizationHeader("REPLACE Tenant ID", "REPLACE ClientId", "REPLACE Client Secret");

            if (args == null)
            {
                return NotFound();
            }
            return Ok(Operations.GetMetrics(args,ref token));
        }

        public class Arguments
        {
            public string ResourceUri { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public string Timegrain { get; set; }
            public string Subscription { get; set; }
            public List<string> Metrics { get; set; }
        }

        private static class Operations
        {
            public static List<Models.Metric> GetMetrics(Arguments args, ref string token)
            {
                // Authentication and connection
                InsightsClient client;
                try
                {
                    var creds = new TokenCloudCredentials(args.Subscription, token);
                    Uri baseUri = new Uri("https://management.azure.com");
                    client = new InsightsClient(creds, baseUri);
                }
                catch(Exception ex)
                {
                    token = GetAuthorizationHeader("REPLACE Tenant ID", "REPLACE ClientId", "REPLACE Client Secret");
                    var creds = new TokenCloudCredentials(args.Subscription, token);
                    Uri baseUri = new Uri("https://management.azure.com");
                    client = new InsightsClient(creds, baseUri);
                }

                StringBuilder builder = new StringBuilder("(");

                // if the list of metric to be queried is filled
                if (args.Metrics != null)
                {

                    // limit max 25 defitions other this throws a server error
                    int length = 0;
                    if (args.Metrics.Count > 25)
                        length = 25;
                    else
                        length = args.Metrics.Count;

                    for (int i = 0; i < length; i++)
                    {
                        builder.Append("name.value eq '");
                        builder.Append(args.Metrics[i]);
                        builder.Append("'");
                        if (i != length - 1)
                            builder.Append(" or ");
                        else
                            builder.Append(")");
                    }
                }
                else
                {

                    // Get Metrics available
                    MetricDefinitionListResponse metricDefinitionsResponse = client.MetricDefinitionOperations.GetMetricDefinitions(args.ResourceUri, "");

                    // Build filter string based on the metrics available
                    MetricDefinition[] metricDefCollection = metricDefinitionsResponse.MetricDefinitionCollection.Value.ToArray();

                    // limit max 25 defitions other this throws a server error
                    int length = 0;
                    if (metricDefCollection.Length > 25)
                        length = 25;
                    else
                        length = metricDefCollection.Length;

                    for (int i = 0; i < length; i++)
                    {
                        MetricDefinition metricDef = metricDefCollection[i];
                        builder.Append("name.value eq '");
                        builder.Append(metricDef.Name.Value);
                        builder.Append("'");
                        if (i != length - 1)
                            builder.Append(" or ");
                        else
                            builder.Append(")");
                    }
                }
                builder.Append(string.Format("and startTime eq {0} and endTime eq {1} and timeGrain eq {2}", args.Start.ToString("yyyy-MM-ddTHH:mmZ"), args.End.ToString("yyyy-MM-ddTHH:mmZ"), args.Timegrain));
                string filterString = builder.ToString();

                // gets metric data
                MetricListResponse data = client.MetricOperations.GetMetrics(args.ResourceUri, filterString);

                List<Models.Metric> OutputMetrics = new List<Models.Metric>();

                foreach (Microsoft.Azure.Insights.Models.Metric metric in data.MetricCollection.Value)
                {
                    Models.Metric tmpMetric = new Models.Metric();
                    tmpMetric.EndTime = metric.EndTime;
                    tmpMetric.Name = metric.Name.Value;
                    tmpMetric.Properties = metric.Properties;
                    tmpMetric.ResourceId = metric.ResourceId;
                    tmpMetric.StartTime = metric.StartTime;
                    tmpMetric.TimeGrain = metric.TimeGrain;
                    tmpMetric.Unit = metric.Unit;
                    tmpMetric.Values = metric.MetricValues;

                    OutputMetrics.Add(tmpMetric);
                }

                return OutputMetrics;
            }

            public static string GetAuthorizationHeader(string tenantId, string clientId, string clientSecret)
            {
                var context = new AuthenticationContext("https://login.windows.net/" + tenantId);
                ClientCredential creds = new ClientCredential(clientId, clientSecret);
                AuthenticationResult result =
                    context.AcquireToken("https://management.core.windows.net/", creds);
                return result.AccessToken;
            }
        }
    }
}
