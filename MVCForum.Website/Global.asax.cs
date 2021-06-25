using System.Web.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;

namespace MvcForum.Web
{
    using System;
    using System.Web;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = System.Web.Configuration.WebConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];
            //var instrumentationKey = WebConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];
            //// Create a TelemetryConfiguration instance.
            //TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();
            //config.InstrumentationKey = instrumentationKey;
            //QuickPulseTelemetryProcessor quickPulseProcessor = null;
            //config.DefaultTelemetrySink.TelemetryProcessorChainBuilder
            //    .Use((next) =>
            //    {
            //        quickPulseProcessor = new QuickPulseTelemetryProcessor(next);
            //        return quickPulseProcessor;
            //    })
            //    .Build();

            //var quickPulseModule = new QuickPulseTelemetryModule();

            //// Secure the control channel.
            //// This is optional, but recommended.
            //quickPulseModule.AuthenticationApiKey = "APPINSIGHTS_APIKEY";
            //quickPulseModule.Initialize(config);
            //quickPulseModule.RegisterTelemetryProcessor(quickPulseProcessor);

            //// Create a TelemetryClient instance. It is important
            //// to use the same TelemetryConfiguration here as the one
            //// used to setup Live Metrics.
            //TelemetryClient client = new TelemetryClient(config);
        }
    }
}