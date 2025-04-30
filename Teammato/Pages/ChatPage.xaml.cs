using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teammato.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.DependencyInjection;
using Teammato.Utils;

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
        
    
        LoadData();
    }

    public async void LoadData()
    {
        
    
        await (this.BindingContext as ChatViewModel).LoadMessages();
        ScrollToEnd();
    }
    
    public void ScrollToEnd()
    {
        var messages = (BindingContext as ChatViewModel)?.Messages;
        if (messages != null && messages.Count > 0)
        {
            MessageCollectionView.ScrollTo(messages.Count - 1, position: ScrollToPosition.End);
        }
    }

    private void MessageTextEntry_OnFocused(object? sender, FocusEventArgs e)
    {
        //MessageCollectionView.Margin = new Thickness(0, this.Height / 2,0,0);
    }
}