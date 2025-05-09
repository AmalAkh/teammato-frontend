using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using Teammato.Abstractions;
using Teammato.Pages;
using Teammato.Services;
using Teammato.Utils;
using System.Text;
namespace Teammato.ViewModels;

public class ChatsViewModel : BaseViewModel
{
    public ObservableCollection<ChatViewModel> Chats { private set; get; }


    public ChatViewModel SelectedChat { get; set; }
   
    public ChatsViewModel()
    {
        Chats = new ObservableCollection<ChatViewModel>();
        Task.Run(async () =>
        {
            await LoadChats();
            
        });
   
        WebSocketService.AddHandler("ChatsPageHandler",(notification) =>
        {
            if (notification.Type == WebSocketNotificationType.NewChatMessage)
            {
                var newMessage = JsonSerializer.Deserialize<Message>(notification.Content);
            
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (SelectedChat.Id == newMessage.ChatId)
                    {
                        newMessage.Sender = SelectedChat.Participants.FirstOrDefault(p => p.Id == newMessage.UserId);
                        SelectedChat.AddMessage(newMessage);
                        return;
                    }
                    foreach (var chat in Chats)
                    {
                        
                        if (chat.Id == newMessage.ChatId)
                        {
                            newMessage.Sender = chat.Participants.FirstOrDefault(p => p.Id == newMessage.UserId);
                            chat.Messages.Add(newMessage);
                            break;
                        }
                    }

                });
            }
        });
        
    }
    
    public void SortChats()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var sortedChats = Chats.OrderByDescending((chat) => chat.LastMessage?.CreatedAt).ToList();
            Chats.Clear();
            foreach (var chat in sortedChats)
            {
                Chats.Add(chat);
            }
        });
    }

    public async Task OpenNewChat()
    {
        if (!string.IsNullOrEmpty(ChatPageBus.NewChatId))
        {
            await LoadChats();
            var chat = Chats.Where((chat) => chat.Id == ChatPageBus.NewChatId).FirstOrDefault();
            ChatPageBus.NewChatId = null;
            SelectedChat = chat;
            await Shell.Current.Navigation.PushAsync(new ChatPage(chat));
            
            
        }
    }
    public async Task LoadChats()
    {
        
        var chats = await RestAPIService.GetChats();
        chats = chats.OrderByDescending((chat) => chat.LastMessage?.CreatedAt).ToList();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Chats.Clear();
            foreach (var chat in chats)
            {
                if (chat.LastMessage != null)
                {
                    chat.LastMessage.Sender = chat.Participants.FirstOrDefault(p => p.Id == chat.LastMessage.UserId);
                }
                
                Chats.Add(new ChatViewModel(this,chat));
            }
        });


    }
}