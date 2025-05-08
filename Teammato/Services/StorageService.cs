using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using Teammato.Abstractions;

namespace Teammato.Services;

public class StorageService
{
    public static User CurrentUser { get; set; }
    
    public static List<Language> Languages { get; set; }
    
    public static async Task<User> GetUserAsync()
    {
        try{
            var user = await BlobCache.UserAccount.GetObject<User>("user");

            return user;
        }catch (KeyNotFoundException ex)
        {
            return null;
        }
    }
    public static async Task<List<Chat>> GetChatsAsync()
    {
        try{
            var chats = await BlobCache.UserAccount.GetObject<List<Chat>>("chats");

            return chats;
        }catch (KeyNotFoundException ex) {
            return new List<Chat>();
        }
    }

    public static async Task<List<Message>> GetMessagesAsync(string chatId)
    {
        try{
            var chats = await BlobCache.UserAccount.GetObject<List<Message>>($"chat-{chatId}-messages");

            return chats;
        }catch (KeyNotFoundException ex) {
            return new List<Message>();
        }
    }
    
    public static async Task<UserProfile> GetProfileAsync()
    {
        try{
            var userProfile = await BlobCache.UserAccount.GetObject<UserProfile>("user_profile");
            return userProfile;
        }catch (KeyNotFoundException ex)
        {
            return null;
        }
    }
    
    public static async Task<List<Game>> GetGamesAsync()
    {
        try{
            var games = await BlobCache.UserAccount.GetObject<List<Game>>("favorite_games");
            return games;
        }catch (KeyNotFoundException ex)
        {
            return null;
        }
    }
    
    public static async Task<List<Language>> GetLanguagesAsync()
    {
        try{
            var languages = await BlobCache.UserAccount.GetObject<List<Language>>("preferred_languages");
            return languages;
        }catch (KeyNotFoundException ex)
        {
            return null;
        }
    }
}