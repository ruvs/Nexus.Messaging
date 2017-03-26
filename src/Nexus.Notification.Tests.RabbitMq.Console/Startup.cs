using MassTransit;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System.Configuration;

namespace Nexus.Notification.Tests.RabbitMq.Console
{
    public class Startup
    {
        private static string _rabbitMqHostUri;
        private static string _rabbitMqUser;
        private static string _rabbitMqSignalRServiceQueue;

        static void Main(string[] args)
        {
            string url = ConfigurationManager.AppSettings["SignalRUrl"];

            GetConfigurationValues();

            var bus = BusConfigurator.ConfigureBus(_rabbitMqHostUri, _rabbitMqUser);

            bus.Start();

            using (WebApp.Start(url))
            {
                System.Console.WriteLine(string.Format("Server running at {0}", url));
                System.Console.ReadLine();
            }
        }

        private static void GetConfigurationValues()
        {
            _rabbitMqHostUri = GetAppSetting("RabbitMqHost");
            _rabbitMqUser = GetAppSetting("RabbitMqUser");
            _rabbitMqSignalRServiceQueue = GetAppSetting("SignalRNotificationServiceQueue");
        }

        private static string GetAppSetting(string appSetting)
        {
            if (ConfigurationManager.AppSettings[appSetting] != null)
                return ConfigurationManager.AppSettings[appSetting];
            return string.Empty;
        }

        public void Configuration(IAppBuilder app)
        {
            var includeErrorDetailPolicy = ConfigurationManager.AppSettings["IncludeErrorDetailPolicy"];

            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfig = new HubConfiguration()
                {
                    EnableDetailedErrors = includeErrorDetailPolicy != "Never",
                };
                map.RunSignalR(hubConfig);
            });
        }
    }
}
