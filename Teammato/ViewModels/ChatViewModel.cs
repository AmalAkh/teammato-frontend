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
using System.Linq;

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
    
    public Message LastMessage
    {
        get => _chat.LastMessage;
        set
        {
            if (_chat.LastMessage != value)
            {
                _chat.LastMessage = value;
                OnPropertyChanged("LastMessage");
            }
        }
    }

    private bool _isOwned = false;
    public bool IsOwned
    {
        get => _isOwned;
        set
        {
            if (_isOwned != value)
            {
                _isOwned = value;
                OnPropertyChanged("IsOwned");
            }
        }
    }
    public ObservableCollection<Message> Messages { get;private set;  }
    public ObservableCollection<User> Participants { get;private set;  }
    private ChatsViewModel _chatsViewModel;

    public ICommand SelectChatCommand { get; private set; }
    public ICommand AddMessageCommand { get; private set; }
    public ICommand GoBackCommand { get; private set; }
    public ICommand OpenChatInfoCommand { get; private set; }
    public ICommand RemoveChatCommand { get; private set; }
    public ChatViewModel(ChatsViewModel chatsViewModel,Chat chat)
    {
        this._chat = chat;
        this._chatsViewModel = chatsViewModel;
        Participants = new ObservableCollection<User>(chat.Participants);
        Messages = new ObservableCollection<Message>();
        IsOwned = chat.Owner.Id == StorageService.CurrentUser.Id;
        
        SelectChatCommand = new Command(async() =>
        {
            _chatsViewModel.SelectedChat = this;
            if (DeviceInfo.Idiom != DeviceIdiom.Tablet ||(DeviceInfo.Idiom == DeviceIdiom.Tablet && DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait))
            {
                await Shell.Current.Navigation.PushAsync(new ChatPage(this));    
            }
            else
            {
                Task.Run(async()=>
                { 
                    await LoadMessages();
                });
            }
            
            
        });
        AddMessageCommand = new Command(SendMessage);
        OpenChatInfoCommand = new Command(OpenChatInfo);
        GoBackCommand = new Command(GoBack);
        RemoveChatCommand = new Command(RemoveChat);
    }

    public void AddMessage(Message message)
    {
        Messages.Add(message);
        LastMessage = message;

    }
    public async Task LoadMessages(int offset = 0)
    {
        
        var messages = await RestAPIService.GetMessages(Id);
        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Messages.Clear();
            foreach (var msg in messages)
            {
                msg.Sender = _chat.Participants.Where((user => user.Id == msg.UserId)).Select((user) => user).First();
                Messages.Add(msg);
            }
            SimpleEventBus.Emit("ScrollToEnd");
            if (messages.Count > 0)
            {
                LastMessage = messages[messages.Count - 1];    
            }
            
        });
        
    }
    
    public async void SendMessage()
    {
        
        //Messages.Add(new Message (MessageText, StorageService.CurrentUser));
        if (await RestAPIService.SendMessage(MessageText, Id))
        {
            UnfocusEntryRequested?.Invoke();
            MessageText = "";
        }
        else
        {
            await Shell.Current.DisplayAlert("Unable to send message", "", "ok");
        }
        
    }

    public async void OpenChatInfo()
    {
        await Shell.Current.Navigation.PushAsync(new ChatInfoPage(this));
    }
    public async void RemoveChat()
    {
        await RestAPIService.RemoveChat(this.Id);
        await _chatsViewModel.LoadChats();
        await Shell.Current.Navigation.PopAsync();
        await Shell.Current.Navigation.PopAsync();

        
    }
    public event Action UnfocusEntryRequested;

    public async void GoBack()
    {
        
        _chatsViewModel.SortChats();
        await Shell.Current.Navigation.PopAsync();
    }
    
}