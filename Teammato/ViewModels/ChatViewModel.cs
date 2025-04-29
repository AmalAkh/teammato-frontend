using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows.Input;
using Teammato.Abstractions;
using Teammato.Pages;

namespace Teammato.ViewModels;

public class ChatViewModel : BaseViewModel
{

    private Chat _chat;
    
       
    public string Name
    {
        get => _chat.Name;
        set
        {
            if (_chat.Name != value)
            {
                _chat.Name = value;
                OnPropertyChanged("Name");
            }
        }
    }

    [JsonPropertyName("id")]
    public string? Id
    {
        get => _chat.Id;
        set
        {
            if (_chat.Id != value)
            {
                _chat.Id = value;
                OnPropertyChanged("Id");
            }
        }
    }

  
    public ICommand SelectChatCommand { get; private set; }
    public ChatViewModel(Chat chat)
    {
        this._chat = chat;
        SelectChatCommand = new Command(async() =>
        {
            await Shell.Current.Navigation.PushAsync(new ChatPage(this));
        });

    }
    
}