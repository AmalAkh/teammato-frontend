using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teammato.Services;

namespace Teammato.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        
        await RestAPIService.CheckAuthorization();
        base.OnAppearing();
        if (RestAPIService.IsLoggedIn)
        {
            App.Current.MainPage = new AppShell();
        }
    }
}