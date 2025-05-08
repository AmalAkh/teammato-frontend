using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teammato.Abstractions;
using Teammato.ViewModels;

namespace Teammato.Pages;

public partial class WaitingRoomPage : ContentPage
{
    public WaitingRoomPage(GameSession session)
    {
        InitializeComponent();
        this.BindingContext = new GameSessionViewModel(session);
    }
}