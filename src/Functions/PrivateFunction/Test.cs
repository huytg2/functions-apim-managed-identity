using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Monitor.OpenTelemetry.Exporter; // Add this using directive
using OpenTelemetry; // Add this using directive
using OpenTelemetry.Trace; // Add this using directive

namespace PublicFunction
{
    public class Test
    {
        static Test()
        {
            Sdk.CreateTracerProviderBuilder()
                .AddSource("AzureMonitorDistro")
                .AddAzureMonitorTraceExporter(o =>
                {
                    o.ConnectionString = "InstrumentationKey=InstrumentationKey=464a188f-1b0e-4760-8357-0c14da32d6ba;IngestionEndpoint=https://germanywestcentral-1.in.applicationinsights.azure.com/;LiveEndpoint=https://germanywestcentral.livediagnostics.monitor.azure.com/;ApplicationId=32316e2b-741c-41ab-99b5-6f622663a4bf";
                })
                .Build();
        }

        [FunctionName("test")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("Starting function call");

            var claims = req.HttpContext.User.Claims.ToList();
            claims.ForEach(c => log.LogInformation($"{c.Type} - {c.Value} - {c.ValueType}"));

            var roleClaim = claims.Single(c => c.Type == "roles");
            log.LogInformation($"Role Claim: {roleClaim.Type} - {roleClaim.Value} - {roleClaim.ValueType}");
            
            return new OkObjectResult(new TestResponse { 
                DateOfMessage = DateTime.Now, 
                Message = $"Hello from the Private Function! The APIM Managed Identity has been assigned to the role: {roleClaim.Value}" 
            });
        }
    }
}

public class TestResponse
{
    public string Message { get; set; }
    public DateTime DateOfMessage { get; set; }
}