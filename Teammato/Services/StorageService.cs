using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using Teammato.Abstractions;
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
}