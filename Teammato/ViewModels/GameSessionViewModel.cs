using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using Teammato.Abstractions;
using Teammato.Services;

namespace Teammato.ViewModels;

public class GameSessionViewModel : BaseViewModel
{
    public GameSession GameSession { get; set; }
    
    public string GameName 
    {
        get
        {
            return GameSession.GameName;
        }
        set
        {
            if (value != GameSession.GameName)
            {
                GameSession.GameName = value;
                OnPropertyChanged("GameName");
                
            }
            
            
        }
        
    }
    public ICommand CancelCommand { get; private set; }
    public ObservableCollection<User> Participants { get; set; } = new ObservableCollection<User>();

    public GameSessionViewModel(GameSession gameSession)
    {
        this.GameSession = gameSession;

        foreach (var participant in gameSession.Participants)
        {
            if (participant.Id != gameSession.Owner.Id)
            {
                Participants.Add(participant);
            }
        }
        WebSocketService.AddHandler("GameSessionWaitingRoom", (notification) =>
        {
            if (notification.Type == WebSocketNotificationType.NewPlayerJoined)
            {
                var user = JsonSerializer.Deserialize<User>(notification.Content);
                Participants.Add(user);
            }
        });

        CancelCommand = new Command(Cancel);
    }

    public async void Cancel()
    {
        
        await Shell.Current.Navigation.PopAsync();
        WebSocketService.RemoveHandler("GameSessionWaitingRoom");
    }
    
    
}