using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using Teammato.Abstractions;
using Teammato.Services;
using Teammato.Utils;

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
    public int RequiredPlayersCount
    {
        get
        {
            return GameSession.RequiredPlayersCount;
        }
        set
        {
            if (value != GameSession.RequiredPlayersCount)
            {
                GameSession.RequiredPlayersCount = value;
                OnPropertyChanged("RequiredPlayersCount");
                
            }
            
        }
        
    }
    public int Duration
    {
        get
        {
            return GameSession.Duration;
        }
        set
        {
            if (value != GameSession.Duration)
            {
                GameSession.Duration = value;
                OnPropertyChanged("Duration");
                
            }
            
        }
        
    }

    public string Image
    {
        get
        {
            return GameSession.Image;   
        }
        set
        {
            if (value != GameSession.Image)
            {
                GameSession.GameName = value;
                OnPropertyChanged("GameName");
                
            }
            
        }
    }
    public ICommand CancelCommand { get; private set; }

    public ICommand StartGameCommand { get; private set; }
    public ObservableCollection<User> Participants { get; set; } = new ObservableCollection<User>();

    private bool _isOwned = false;
    public bool IsOwned
    {
        get
        {
            return _isOwned;
        }
        set
        {
            if (value != _isOwned)
            {
                _isOwned = value;
                OnPropertyChanged(nameof(IsOwned));
                
            }
        }
    }
    public GameSessionViewModel(GameSession gameSession)
    {
        this.GameSession = gameSession;
        if (gameSession.Owner.Id == StorageService.CurrentUser.Id)
        {
            IsOwned = true;
        }

        if (!IsOwned)
        {
            Task.Run(async () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    GameSession.Participants = await RestAPIService.GetGameSessionParticipants(gameSession.Id);
                    foreach (var participant in gameSession.Participants)
                    {
                        
                        Participants.Add(participant);
                        
                    }
                });
            });
        }
        else
        {
            foreach (var participant in gameSession.Participants)
            {
                if (participant.Id != gameSession.Owner.Id)
                {
                    Participants.Add(participant);
                }
            }
        }

        WebSocketService.AddHandler("GameSessionWaitingRoom", async (notification) =>
        {
            if (notification.Type == WebSocketNotificationType.NewPlayerJoined)
            {
                var user = JsonSerializer.Deserialize<User>(notification.Content);
                Participants.Add(user);
            }else if (notification.Type == WebSocketNotificationType.PlayerLeavedGameSession)
            {
                var user = JsonSerializer.Deserialize<User>(notification.Content);
                Participants.Remove(user);
            }else if (notification.Type == WebSocketNotificationType.GameSessionCancelled)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    WebSocketService.RemoveHandler("GameSessionWaitingRoom");
                    await Shell.Current.DisplayAlert("Game session cancelled", "Game session cancelled", "OK");
                    await Shell.Current.Navigation.PopAsync();
                    await Shell.Current.Navigation.PopAsync();
                });
            }else if (notification.Type == WebSocketNotificationType.GameSessionStarted)
            {
                
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    WebSocketService.RemoveHandler("GameSessionWaitingRoom");
                    Dictionary<string, string> data =
                        JsonSerializer.Deserialize<Dictionary<string, string>>(notification.Content);
                    await Shell.Current.Navigation.PopAsync();
                    await Shell.Current.Navigation.PopAsync();
                    ChatPageBus.NewChatId = data["chatId"];
                    await Shell.Current.GoToAsync("//chats");
                });

            }
        });

        CancelCommand = new Command(Cancel);
        StartGameCommand = new Command(StartGame);
    }
    
    public async void Cancel()
    {
        if (IsOwned)
        {
            await RestAPIService.CancelGame(GameSession.Id);
        }
        WebSocketService.RemoveHandler("GameSessionWaitingRoom");
        await Shell.Current.Navigation.PopAsync();
        
    }

    public async void StartGame()
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            
            await App.Current.MainPage.DisplayAlert("No connection", "Your device is not connected to the internet ", "OK");
            
            return;
        }
        var chatId = await RestAPIService.StartGame(GameSession.Id);
        await Shell.Current.Navigation.PopAsync();
        await Shell.Current.Navigation.PopAsync();
        ChatPageBus.NewChatId = chatId;
        WebSocketService.RemoveHandler("GameSessionWaitingRoom");
        await Shell.Current.GoToAsync("//chats");
    }
    
    
}