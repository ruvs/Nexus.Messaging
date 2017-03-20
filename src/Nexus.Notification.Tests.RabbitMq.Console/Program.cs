using MassTransit;
using Nexus.Notification.Messages.SignalR;
using Nexus.ParticipantLibrary.Messages.Interfaces;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Nexus.Notification.Tests.RabbitMq.Console
{
    public class Program
    {
        private static string _rabbitMqHostUri;
        private static string _rabbitMqUser;
        private static string _rabbitMqSignalRServiceQueue;

        static void Main(string[] args)
        {
            GetConfigurationValues();

            var bus = BusConfigurator.ConfigureBus(_rabbitMqHostUri, _rabbitMqUser);

            bus.Start();

            bool exit = false;
            while (!exit)
            {
                System.Console.WriteLine("Press 'S' to send a message, 'C, U' to publish or 'Q' to quit.");
                var key = System.Console.ReadKey();
                System.Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.S:
                        var sendTask = SendMessage(bus, GetAppSetting("RabbitMqHost") + GetAppSetting("SignalRNotificationServiceQueue"));
                        break;

                    case ConsoleKey.C:
                    case ConsoleKey.U:
                        var publishTask = PublishMessage(bus, key.Key);
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

        private static void GetConfigurationValues()
        {
            System.Console.WriteLine("Press 'I' for integration test or any other key for dev test.");
            if (System.Console.ReadKey().Key == ConsoleKey.I)
            {
                _rabbitMqHostUri = GetAppSetting("RabbitMqHostIntegrationTests");
            }
            else
            {
                _rabbitMqHostUri = GetAppSetting("RabbitMqHost");
            }
            _rabbitMqUser = GetAppSetting("RabbitMqUser");
            _rabbitMqSignalRServiceQueue = GetAppSetting("SignalRNotificationServiceQueue");
            System.Console.WriteLine();
        }

        public static async Task SendMessage(ISendEndpointProvider sendEndpointProvider, string endPointUri)
        {
            System.Console.WriteLine("Sending...");
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(endPointUri));

            await endpoint.Send(new ChannelEvent { ChannelName = "theChannelName", Name = "theName", Timestamp = DateTimeOffset.Now });
        }

        public static async Task PublishMessage(IPublishEndpoint publishEndpoint, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.C:
                    System.Console.WriteLine("Publishing IParticipantLibraryItemCreated...");
                    System.Console.WriteLine();
                    await publishEndpoint.Publish<IParticipantLibraryItemCreated>(new
                    {
                        NexusKey = Guid.NewGuid(),
                        DisplayCode = "PLC"
                    });
                    break;
                case ConsoleKey.U:
                    System.Console.WriteLine("Publishing IParticipantLibraryItemUpdated...");
                    System.Console.WriteLine();
                    await publishEndpoint.Publish<IParticipantLibraryItemUpdated>(new
                    {
                        NexusKey = Guid.NewGuid(),
                        DisplayCode = "PLU"
                    });
                    break;
            }
        }

        private static string GetAppSetting(string appSetting)
        {
            if (ConfigurationManager.AppSettings[appSetting] != null)
                return ConfigurationManager.AppSettings[appSetting];
            return string.Empty;
        }
    }
}
