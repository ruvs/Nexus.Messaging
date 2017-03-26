using System.Threading.Tasks;
using MassTransit;
using Nexus.ParticipantLibrary.Messages.Interfaces;
using Nexus.Notification.Messages.SignalR;
using Microsoft.AspNet.SignalR;
using Nexus.Notification.Tests.RabbitMq.Console.Hubs;

namespace Nexus.Notification.Tests.RabbitMq.Console.ParticipantLibrary
{
    public class ParticipantLibraryItemUpdatedConsumer : IConsumer<IParticipantLibraryItemUpdated>
    {
        public Task Consume(ConsumeContext<IParticipantLibraryItemUpdated> context)
        {
            System.Console.WriteLine("Received IParticipantLibraryItemUpdated " + context.Message.DisplayName + " " + context.Message.SentDate.ToString());

            var channelEvent = new ChannelEvent()
            {
                Name = "ParticipantLibraryItemUpdated",
                ChannelName = "ParticipantLibrary",
                Data = context.Message,
                Timestamp = context.Message.SentDate
            };

            var _context = GlobalHost.ConnectionManager.GetHubContext<BroadcasterHub>();
            _context.Clients.Group(channelEvent.ChannelName).OnEvent(channelEvent.ChannelName, channelEvent);

            return Task.FromResult(context.Message);
        }
    }
}