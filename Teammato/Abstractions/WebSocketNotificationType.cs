using System.Text.Json.Serialization;

namespace Teammato.Abstractions;


public enum WebSocketNotificationType
{
    NewPlayerJoined, PlayerLeavedGameSession, NewChatMessage, GameSessionStarted, SuccessAuth,GameSessionCancelled
}
public class WebSocketNotification
{
    [JsonPropertyName("type")]
    public WebSocketNotificationType Type {get;set;}
    [JsonPropertyName("content")]
    public string Content {get;set;}
}
