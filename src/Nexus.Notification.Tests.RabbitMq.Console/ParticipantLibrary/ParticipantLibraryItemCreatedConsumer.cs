using System.Threading.Tasks;
using MassTransit;
using Nexus.ParticipantLibrary.Messages.Interfaces;

namespace Nexus.Notification.Tests.RabbitMq.Console.ParticipantLibrary
{
    public class ParticipantLibraryItemCreatedConsumer : IConsumer<IParticipantLibraryItemCreated>
    {
        public Task Consume(ConsumeContext<IParticipantLibraryItemCreated> context)
        {
            System.Console.WriteLine("Received IParticipantLibraryItemCreated " + context.Message.DisplayName + " " + context.Message.SentDate.ToString());
            return Task.FromResult(context.Message);
        }
    }
}