using MassTransit;
using Nexus.Notification.Messages.SignalR;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Nexus.Notification.Tests.RabbitMq.Console
{
    public class Program
    {

        static void Main(string[] args)
        {
            string _rabbitMqHostUri = GetAppSetting("RabbitMqHost");
            string _rabbitMqUser = GetAppSetting("RabbitMqUser");
            string _rabbitMqSignalRServiceQueue = GetAppSetting("SignalRNotificationServiceQueue");

            var bus = BusConfigurator.ConfigureBus();

            bus.Start();

            var sendToUri = new Uri(_rabbitMqHostUri + _rabbitMqSignalRServiceQueue);
            var endPoint = bus.GetSendEndpoint(sendToUri);

            bool exit = false;
            while (!exit)
            {
                System.Console.WriteLine("Press 'S' to send a message, or 'Q' to quit.");
                var key = System.Console.ReadKey();
                System.Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.S:
                        //bus.Send(new ChannelEvent { ChannelName = "theChannelName", Name = "theName", Timestamp = DateTimeOffset.Now });
                        //bus.Publish (new ChannelEvent { ChannelName = "theChannelName", Name = "theName", Timestamp = DateTimeOffset.Now });
                        var task = SendMessage(bus);
                        break;

                    case ConsoleKey.Q:
                        exit = true;
                        break;

                    default:
                        System.Console.WriteLine("Unknown input. Please try again.");
                        break;
                }
            }
            bus.Stop();
        }

        public static async Task SendMessage(ISendEndpointProvider sendEndpointProvider)
        {
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(GetAppSetting("RabbitMqHost") + GetAppSetting("SignalRNotificationServiceQueue")));

            await endpoint.Send(new ChannelEvent { ChannelName = "theChannelName", Name = "theName", Timestamp = DateTimeOffset.Now });
        }

        private static string GetAppSetting(string appSetting)
        {
            if (ConfigurationManager.AppSettings[appSetting] != null)
                return ConfigurationManager.AppSettings[appSetting];
            return string.Empty;
        }
    }
}
