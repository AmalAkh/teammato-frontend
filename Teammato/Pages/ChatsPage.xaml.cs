using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Locations;
using Teammato.Utils;
using Teammato.ViewModels;

namespace Teammato.Pages;

public partial class ChatsPage : ContentPage
{
    private readonly double _portraitCoef = 0.05;
    private readonly double _landscapeCoef = 0.1;
    public ChatsPage()
    {
        InitializeComponent();
        OrientationChanged(this, System.EventArgs.Empty);
        DeviceDisplay.MainDisplayInfoChanged += OrientationChanged;
        MessageCollectionView.SizeChanged += (s, e) =>
        {
            if ( MessageCollectionView.Height > 0)
            {
                TopMessangerSpace.HeightRequest = MessageCollectionView.Height * 0.9;
            }
        };
    }

    public void OrientationChanged(Object sender, System.EventArgs e)
    {
        if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            if (DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait)
            {
                Grid.SetColumnSpan(ChatsView, 3);
                MessangerContainer.IsVisible = false;
                this.Padding = new Thickness(DeviceDisplay.Current.MainDisplayInfo.Width*_portraitCoef, 0, DeviceDisplay.Current.MainDisplayInfo.Width*_portraitCoef, 0);
            }
            else
            {
                Grid.SetColumnSpan(ChatsView, 1);
                MessangerContainer.IsVisible = true;
                this.Padding = new Thickness(0, 0, 0, 0);
                
            }
                
        }
        else
        {
            Grid.SetColumnSpan(ChatsView, 3);
            MessangerContainer.IsVisible = false;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await (this.BindingContext as ChatsViewModel).OpenNewChat();
    }
}