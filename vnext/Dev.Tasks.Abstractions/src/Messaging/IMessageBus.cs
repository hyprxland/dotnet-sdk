namespace Hyprx.Dev.Messaging;

public interface IMessageBus
{
    void Send(IMessage message);
}