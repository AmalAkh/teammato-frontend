using System.Text;
using System.Text.Json;

namespace Teammato.Services;
using System.Net.Http;
using Models;
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

    public static void Init(string baseUrl)
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
    
    public static async Task<bool> UploadProfileImage(Stream imageStream, string fileName)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, "users/upload-image");
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
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "users/profile");
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
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, "users/profile-update");
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
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "languages/list");
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
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"languages/{ISOName}");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);

        var response = await _client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
    
    public static async Task<bool> AddLanguage(string ISOName)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"languages/new");
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