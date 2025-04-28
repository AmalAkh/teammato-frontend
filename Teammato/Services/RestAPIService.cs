using System.Text.Json;

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
    public static async Task<bool> SignIn(string login, string password)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "users/signin");
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
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "users/access-token");
        
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

    public static async Task LogOut()
    { 
        SecureStorage.Remove("refresh_token");
        IsLoggedIn = false;
    }

    public static  void Init(string baseUrl)
    {
        _client.BaseAddress = new Uri(baseUrl);
        
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

    }
    
}