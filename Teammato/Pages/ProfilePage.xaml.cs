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
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        this.BindingContext = App.LocalProfileViewModel;
        if (BindingContext is ProfileViewModel viewModel)
        {
            
            /*await viewModel.LoadProfile();
            await viewModel.LoadLanguages();
            await viewModel.LoadFavoriteGames();*/
        }
    }
}