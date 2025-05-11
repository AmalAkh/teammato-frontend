using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teammato.Abstractions;

namespace Teammato.Pages;
using ViewModels;

public partial class GameSearchPage : ContentPage
{
    public event EventHandler<Game> GameSelected;
    public GameSearchPage()
    {
        InitializeComponent();
    }
    
    public void OnGameSelected(Game game)
    {
        GameSelected?.Invoke(this, game);
        Navigation.PopAsync();
    }
}