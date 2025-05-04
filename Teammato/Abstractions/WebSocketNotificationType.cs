namespace Teammato.Abstractions;


public enum WebSocketNotificationType
{
    NewPlayerJoined, PlayerLeavedGameSession, NewChatMessage, GameSessionStarted, SuccessAuth
}
public class WebSocketNotification
{
    public WebSocketNotificationType Type {get;set;}
    public string Content {get;set;}
}
