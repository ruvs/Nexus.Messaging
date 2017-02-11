using Microsoft.AspNet.SignalR;
using Nexus.Notification.Messages.SignalR;

namespace Nexus.Notification.SignalR.Console
{
    public class BroadcasterHub : Hub
    {
        public void BroadcastChannelEvent(ChannelEvent channelEvent)
        {
            Clients.Group(channelEvent.ChannelName).OnEvent(channelEvent.ChannelName, channelEvent);
        }
    }
}
