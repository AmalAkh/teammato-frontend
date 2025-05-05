using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teammato.Abstractions;

namespace Teammato.Controls;

public partial class MessageView : ContentView
{
    public static BindableProperty MessagesProperty =
        BindableProperty.Create("Messages", typeof(ObservableCollection<Message>), typeof(MessageView));
    public MessageView()
    {
        InitializeComponent();
        
    }

    public ObservableCollection<Message> Messages
    {
        get { return (ObservableCollection<Message>)GetValue(MessagesProperty); }
        set { SetValue(MessagesProperty, value); }
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == MessagesProperty.PropertyName)
        {
            MessagesContainer.Children.Clear();
            foreach (var msg in Messages)
            {
                MessagesContainer.Children.Add(new MessageElement(){TextContent = msg.Content, CreatedAt = msg.CreatedAt.ToString(), SenderImage = msg.Sender.Image});
            }
        }
    }
}