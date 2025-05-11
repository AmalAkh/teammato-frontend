using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teammato.ViewModels;

public partial class ChatInfoPage : ContentPage
{
    private readonly double _portraitCoef = 0.05;
    private readonly double _landscapeCoef = 0.1;
    public ChatInfoPage(ChatViewModel chatViewModel)
    {
        InitializeComponent();
        this.BindingContext = chatViewModel;
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
}