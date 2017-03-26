using Microsoft.AspNet.SignalR;
using Nexus.Notification.Messages.SignalR;
using System.Threading.Tasks;

namespace Nexus.Notification.Tests.RabbitMq.Console.Hubs
{
    public class BroadcasterHub : Hub
    {
        public string ConnectionId { get { return Context.ConnectionId; } }

        public void Subscribe()
        {
            Groups.Add(Context.ConnectionId, "channel");
        }

        public Task Subscribe(string channel)
        {
            return Groups.Add(Context.ConnectionId, channel);
        }

        //public Task Subscribe(object channel)
        //{
        //    return Groups.Add(Context.ConnectionId, channel.ToString());
        //}

        public void Unsubscribe(string customerId)
        {
            Groups.Remove(Context.ConnectionId, customerId);
        }


        public override Task OnConnected()
        {
            // Set connection id for just connected client only
            return Clients.Client(Context.ConnectionId).SetConnectionId(Context.ConnectionId);
        }

        // Server side methods called from client
        //public Task Subscribe(int id)
        //{
        //    return Groups.Add(Context.ConnectionId, id.ToString());
        //}

        public Task Unsubscribe(int id)
        {
            return Groups.Remove(Context.ConnectionId, id.ToString());
        }


        public void BroadcastChannelEvent(ChannelEvent channelEvent)
        {
            Clients.Group(channelEvent.ChannelName).OnEvent(channelEvent.Name, channelEvent);
        }
    }
}
