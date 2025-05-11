using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teammato.Pages;
using ViewModels;

public partial class SettingsPage : ContentPage
{
    private readonly double _portraitCoef = 0.05;
    private readonly double _landscapeCoef = 0.1;
    public SettingsPage()
    {
        InitializeComponent();
        OrientationChanged(this, EventArgs.Empty);
        DeviceDisplay.MainDisplayInfoChanged += OrientationChanged;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = App.SettingsViewModel;
        
    }
    public void OrientationChanged(object sender, System.EventArgs e)
    {
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
        
    }
}