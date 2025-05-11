using System.Text.Json.Serialization;

namespace Teammato.Abstractions;

public class Chat
{
    // The Name of the chat.
    [JsonPropertyName("name")]
    public string Name{get;set;}
    [JsonPropertyName("id")]
    // The unique identifier for the chat.
    public string ?Id{get;set;}
    
    [JsonPropertyName("owner")]
    // The unique identifier for the chat.
    public User Owner{get;set;}
    
    [JsonPropertyName("lastMessage")]
    public Message? LastMessage{get;set;}
        
    // A list of participants in the chat.
    [JsonPropertyName("participants")]
    public List<User> Participants{get;set;}
    public List<Message> Messages{get;set;}
    // A list of messages associated with the chat.
    
    [JsonPropertyName("image")]
    public string Image{get;set;}
    // Constructor for the Chat class that initializes the Participants and Messages lists.
    // The lists are initialized to avoid null references and to provide an empty list by default.
    public Chat()
    {
        // Initializes an empty list of Participants, which will store all users in the chat.
        Participants = new List<User>();
       
    }
}