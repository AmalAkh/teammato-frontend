using System.Text.Json.Serialization;

namespace Teammato.Abstractions;

public class GameSession
{
    
    [JsonPropertyName("gameName")]
    public string GameName { get; set; }
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("requiredPlayersCount")]
    public int RequiredPlayersCount { get; set; }
    [JsonPropertyName("owner")]
    public User Owner { get; set; }
    [JsonPropertyName("participants")]
    public List<User> Participants { get; set; }

}