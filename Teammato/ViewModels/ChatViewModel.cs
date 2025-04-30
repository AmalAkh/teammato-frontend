using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Teammato.Abstractions;
using Teammato.Pages;
using Teammato.Services;
using Teammato.Utils;

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
    public string? Image
    {
        get => _chat.Image;
        set
        {
            if (_chat.Image != value)
            {
                _chat.Image = value;
                OnPropertyChanged("Image");
            }
        }
    }
    private string _messageText;
    public string MessageText
    {
        get => _messageText;
        set
        {
            if (_messageText != value)
            {
                _messageText = value;
                OnPropertyChanged("MessageText");
            }
        }
    }
    public ObservableCollection<Message> Messages { get;private set;  }


    public ICommand SelectChatCommand { get; private set; }
    public ICommand AddMessageCommand { get; private set; }
    public ChatViewModel(Chat chat)
    {
        this._chat = chat;
        Messages = new ObservableCollection<Message>();
        
        SelectChatCommand = new Command(async() =>
        {
            await Shell.Current.Navigation.PushAsync(new ChatPage(this));
        });
        AddMessageCommand = new Command(AddMessage);

        
    }

    

    public async Task LoadMessages(int offset = 0)
    {
        var messages = await RestAPIService.GetMessages(Id);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var msg in messages)
            {
                msg.Sender = _chat.Participants.Where((user => user.Id == msg.UserId)).Select((user) => user).First();
                Messages.Add(msg);
            }
            SimpleEventBus.Emit("ScrollToEnd");
        });
        
    }
    public async void AddMessage()
    {
        
        Messages.Add(new Message (MessageText, StorageService.CurrentUser));
        await RestAPIService.SendMessage(MessageText, Id);
        MessageText = "";
    }
    
}