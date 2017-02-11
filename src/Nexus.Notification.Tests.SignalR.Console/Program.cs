using Microsoft.Owin.Hosting;
using Microsoft.Owin.Cors;
using Owin;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using NServiceBus;
using System.Threading.Tasks;
using Nexus.Notification.Messages.SignalR;
using NServiceBus.Logging;

namespace Nexus.Notification.Tests.SignalR.Console
{
    public class Program
    {
        private static string CONNECTION_STRING_NSB_DB_KEY = "Nsb_Notification";
        static ILog logger = LogManager.GetLogger<Program>();

        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();

            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            //////string url = ConfigurationManager.AppSettings["SignalRUrl"];
            //////using (WebApp.Start(url))
            //////{
            //////    System.Console.WriteLine("Server running on {0}", url);
            //////    System.Console.ReadLine();
            //////}
        }

        static async Task AsyncMain()
        {
            System.Console.Title = "NotificationServiceClient";

            var endpointConfiguration = new EndpointConfiguration("Nexus.Notification");

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();

            var sqlConnString = ConfigurationManager.ConnectionStrings[CONNECTION_STRING_NSB_DB_KEY].ConnectionString;
            transport.ConnectionString(sqlConnString);

            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.EnableInstallers();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            await RunLoop(endpointInstance);

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            string url = ConfigurationManager.AppSettings["SignalRUrl"];
            using (WebApp.Start(url))
            {
                while (true)
                {
                    System.Console.WriteLine("Server running on {0}", url);
                    //System.Console.ReadLine();

                    logger.Info("Press 'S' to send a message, or 'Q' to quit.");
                    var key = System.Console.ReadKey();
                    System.Console.WriteLine();

                    switch (key.Key)
                    {
                        case System.ConsoleKey.S:
                            // Instantiate the command

                            var signalRData = new SignalRNotificationData()
                            {
                                Prop1 = "111",
                                Prop2 = "222"
                            };

                            var command = new ChannelEvent()
                            {
                                ChannelName = "BroadcasterHub",
                                Name = "theName",
                                Data = signalRData
                            };

                            // Send the command to the local endpoint
                            logger.Info($"Sending ChannelEvent command, OrderId = {command.Name}");
                            await endpointInstance.SendLocal(command).ConfigureAwait(false);

                            break;

                        case System.ConsoleKey.Q:
                            return;

                        default:
                            logger.Info("Unknown input. Please try again.");
                            break;
                    }
                }
            }
        }
    }
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfig = new HubConfiguration()
                {
                    EnableDetailedErrors = ConfigurationManager.AppSettings["IncludeErrorDetailPolicy"] != "Never",
                };
                map.RunSignalR(hubConfig);
            });
        }
    }
}
