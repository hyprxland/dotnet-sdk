namespace Hyprx.Rex.Messaging;

public interface IMessageBus
{
    void Send(IMessage message);

    Task SendAsync(IMessage message);
}