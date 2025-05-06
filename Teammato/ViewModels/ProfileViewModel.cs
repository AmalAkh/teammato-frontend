using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using System.Globalization;
using Microsoft.Maui.Controls;
using Teammato.Abstractions;
using Teammato.Pages;
using Teammato.Services;

namespace Teammato.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    public ICommand LogOutCommand { get; private set; }
    public ICommand RemoveAccountCommand { get; private set; }
    public ICommand ApplyChangesCommand { get; private set; }
    public ICommand PickProfileImageCommand { get; private set; }
    
    public ICommand AddLanguageCommand { get; }
    public ICommand RemoveLanguageCommand { get; }
    
    public ICommand AddFavoriteGameCommand { get; }
    public ICommand RemoveFavoriteGameCommand { get; }
    
    private const string PROFILE_KEY = "user_profile";
    
    private UserProfile _userProfile = new UserProfile();
    private UserProfile _originalUserProfile = new UserProfile();
    
    public ObservableCollection<Language> PreferredLanguages { get; set; } = new();
    public ObservableCollection<Game> FavoriteGames { get; set; } = new();
    
    private bool _isModified;
    public bool IsModified
    {
        get => _isModified;
        set
        {
            _isModified = value;
            OnPropertyChanged(nameof(IsModified));
        }
    }
    
    public string Nickname
    {
        get => _userProfile.Nickname;
        set
        {
            _userProfile.Nickname = value;
            OnPropertyChanged();
            CheckIfModified();
        }
    }
    
    public string Description
    {
        get => _userProfile.Description;
        set
        {
            _userProfile.Description = value;
            OnPropertyChanged();
            CheckIfModified();
        }
    }

    public ProfileViewModel()
    {
        PickProfileImageCommand = new Command(async () => await PickAndUploadImage());
        
        LogOutCommand = new Command(async () => await LogOut());
        
        RemoveAccountCommand = new Command(async () => RemoveAccount());
        
        ApplyChangesCommand = new Command(async () => ApplyChanges());
        
        AddLanguageCommand = new Command(async () => await AddLanguage());
        RemoveLanguageCommand = new Command<string>(RemoveLanguage);

        AddFavoriteGameCommand = new Command(async () => await AddFavoriteGame());
        RemoveFavoriteGameCommand = new Command<string>(RemoveFavoriteGame);
    }

    private async Task LogOut()
    {
        await RestAPIService.LogOut();
        App.Current.MainPage = new LoginPage();
    }

    private async Task RemoveAccount()
    {
        // TODO
    }

    private async Task ApplyChanges()
    {
        if (string.IsNullOrWhiteSpace(Nickname))
        {
            await App.Current.MainPage.DisplayAlert("Validation Error", "Nickname cannot be empty.", "OK");
            Nickname = _originalUserProfile.Nickname;
            return;
        }
            
        var success = await RestAPIService.UpdateProfile(_userProfile);

        if (success)
        {
            _originalUserProfile = new UserProfile
            {
                Nickname = Nickname,
                Description = Description
            };
            
            await SaveProfileAsync();
            IsModified = false;
        }
    }

    private async Task PickAndUploadImage()
    {
        var customImageFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { "public.image" } }, // iOS UTType
            { DevicePlatform.Android, new[] { "image/jpeg", "image/png" } }, // MIME types
            { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png" } }, // file extensions
            { DevicePlatform.MacCatalyst, new[] { "public.jpeg", "public.png" } }
        });
        
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Select Profile Image Please",
            FileTypes = customImageFileType
        });
        
        if (result == null)
            return;

        var stream = await result.OpenReadAsync();
        var success = await RestAPIService.UploadProfileImage(stream, result.FileName);

        if (success)
        {
            // TODO
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Error", "Profile image could not be uploaded", "OK");
        }
    }
    
    private async Task SaveProfileAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_userProfile);
            await SecureStorage.SetAsync(PROFILE_KEY, json);
        }
        catch
        {

        }
    }
    
    public async Task LoadLanguages()
    {
        var languages = await RestAPIService.GetLanguages();
        if (languages != null)
        {
            PreferredLanguages.Clear();
            foreach (var language in languages)
            {
                try
                {
                    var culture = new CultureInfo(language.ISOName);
                    language.Name = culture.EnglishName;
                }
                catch (CultureNotFoundException)
                {
                    language.Name = language.ISOName;
                }
                PreferredLanguages.Add(language);
            }
        }
    }
    
    public async Task LoadProfile()
    {
        try
        {
            var json = await SecureStorage.GetAsync(PROFILE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                _userProfile = JsonSerializer.Deserialize<UserProfile>(json) ?? new UserProfile();
                _originalUserProfile = JsonSerializer.Deserialize<UserProfile>(json) ?? new UserProfile();
            }
        }
        catch
        {
            _userProfile = new UserProfile();
            _originalUserProfile = new UserProfile();
        }

        OnPropertyChanged(nameof(Nickname));
        OnPropertyChanged(nameof(Description));
        CheckIfModified();

        // Server data
        try
        {
            var serverUserProfile = await RestAPIService.GetProfile();
            if (serverUserProfile != null)
            {
                _userProfile = serverUserProfile;
                _originalUserProfile = new UserProfile
                {
                    Nickname = serverUserProfile.Nickname,
                    Description = serverUserProfile.Description
                };

                await SaveProfileAsync();

                OnPropertyChanged(nameof(Nickname));
                OnPropertyChanged(nameof(Description));
                CheckIfModified();
            }
        }
        catch
        {
            
        }
    }
    
    private void CheckIfModified()
    {
        IsModified = _userProfile.Nickname != _originalUserProfile.Nickname ||
                     _userProfile.Description != _originalUserProfile.Description;
    }
    
    public List<Language> GetAllLanguages()
    {
        List<Language> languages = new List<Language>();
        
        foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
        {
            languages.Add(new Language
            {
                Name = culture.EnglishName,
                ISOName = culture.TwoLetterISOLanguageName
            });
        }

        return languages;
    }
    
    private async Task AddLanguage()
    {
        var allLanguages = GetAllLanguages();
        string[] names = allLanguages.Select(l => l.Name).ToArray();
        
        var selected = await Application.Current.MainPage.DisplayActionSheet(
            "Select Language", "Cancel", null, names);
        
        if (selected != null && selected != "Cancel")
        {
            var selectedLanguage = allLanguages.FirstOrDefault(l => l.Name == selected);
            var newLanguage = new Language
            {
                Name = selectedLanguage.Name,
                ISOName = selectedLanguage.ISOName
            };
            
            var success = await RestAPIService.AddLanguage(newLanguage.ISOName);
        
            if (success)
            {
                PreferredLanguages.Add(newLanguage);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Failed to add language", "OK");
            }
        }
    }
    
    private async void RemoveLanguage(string ISOName)
    {
        var languageToRemove = PreferredLanguages.FirstOrDefault(l => l.ISOName == ISOName);
        if (languageToRemove != null)
        {
            var success = await RestAPIService.RemoveLanguage(ISOName);
            if (success)
            {
                PreferredLanguages.Remove(languageToRemove);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Failed to remove language", "OK");
            }
        }
    }
    
    public async Task LoadFavoriteGames()
    {
        var games = await RestAPIService.GetGames();
        if (games != null)
        {
            FavoriteGames.Clear();
            foreach (var game in games)
            {
                FavoriteGames.Add(game);
            }
        }
    }
    
    private async void OnGameSelected(object sender, Game selectedGame)
    {
        if (selectedGame != null && !FavoriteGames.Any(g => g.GameID == selectedGame.GameID))
        {
            var success = await RestAPIService.AddGame(selectedGame);
            if (success)
            {
                FavoriteGames.Add(selectedGame);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Failed to add game", "OK");
            }
        }
    }

    private async Task AddFavoriteGame()
    {
        var gameSearchPage = new GameSearchPage();
        gameSearchPage.GameSelected += OnGameSelected;
        await Shell.Current.Navigation.PushAsync(gameSearchPage);
    }
    
    private async void RemoveFavoriteGame(string gameID)
    {
        var gameToRemove = FavoriteGames.FirstOrDefault(g => g.GameID == gameID);
        if (gameToRemove != null)
        {
            var success = await RestAPIService.RemoveGame(gameID);
            if (success)
            {
                FavoriteGames.Remove(gameToRemove);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Failed to remove favorite game", "OK");
            }
        }
    }
}