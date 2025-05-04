using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teammato.Services;

namespace Teammato.Pages;
using ViewModels;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel();
    }
    
    protected override async void OnAppearing()
    {
        await RestAPIService.CheckAuthorization();
        base.OnAppearing();
        if (BindingContext is ProfileViewModel viewModel)
        {
            await viewModel.LoadProfile();
        }
    }

    private async void OnProfileImageClicked(object sender, EventArgs e)
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
            ProfileImage.Source = ImageSource.FromStream(() => stream);
        }
        else
        {
            await DisplayAlert("Error", "Profile image could not be uploaded", "OK");
        }
    }
}