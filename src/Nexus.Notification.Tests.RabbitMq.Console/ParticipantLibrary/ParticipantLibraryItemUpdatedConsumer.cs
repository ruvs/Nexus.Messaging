using System.Threading.Tasks;
using MassTransit;
using Nexus.ParticipantLibrary.Messages.Interfaces;

namespace Nexus.Notification.Tests.RabbitMq.Console.ParticipantLibrary
{
    public class ParticipantLibraryItemUpdatedConsumer : IConsumer<IParticipantLibraryItemUpdated>
    {
        public Task Consume(ConsumeContext<IParticipantLibraryItemUpdated> context)
        {
            System.Console.WriteLine("Received IParticipantLibraryItemUpdated " + context.Message.DisplayName + " " + context.Message.SentDate.ToString());
            return Task.FromResult(context.Message);
        }
    }
}