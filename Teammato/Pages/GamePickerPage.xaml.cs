using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teammato.Abstractions;
using Teammato.ViewModels;

namespace Teammato.Pages;

public partial class GamePickerPage : ContentPage
{
    public GamePickerPage(List<GameSession> gameSessions)
    {
        InitializeComponent();
        this.BindingContext = new GamePickerViewModel(gameSessions);
        
    }
}