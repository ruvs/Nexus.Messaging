using Microsoft.AspNet.SignalR;
using Nexus.Notification.Messages.SignalR;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace Nexus.Notification.SignalR.Console
{
    public class SendChannelEventHandler : IHandleMessages<ChannelEvent>
    {
        static ILog logger = LogManager.GetLogger<SendChannelEventHandler>();

        public Task Handle(ChannelEvent message, IMessageHandlerContext context)
        {
            var srData = (SignalRNotificationData)message.Data;

            logger.Info($"Received Notification, Name = {message.ChannelName}, Data = {srData.Prop1},{srData.Prop2}");

            var _context = GlobalHost.ConnectionManager.GetHubContext<BroadcasterHub>();
            _context.Clients.Group(message.ChannelName).OnEvent(message.ChannelName, message);
            _context.Clients.All.OnEvent(message.ChannelName, message);

            return Task.CompletedTask;
        }
    }
}
