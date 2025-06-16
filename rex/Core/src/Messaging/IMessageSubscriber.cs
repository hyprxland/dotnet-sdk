namespace Hyprx.Rex.Messaging;

public interface IMessageSink
{
    Task<bool> ReceiveAsync(IMessage message);
}