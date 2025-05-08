using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Teammato.Utils;
using Teammato.ViewModels;

namespace Teammato.Pages;

public partial class ChatsPage : ContentPage
{
    public ChatsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await (this.BindingContext as ChatsViewModel).OpenNewChat();
    }
}