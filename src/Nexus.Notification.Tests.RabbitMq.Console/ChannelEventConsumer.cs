using System.Threading.Tasks;
using MassTransit;
using System;
using Nexus.Notification.Messages.SignalR;

public class ChannelEventConsumer : IConsumer<ChannelEvent>
{
    public Task Consume(ConsumeContext<ChannelEvent> context)
    {
        Console.WriteLine("Received " + context.Message.ChannelName + " " + context.Message.Timestamp.ToString());
        return Task.FromResult(context.Message);
    }
}