using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Akavache;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using Teammato.Abstractions;

namespace Teammato.Services;
using System.Reactive.Linq;
using System.Net.Http;
using Models;
public class RestAPIService
{
    private static readonly HttpClient _client = new HttpClient();
    private static string _refreshToken;
    private static string _accessToken;

    internal static string GetAccessToken()
    {
        return _accessToken;
    }
    public static bool IsLoggedIn
    {
        get;
        private set;
    } = true;
    
    public static async Task SendMessage(string text, string chatId)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"api/messages/{chatId}/new");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        Dictionary<string, string> data = new Dictionary<string, string>();

        data.Add("Content", text);
        

        var content = new StringContent(JsonSerializer.Serialize(data), null, "text/json");
        request.Content = content;
        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            
            
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        {
            UpdateAccessToken();
        }
      
    }
    public static async Task<bool> SignIn(string login, string password)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/users/signin");
        
        Dictionary<string, string> data = new Dictionary<string, string>();

        data.Add("login", login);
        data.Add("password", password);

        var content = new StringContent(JsonSerializer.Serialize(data), null, "text/json");
        request.Content = content;
        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            _refreshToken = await response.Content.ReadAsStringAsync();
            await SecureStorage.Default.SetAsync("refresh_token", _refreshToken);
            await UpdateAccessToken();
            IsLoggedIn = true;
            
            return true;
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        {
            return false;
        }
        return false;
    }

    public static async Task UpdateAccessToken()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/users/access-token");
        
        request.Headers.Add("Authorization", "Bearer " + _refreshToken);

        try
        {
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                _accessToken = await response.Content.ReadAsStringAsync();

            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) 

            {
                throw new FailedAccessTokenRequestException();
            }
        }
        catch (HttpRequestException e)
        {
            
        }

    }

    public static async Task<List<Chat>> GetChats()
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return await StorageService.GetChatsAsync();
        }

        try
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/chats/list");
            request.Headers.Add("Authorization", "Bearer " + _accessToken);
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var chats = JsonSerializer.Deserialize<List<Chat>>(await response.Content.ReadAsStringAsync());
                await BlobCache.UserAccount.InsertObject<List<Chat>>("chats", chats);
                return chats;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await UpdateAccessToken();
                return await GetChats();
            }
        }
        catch (HttpRequestException e)
        {
            return await StorageService.GetChatsAsync();
        }

        throw new Exception();
    }
    public static async Task<List<Message>> GetMessages(string chatId)
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return await StorageService.GetMessagesAsync(chatId);
        }

        try
        {


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"api/messages/{chatId}/messages");
            request.Headers.Add("Authorization", "Bearer " + _accessToken);
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var messages = JsonSerializer.Deserialize<List<Message>>(await response.Content.ReadAsStringAsync());
                await BlobCache.UserAccount.InsertObject<List<Message>>($"chat-{chatId}-messages", messages);
                return messages;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await UpdateAccessToken();
                return await GetMessages(chatId);
            }
        }
        catch (HttpRequestException e)
        {
            return await StorageService.GetMessagesAsync(chatId);
        }
        

        throw new Exception();
    }

    public static async Task LogOut()
    { 
        SecureStorage.Remove("refresh_token");
        IsLoggedIn = false;
    }

    public static string BaseAddress
    {
        get
        {
            return _client.BaseAddress.ToString();
        }
        set
        {
            _client.BaseAddress = new Uri(value);
        }
    }
    
    public static  void Init(string baseUrl)

    {
        _client.BaseAddress = new Uri(baseUrl);
    }

    public static async Task<User> GetUser()
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return await StorageService.GetUserAsync();
        }

        try
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/users/info");
            request.Headers.Add("Authorization", "Bearer " + _accessToken);
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var user = JsonSerializer.Deserialize<User>(await response.Content.ReadAsStringAsync());
                await BlobCache.UserAccount.InsertObject<User>("user", user);
                return user;

            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await UpdateAccessToken();
                return await GetUser();
            }
        }
        catch (HttpRequestException e)
        {
            return await StorageService.GetUserAsync();
        }

        throw new Exception();
    }
    
    

    
    
    public static async Task<bool> UploadProfileImage(Stream imageStream, string fileName)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, "api/users/upload-image");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        
        var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(imageStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/" + Path.GetExtension(fileName).TrimStart('.'));
        
        content.Add(streamContent, "image", fileName);
        request.Content = content;

        var response = await _client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
    
    public static async Task<UserProfile> GetProfile()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/users/profile");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);

        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            // Deserialize response into object
            var content = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var userProfile = await JsonSerializer.DeserializeAsync<UserProfile>(content, options);
            
            return userProfile;
        }

        return null;
    }

    public static async Task<bool> UpdateProfile(UserProfile userProfile)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, "api/users/profile-update");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);

        try
        {
            var data = new Dictionary<string, string>
            {
                { "newNickname", userProfile.Nickname },
                { "newDescription", userProfile.Description ?? "" }
            };
            
            var content = new StringContent(JsonSerializer.Serialize(data), null, "text/json");
            request.Content = content;
            
            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<List<Language>> GetLanguages()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/languages/list");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        
        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var languages = JsonSerializer.Deserialize<List<Language>>(content, options);
            return languages;
        }
        
        return null;
    }

   
       

        

   public static async Task<bool> RemoveLanguage(string ISOName)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"api/languages/{ISOName}");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);

        
        var response = await _client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
    public static async Task CheckAuthorization()
    {
        _refreshToken = await SecureStorage.GetAsync("refresh_token");

        if (_refreshToken == null)
        {
            IsLoggedIn = false;
            
            return;
        }

        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return;
        }

        try
        {


            await UpdateAccessToken();
            StorageService.CurrentUser = await GetUser();
            await WebSocketService.ConnectAsync(new Uri(new Uri(BaseAddress.Replace("http", "ws")),"ws"));
        }
        catch (FailedAccessTokenRequestException e)
        {
            IsLoggedIn = false;
            return;
        }

        
        
    }
    
    public static async Task<bool> AddLanguage(string ISOName)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"api/languages/new");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);

        var data = new Dictionary<string, string>
        {
            { "isoname", ISOName }
        };
            
        var content = new StringContent(JsonSerializer.Serialize(data), null, "text/json");
        request.Content = content;
        
        var response = await _client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}