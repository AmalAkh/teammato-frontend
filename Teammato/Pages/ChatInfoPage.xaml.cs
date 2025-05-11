using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teammato.ViewModels;

public partial class ChatInfoPage : ContentPage
{
    public ChatInfoPage(ChatViewModel chatViewModel)
    {
        InitializeComponent();
        this.BindingContext = chatViewModel;
    }
}