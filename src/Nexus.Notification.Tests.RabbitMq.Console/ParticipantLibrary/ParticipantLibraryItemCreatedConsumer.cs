using System.Threading.Tasks;
using MassTransit;
using Nexus.ParticipantLibrary.Messages.Interfaces;
using Microsoft.AspNet.SignalR;
using Nexus.Notification.Tests.RabbitMq.Console.Hubs;
using Nexus.Notification.Messages.SignalR;

namespace Nexus.Notification.Tests.RabbitMq.Console.ParticipantLibrary
{
    public class ParticipantLibraryItemCreatedConsumer : IConsumer<IParticipantLibraryItemCreated>
    {
        public Task Consume(ConsumeContext<IParticipantLibraryItemCreated> context)
        {
            System.Console.WriteLine("Received IParticipantLibraryItemCreated " + context.Message.DisplayName + " " + context.Message.SentDate.ToString());

            var channelEvent = new ChannelEvent()
            {
                Name = "ParticipantLibraryItemCreated",
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