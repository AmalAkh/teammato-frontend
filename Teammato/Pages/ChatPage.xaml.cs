using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teammato.ViewModels;

namespace Teammato.Pages;

public partial class ChatPage : ContentPage
{
    public ChatPage(ChatViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
        if (NavigationPage.GetTitleView(this) is View titleView)
        {
            titleView.BindingContext = viewModel;
        }
    }
}