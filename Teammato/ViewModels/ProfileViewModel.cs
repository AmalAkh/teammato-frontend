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
    
    private string _originalNickname;
    private string _originalDescription;
    
    private string _nickname;
    private string _description;
    private string _imageUrl;
    
    private bool _isModified;
    public string Nickname
    {
        get => _nickname;
        set
        {
            _nickname = value;
            OnPropertyChanged("Nickname");
            CheckIfModified();
        }
    }
    
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged("Description");
            CheckIfModified();
        }
    }
    
    public bool IsModified
    {
        get => _isModified;
        set
        {
            _isModified = value;
            OnPropertyChanged(nameof(IsModified));
        }
    }

    public ProfileViewModel()
    {
        LogOutCommand = new Command(async () =>
        {
            await RestAPIService.LogOut();
            App.Current.MainPage = new LoginPage();
        });
        
        RemoveAccountCommand = new Command(async () =>
        {
            // TODO
        });
        
        ApplyChangesCommand = new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(Nickname))
            {
                await App.Current.MainPage.DisplayAlert("Validation Error", "Nickname cannot be empty.", "OK");
                Nickname = _originalNickname;
                return;
            }
            
            var success = await RestAPIService.UpdateProfile(new UserProfile
            {
                Nickname = Nickname,
                Description = Description
            });

            if (success)
            {
                _originalNickname = Nickname;
                _originalDescription = Description;
                IsModified = false;
            }
        });
    }
    
    public async Task LoadProfile()
    {
        var userProfile = await RestAPIService.GetProfile();
        if (userProfile != null)
        {
            Nickname = userProfile.Nickname;
            Description = userProfile.Description;
            
            _originalNickname = Nickname;
            _originalDescription = Description;
            IsModified = false;
        }
    }
    
    private void CheckIfModified()
    {
        IsModified = _nickname != _originalNickname || _description != _originalDescription;
    }
}