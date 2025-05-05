using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teammato.Controls;

public partial class MessageElement : ContentView
{
   
    public MessageElement()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public static readonly BindableProperty SenderNickNameProperty =
        BindableProperty.Create(nameof(SenderNickName), typeof(string), typeof(MessageElement), default(string));

    public string SenderNickName
    {
        get => (string)GetValue(SenderNickNameProperty);
        set => SetValue(SenderNickNameProperty, value);
    }

    public static readonly BindableProperty SenderImageProperty =
        BindableProperty.Create(nameof(SenderImage), typeof(string), typeof(MessageElement), default(string));

    public string SenderImage
    {
        get => (string)GetValue(SenderImageProperty);
        set => SetValue(SenderImageProperty, value);
    }

    public static readonly BindableProperty TextContentProperty =
        BindableProperty.Create("TextContent", typeof(string), typeof(MessageElement), default(string));

    public string TextContent
    {
        get => (string)GetValue(TextContentProperty);
        set => SetValue(TextContentProperty, value);
    }

    public static readonly BindableProperty CreatedAtProperty =
        BindableProperty.Create(nameof(CreatedAt), typeof(string), typeof(MessageElement), default(string));

    public string CreatedAt
    {
        get => (string)GetValue(CreatedAtProperty);
        set => SetValue(CreatedAtProperty, value);
    }
}