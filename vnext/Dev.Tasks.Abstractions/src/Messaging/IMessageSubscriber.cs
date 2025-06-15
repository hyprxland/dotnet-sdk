namespace Hyprx.Dev.Messaging;

public interface IMessageSink
{
    Task<bool> ReceiveAsync(IMessage message);
}