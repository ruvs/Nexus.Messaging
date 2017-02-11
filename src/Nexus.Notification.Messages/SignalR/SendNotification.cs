using NServiceBus;
using System;

namespace Nexus.Notification.Messages.SignalR
{
    public class SendNotification : ICommand
    {
        public string Name { get; set; }

        public string ChannelName { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public object Data { get; set; }
    }
}
