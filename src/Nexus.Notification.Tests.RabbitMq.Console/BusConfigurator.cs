using MassTransit;
using MassTransit.RabbitMqTransport;
using Nexus.Notification.Tests.RabbitMq.Console.ParticipantLibrary;
using System;
using System.Configuration;

namespace Nexus.Notification.Tests.RabbitMq.Console
{
    public static class BusConfigurator
    {
        public static IBusControl ConfigureBus(string hostUri, string username, Action<IRabbitMqBusFactoryConfigurator, IRabbitMqHost> registration = null)
        {
            string _rabbitMqSignalRServiceQueue = GetAppSetting("SignalRNotificationServiceQueue");

            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(hostUri), hst =>
                {
                    hst.Username(username);
                    hst.Password(username);
                });

                cfg.ReceiveEndpoint(host, _rabbitMqSignalRServiceQueue, c =>
                {
                    c.Consumer<ParticipantLibraryItemCreatedConsumer>();
                    c.Consumer<ParticipantLibraryItemUpdatedConsumer>();
                });

                registration?.Invoke(cfg, host);
            });
        }

        private static string GetAppSetting(string appSetting)
        {
            if (ConfigurationManager.AppSettings[appSetting] != null)
                return ConfigurationManager.AppSettings[appSetting];
            return string.Empty;
        }
    }
}
