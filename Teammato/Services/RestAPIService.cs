using System.Text.Json;
using Teammato.Abstractions;

namespace Teammato.Services;
using System.Net.Http;
public class RestAPIService
{
    private static readonly HttpClient _client = new HttpClient();
    private static string _refreshToken;
    private static string _accessToken;

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

        
        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            _accessToken = await response.Content.ReadAsStringAsync();
            
        }
        else
        {
            throw new FailedAccessTokenRequestException();
        }
      
    }

    public static async Task<List<Chat>> GetChats()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/chats/list");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<List<Chat>>(await response.Content.ReadAsStringAsync());
            
        }
        else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await UpdateAccessToken();
            return await GetChats();
        }

        throw new Exception();
    }
    public static async Task<List<Message>> GetMessages(string chatId)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"api/messages/{chatId}/messages");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<List<Message>>(await response.Content.ReadAsStringAsync());
            
        }
        else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await UpdateAccessToken();
            return await GetMessages(chatId);
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
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/users/info");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<User>(await response.Content.ReadAsStringAsync());
            
        }
        else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await UpdateAccessToken();
            return await GetUser();
        }

        throw new Exception();
    }
    public static async Task CheckAuthorization()
    {
        _refreshToken = await SecureStorage.GetAsync("refresh_token");

        if (_refreshToken == null)
        {
            IsLoggedIn = false;
            return;
        }

        await UpdateAccessToken();
        
        StorageService.CurrentUser = await GetUser();

    }
    
}