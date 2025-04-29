namespace Teammato.Abstractions;

public class User
{
    // The user's nickname (username).
    public string NickName{get;set;}
        
    // The user's email address, used for authentication and notifications.
    public string Email{get;set;}
        
    // The user's password, stored securely and used for authentication.
    public string Password{get;set;}
       
    // The user's profile image.
    public string ?Image{get;set;}
        
    // The user's unique identifier.
   
    public string ?Id{get;set;}


      


}