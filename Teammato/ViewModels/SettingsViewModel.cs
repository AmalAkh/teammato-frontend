using System.Text.Json;
using Microsoft.Maui.Storage;

namespace Teammato.ViewModels;
using Teammato.Abstractions;

public class SettingsViewModel : BaseViewModel
{
    private const string SETTINGS_KEY = "user_settings";
    
    private UserSettings _userSettings = new UserSettings();
    
    public bool Notifications
    {
        get => _userSettings.Notifications;
        set
        {
            if (_userSettings.Notifications != value)
            {
                _userSettings.Notifications = value;
                OnPropertyChanged();
                SaveSettingsAsync();
            }
        }
    }

    public bool DarkTheme
    {
        get => _userSettings.DarkTheme;
        set
        {
            if (_userSettings.DarkTheme != value)
            {
                _userSettings.DarkTheme = value;
                OnPropertyChanged();
                SaveSettingsAsync();
            }
        }
    }

    public bool Geoposition
    {
        get => _userSettings.Geoposition;
        set
        {
            if (_userSettings.Geoposition != value)
            {
                _userSettings.Geoposition = value;
                OnPropertyChanged();
                SaveSettingsAsync();
            }
        }
    }
    
    public async Task LoadSettingsAsync()
    {
        try
        {
            var json = await SecureStorage.GetAsync(SETTINGS_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                _userSettings = JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
            }
        }
        catch
        {
            _userSettings = new UserSettings();
        }

        OnPropertyChanged(nameof(Notifications));
        OnPropertyChanged(nameof(DarkTheme));
        OnPropertyChanged(nameof(Geoposition));
    }
    
    private async void SaveSettingsAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_userSettings);
            await SecureStorage.SetAsync(SETTINGS_KEY, json);
        }
        catch (Exception)
        {
            
        }
    }
}