using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Akavache;
using Firebase.Analytics;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using OneSignalSDK.DotNet;
using OneSignalSDK.DotNet.Core.Internal.Utilities;
using Teammato.Abstractions;

namespace Teammato.Services;
using System.Reactive.Linq;
using System.Net.Http;
using Abstractions;
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
    
    public static async Task<bool> SendMessage(string text, string chatId)
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
            return true;
            
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        {
            await UpdateAccessToken();
            await SendMessage(text, chatId);
            return false;
        }
        return false;

    }
    
    public static async Task<bool> SignUp(string email, string nickname, string password)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/users/new");
        Dictionary<string, string> data = new Dictionary<string, string>();

        data.Add("nickname", nickname);
        data.Add("email", email);
        data.Add("password", password);

        var content = new StringContent(JsonSerializer.Serialize(data), null, "text/json");
        request.Content = content;
        var signup_response = await _client.SendAsync(request);
        if (signup_response.IsSuccessStatusCode)
        {
            var signin_response = await SignIn(email, password);
            var analyticsService = new AnalyticsService();
            analyticsService?.LogEvent(FirebaseAnalytics.Event.SignUp, new Dictionary<string, string>
            {
                { "timestamp", DateTime.UtcNow.ToString() }
            });
            return signin_response;
        }
        return false;
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
            var analyticsService = new AnalyticsService();
            analyticsService?.LogEvent(FirebaseAnalytics.Event.Login , new Dictionary<string, string>
            {
                { "timestamp", DateTime.UtcNow.ToString() }
            });
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
    
    public static async Task<List<GameSession>> GetGameSessions(GameSessionSearchConfig config)
    {
        

        try
        {


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"api/gamesessions/list");
            request.Content = new StringContent(JsonSerializer.Serialize(config), null, "text/json");
            request.Headers.Add("Authorization", "Bearer " + _accessToken);
            var response = await _client.SendAsync(request);
            var str = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var sessions = JsonSerializer.Deserialize<List<GameSession>>(await response.Content.ReadAsStringAsync());
                
                return sessions;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await UpdateAccessToken();
                return await GetGameSessions(config);
            }
        }
        catch (HttpRequestException e)
        {
            throw new Exception();
        }
        

        throw new Exception();
    }
    public static async Task<List<User>> GetGameSessionParticipants(string gameSessionId)
    {
        

        try
        {


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"api/gamesessions/{gameSessionId}/users");
            request.Headers.Add("Authorization", "Bearer " + _accessToken);
            var response = await _client.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var users = JsonSerializer.Deserialize<List<User>>(await response.Content.ReadAsStringAsync());
                
                return users;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await UpdateAccessToken();
                return await GetGameSessionParticipants(gameSessionId);
            }
        }
        catch (HttpRequestException e)
        {
            throw new Exception();
        }
        

        throw new Exception();
    }
    
    public static async Task<bool> JoinGameSession(string gameSessionId)
    {
        

        try
        {


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"api/gamesessions/{gameSessionId}/join");
            request.Headers.Add("Authorization", "Bearer " + _accessToken);
            var response = await _client.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await UpdateAccessToken();
                return await JoinGameSession(gameSessionId);
            }
        }
        catch (HttpRequestException e)
        {
            throw new Exception();
        }
        

        throw new Exception();
    }

    public static async Task LogOut()
    { 
        SecureStorage.Remove("refresh_token");
        OneSignal.User.RemoveTag("UserId");
        await BlobCache.UserAccount.InvalidateAll();
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
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return false;
        }
        
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
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return await StorageService.GetProfileAsync();
        }
        
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
            await BlobCache.UserAccount.InsertObject<UserProfile>("user_profile", userProfile);
            return userProfile;
        }

        return null;
    }

    public static async Task<bool> UpdateProfile(UserProfile userProfile)
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return false;
        }
        
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
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return await StorageService.GetLanguagesAsync();
        }
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/languages/list");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        try
        {
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var languages = JsonSerializer.Deserialize<List<Language>>(content, options);
                await BlobCache.UserAccount.InsertObject<List<Language>>("preferred_languages", languages);
                return languages;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await UpdateAccessToken();
                return await GetLanguages();
            }
        }
        catch (HttpRequestException e)
        {
            return await StorageService.GetLanguagesAsync();
        }

        return null;
    }
    
   public static async Task<bool> RemoveLanguage(string ISOName)
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return false;
        }
        
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
            StorageService.CurrentUser = await GetUser();
            await App.Current.MainPage.DisplayAlert("No connection", "Your device is not connected to the internet ", "OK");
            Connectivity.ConnectivityChanged += async (sender, args) =>
            {
                if (args.NetworkAccess == NetworkAccess.Internet && WebSocketService.State != WebSocketState.Open)
                {
                    await UpdateAccessToken();
                    await WebSocketService.ConnectAsync(new Uri(new Uri(BaseAddress.Replace("http", "ws")),"ws"));
                }
            }; 
            return;
        }
        

        try
        {
            await UpdateAccessToken();
            StorageService.CurrentUser = await GetUser();
            OneSignal.User.AddTag("UserId", StorageService.CurrentUser.Id);
            await WebSocketService.ConnectAsync(new Uri(new Uri(BaseAddress.Replace("http", "ws")),"ws"));
            Firebase.Analytics.FirebaseAnalytics.GetInstance(Android.App.Application.Context)
                .SetUserProperty("user_id",StorageService.CurrentUser.Id);
        }
        catch (FailedAccessTokenRequestException e)
        {
            IsLoggedIn = false;
            return;
        }

        
        

    }
    
    public static async Task<bool> AddLanguage(string ISOName)
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return false;
        }
        
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
    
    public static async Task<List<Game>> GetGames()
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return await StorageService.GetGamesAsync();
        }
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"api/favorite-games/list");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);

        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var games = JsonSerializer.Deserialize<List<Game>>(content, options);
            await BlobCache.UserAccount.InsertObject<List<Game>>("favorite_games", games);
            return games;
        }else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            
        }
        
        return null;
    }

    public static async Task<List<Game>> SearchGames(string Name)
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return null;
        }
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"api/favorite-games/available-list");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        
        var data = new Dictionary<string, string>
        {
            { "name", Name }
        };
            
        var content = new StringContent(JsonSerializer.Serialize(data), null, "text/json");
        request.Content = content;
        
        var response = await _client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            var received = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var games = JsonSerializer.Deserialize<List<Game>>(received, options);
            return games;
        }

        return null;
    }
    
    public static async Task<bool> AddGame(Game game)
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return false;
        }
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"api/favorite-games/new");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);

        var data = new Dictionary<string, string>
        {
            { "gameId", game.GameID },
            { "image", "" },
            { "name", game.Name }
        };
            
        var content = new StringContent(JsonSerializer.Serialize(data), null, "text/json");
        request.Content = content;
        
        var response = await _client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
    
    public static async Task<bool> RemoveGame(string gameID)
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return false;
        }
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"api/favorite-games/{gameID}");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        
        var response = await _client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
    public static async Task<GameSession> CreateGame(GameSessionConfig gameSessionConfig)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"api/gamesessions/new");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(gameSessionConfig), null, "text/json");
        var response = await _client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStreamAsync();
            var analyticsService = new AnalyticsService();
           
            analyticsService?.LogEvent("game_session_created" , new Dictionary<string, string>
            {
                { "timestamp", DateTime.UtcNow.ToString() }
            });
            return JsonSerializer.Deserialize<GameSession>(content);
        }else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await UpdateAccessToken();
            return await CreateGame(gameSessionConfig);
        }

        throw new Exception();



    }
    
    public static async Task<string> StartGame(string gameId)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"api/gamesessions/{gameId}/start");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        var response = await _client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var analyticsService = new AnalyticsService();
           
            analyticsService?.LogEvent("game_session_started" , new Dictionary<string, string>
            {
                { "timestamp", DateTime.UtcNow.ToString() }
            });
            return content;
        }else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await UpdateAccessToken();
            return await StartGame(gameId);
        }

        throw new Exception();
        
    }
    
    public static async Task<bool> CancelGame(string gameId)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"api/gamesessions/{gameId}");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        var response = await _client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
           

            return true;
        }else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await UpdateAccessToken();
            return await CancelGame(gameId);
        }

        return false;

    }
    public static async Task<bool> RemoveChat(string chatId)
    {
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"api/chats/{chatId}");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        var response = await _client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
           

            return true;
        }else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await UpdateAccessToken();
            return await CancelGame(chatId);
        }

        return false;

    }
    
    
    
}