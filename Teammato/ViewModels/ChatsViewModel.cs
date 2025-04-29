using System.Collections.ObjectModel;
using System.Windows.Input;
using Teammato.Abstractions;
using Teammato.Pages;
using Teammato.Services;

namespace Teammato.ViewModels;

public class ChatsViewModel : BaseViewModel
{
    private ObservableCollection<ChatViewModel> _chats;
    public ObservableCollection<ChatViewModel> Chats
    {
        get
        {
            return _chats;
        }
        set
        {
            if (value != null)
            {
                _chats = value;
                OnPropertyChanged("Chats");
            }
        }
    }

    private Chat _selectedChat;
    public Chat SelectedChat {
        get
        {
            return _selectedChat;
        }
        set
        {
            
            _selectedChat = value;
            OnPropertyChanged("SelectedChat");
            
        } 
    }
    public ICommand SelectChatCommand { get; private set; }
    public ChatsViewModel()
    {
        Chats = new ObservableCollection<ChatViewModel>();
        LoadChats();
   
        SelectChatCommand = new Command(async (obj) =>
        {
            await (App.Current.MainPage as AppShell).Navigation.PushAsync(new ChatPage(new ChatViewModel(_selectedChat as Chat)));
            
        });
    }

    public async Task LoadChats()
    {
        var chats = await RestAPIService.GetChats();
        foreach (var chat in chats)
        {
            Chats.Add(new ChatViewModel(chat));
        }
    }
}