using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Configuration;

namespace Nexus.Notification.Tests.RabbitMq.Console
{
    public static class BusConfigurator
    {
        public static IBusControl ConfigureBus(Action<IRabbitMqBusFactoryConfigurator, IRabbitMqHost> registration = null)
        {
            string _rabbitMqHostUri = GetAppSetting("RabbitMqHost");
            string _rabbitMqUser = GetAppSetting("RabbitMqUser");
            string _rabbitMqSignalRServiceQueue = GetAppSetting("SignalRNotificationServiceQueue");

            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(_rabbitMqHostUri), hst =>
                {
                    hst.Username(_rabbitMqUser);
                    hst.Password(_rabbitMqUser);
                });

                cfg.ReceiveEndpoint(host, _rabbitMqSignalRServiceQueue, c => c.Consumer<ChannelEventConsumer>());

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
