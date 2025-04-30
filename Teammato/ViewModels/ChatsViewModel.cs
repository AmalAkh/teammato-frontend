using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using Teammato.Abstractions;
using Teammato.Pages;
using Teammato.Services;

namespace Teammato.ViewModels;

public class ChatsViewModel : BaseViewModel
{
    public ObservableCollection<ChatViewModel> Chats { private set; get; }


   
   
    public ChatsViewModel()
    {
        Chats = new ObservableCollection<ChatViewModel>();
        Task.Run(async () => await LoadChats());
   
        
    }

    public async Task LoadChats()
    {
        var chats = await RestAPIService.GetChats();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var chat in chats)
            {
                Chats.Add(new ChatViewModel(chat));
            }
        });


    }
}