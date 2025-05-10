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
                if (!_userSettings.UseSystemTheme)
                {
                    if (_userSettings.DarkTheme)
                    {
                        App.Current.UserAppTheme = AppTheme.Dark;
                    }
                    else
                    {
                        App.Current.UserAppTheme = AppTheme.Light;
                    }
                }

                OnPropertyChanged();
                SaveSettingsAsync();
            }
        }
    }

    public bool UseSystemTheme
    {
        get => _userSettings.UseSystemTheme;
        set
        {
            if (_userSettings.UseSystemTheme != value)
            {
                _userSettings.UseSystemTheme = value;
                if (_userSettings.UseSystemTheme)
                {
                    App.Current.RequestedThemeChanged +=

                    (s, e) =>
                    {
                        App.Current.UserAppTheme = e.RequestedTheme;
                        if (e.RequestedTheme == AppTheme.Light)
                        {
                            _userSettings.DarkTheme = false;
                        }
                        else
                        {
                            _userSettings.DarkTheme = true;
                        }
                    };
                }
                else
                {
                    App.Current.RequestedThemeChanged += (s, e) =>
                    {
                    };
                }
                
                
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
    public bool UseBigFonts
    {
        get => _userSettings.UseBigFonts;
        set
        {
            if (_userSettings.UseBigFonts != value)
            {
                _userSettings.UseBigFonts = value;
                OnPropertyChanged();
                ApplyBigFonts(!value);
                
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
        OnPropertyChanged(nameof(UseSystemTheme));
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

    public SettingsViewModel()
    {
        Task.Run(async () =>
        {
            await LoadSettingsAsync();
            
            ApplyBigFonts(!UseBigFonts);
            
        });

    }

    public void ApplyBigFonts(bool revert = false)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (revert)
            {
                Application.Current.Resources["BasicFontSize"] = 14;
                Application.Current.Resources["TitleFontSize"] = 18;
            }
            else
            {
                Application.Current.Resources["BasicFontSize"] = 26;
                Application.Current.Resources["TitleFontSize"] = 30;
            }
            
        });
      
    }
}