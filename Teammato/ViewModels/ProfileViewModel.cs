using System.Text.Json;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Teammato.Models;
using Teammato.Pages;
using Teammato.Services;

namespace Teammato.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    public ICommand LogOutCommand { get; private set; }
    public ICommand RemoveAccountCommand { get; private set; }
    public ICommand ApplyChangesCommand { get; private set; }
    public ICommand PickProfileImageCommand { get; private set; }
    
    private const string PROFILE_KEY = "user_profile";
    
    private UserProfile _userProfile = new UserProfile();
    private UserProfile _originalUserProfile = new UserProfile();
    
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
}