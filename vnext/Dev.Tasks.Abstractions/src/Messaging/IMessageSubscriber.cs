namespace Hyprx.Dev.Messaging;

public interface IMessageSubscriber
{
    Task<bool> ReceiveAsync(IMessage message);
}