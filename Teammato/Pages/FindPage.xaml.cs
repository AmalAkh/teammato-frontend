using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teammato.ViewModels;

namespace Teammato.Pages;

public partial class FindPage : ContentPage
{
    private readonly double _portraitCoef = 0.05;
    private readonly double _landscapeCoef = 0.1;
    public FindPage()
    {
        InitializeComponent();
        this.BindingContext = new SearchGameSessionViewModel();
        App.LocalProfileViewModel.LoadProfile();
        App.LocalProfileViewModel.LoadLanguages();
        App.LocalProfileViewModel.LoadFavoriteGames();
        
        if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            if (DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait)
            {
                this.Padding = new Thickness(DeviceDisplay.Current.MainDisplayInfo.Width*_portraitCoef, 0, DeviceDisplay.Current.MainDisplayInfo.Width*_portraitCoef, 0);
            }
            else
            {
                this.Padding = new Thickness(DeviceDisplay.Current.MainDisplayInfo.Width*_landscapeCoef, 0, DeviceDisplay.Current.MainDisplayInfo.Width*_landscapeCoef, 0);
            }
                
        }
        DeviceDisplay.MainDisplayInfoChanged += (s,e)=>
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                if (e.DisplayInfo.Orientation == DisplayOrientation.Portrait)
                {
                    this.Padding = new Thickness(DeviceDisplay.Current.MainDisplayInfo.Width*_portraitCoef, 0, DeviceDisplay.Current.MainDisplayInfo.Width*_portraitCoef, 0);
                }
                else
                {
                    this.Padding = new Thickness(DeviceDisplay.Current.MainDisplayInfo.Width*_landscapeCoef, 0, DeviceDisplay.Current.MainDisplayInfo.Width*_landscapeCoef, 0);
                }
                
            }
        };
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
      
        
    }

    private void CreateGameButton_OnClicked(object? sender, EventArgs e)
    {
        Shell.Current.Navigation.PushAsync(new CreateGamePage());
    }
}