using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teammato.Services;
using Device = Android.Health.Connect.DataTypes.Device;

namespace Teammato.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        
        InitializeComponent();
        
        if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            if (DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait)
            {
                this.Padding = new Thickness(DeviceDisplay.Current.MainDisplayInfo.Width*0.1, 0, DeviceDisplay.Current.MainDisplayInfo.Width*0.1, 0);
            }
            else
            {
                this.Padding = new Thickness(DeviceDisplay.Current.MainDisplayInfo.Width*0.15, 0, DeviceDisplay.Current.MainDisplayInfo.Width*0.15, 0);
            }
                
        }
        DeviceDisplay.MainDisplayInfoChanged += (s,e)=>
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                if (e.DisplayInfo.Orientation == DisplayOrientation.Portrait)
                {
                    this.Padding = new Thickness(DeviceDisplay.Current.MainDisplayInfo.Width*0.1, 0, DeviceDisplay.Current.MainDisplayInfo.Width*0.1, 0);
                }
                else
                {
                    this.Padding = new Thickness(DeviceDisplay.Current.MainDisplayInfo.Width*0.15, 0, DeviceDisplay.Current.MainDisplayInfo.Width*0.15, 0);
                }
                
            }
        };
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RestAPIService.CheckAuthorization();
        
        
        
        if (RestAPIService.IsLoggedIn)
        {
            App.Current.MainPage = new AppShell();
            
        }
        else
        {
            loginContainer.IsVisible = true;
        }
        
    }
}