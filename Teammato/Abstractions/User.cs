using System.Text.Json.Serialization;

namespace Teammato.Abstractions;

public class User
{
    // The user's nickname (username).
    [JsonPropertyName(("nickName"))]
    public string NickName{get;set;}
        
    // The user's email address, used for authentication and notifications.
    [JsonPropertyName(("email"))]
    public string Email{get;set;}
        
    // The user's password, stored securely and used for authentication.
    public string Password{get;set;}
    [JsonPropertyName(("image"))]
    // The user's profile image.
    public string ?Image{get;set;}
        
    // The user's unique identifier.
    [JsonPropertyName(("id"))]
    public string ?Id{get;set;}

    public override bool Equals(object? obj)
    {
        if (typeof(User) != obj.GetType())
        {
            return false;
        }
        return (obj as User).Id == Id;
    }
}