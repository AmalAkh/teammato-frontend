using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teammato.ViewModels;

namespace Teammato.Pages;

public partial class FindPage : ContentPage
{
    
    public FindPage()
    {
        InitializeComponent();
        this.BindingContext = new SearchGameSessionViewModel();
        App.LocalProfileViewModel.LoadProfile();
        App.LocalProfileViewModel.LoadLanguages();
        App.LocalProfileViewModel.LoadFavoriteGames();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
      
        
    }
}