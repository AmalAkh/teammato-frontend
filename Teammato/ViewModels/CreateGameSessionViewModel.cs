using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Windows.Input;
using Teammato.Abstractions;
using Teammato.Pages;
using Teammato.Services;


namespace Teammato.ViewModels;

public class CreateGameSessionViewModel : BaseViewModel
{
    private ObservableCollection<Language> _languages {
        get;
        set;
    }
    private ObservableCollection<Game> _games {
        get;
        set;
    }

    private int _temmatesCount = 0;

    public int TeammatesCount
    {
        get { return _temmatesCount; }
        set
        {
            if (_temmatesCount != value)
            {
                _temmatesCount = value;
                OnPropertyChanged("TeammatesCount");
            }
        }
    }

    private double duration = 1;
    
    public double Duration
    {
        get
        {
            return duration;
        }
        set
        {
            if (duration != value)
            {
                duration  = value;
                OnPropertyChanged("Duration");
            }
        }
        
    }
    
    private bool  _nearest;
    public bool Nearest
    {
        get
        {
            return _nearest;
        }
        set
        {
            if (_nearest != value)
            {
                _nearest = value;
                OnPropertyChanged("Nearest");
            }
        }
        
    }
    
    private GameViewModel _targetGame;
    public GameViewModel TargetGame
    {
        get
        {
            return _targetGame;
        }
        set
        {
            if (_targetGame != value)
            {
                _targetGame = value;
                OnPropertyChanged("TargetGame");
            }
        }
        
    }
    
    public  ICommand AddLanguageCommand { get; protected set; }
    
    public ObservableCollection<LanguageViewModel> Languages { get; set; }
    public ObservableCollection<GameViewModel> Games { get; set; }
    
    public ICommand CreateGameCommand { get; protected set; }
    public CreateGameSessionViewModel()
    {
        CreateGameCommand = new Command(CreateGame);
        _languages = App.LocalProfileViewModel.PreferredLanguages;
        _games = App.LocalProfileViewModel.FavoriteGames;
        Languages = new ObservableCollection<LanguageViewModel>();
        Games = new ObservableCollection<GameViewModel>();
     
        foreach (var language in _languages)
        {
            Languages.Add(new LanguageViewModel(language));
        }
        foreach (var game in _games)
        {
            Games.Add(new GameViewModel(game));
        }
        _languages.CollectionChanged += (sender, args) =>
        {
            Languages.Clear();
            foreach (var language in _languages)
            {
                Languages.Add(new LanguageViewModel(language));
            }
        };
        _games.CollectionChanged += (sender, args) =>
        {
            Games.Clear();
            foreach (var game in _games)
            {
                Games.Add(new GameViewModel(game));
            }
        };
    }


    public async void CreateGame()
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            
            await App.Current.MainPage.DisplayAlert("No connection", "Your device is not connected to the internet ", "OK");
            
            return;
        }
        if (TargetGame == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select the game you want to play", "ok");
            return;
        }

        if (Duration <= 0)
        {
            await Shell.Current.DisplayAlert("Error", "Enter valid duration", "ok");
            return;
        }
        var gameSessionConfig = new GameSessionConfig()
        {
            GameId = TargetGame.Game.GameID,
            PlayersCount = TeammatesCount,
            Duration = Duration
            
            
        };
        
        
        foreach (var language in Languages)
        {
            if (language.IsSelected)
            {
                gameSessionConfig.Languages.Add(language.ISOName);
            }
        }
        
        if (Languages.Count == 0)
        {
            await Shell.Current.DisplayAlert("Error", "Please select at least one language", "ok");
            return;
        }
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        

        if (Nearest && status == PermissionStatus.Granted)
        {
            
            var location = await Geolocation.GetLocationAsync();

            if (location != null)
            {
                Console.WriteLine($"Latitude: {location.Latitude}");
                Console.WriteLine($"Longitude: {location.Longitude}");
                gameSessionConfig.Latitude = location.Latitude;
                gameSessionConfig.Longitude = location.Longitude;
            }
            else
            {
                Console.WriteLine("Location is null.");
            }
            
        }
        
            
        
        if (WebSocketService.State == WebSocketState.Open)
        {
            var gameSession = await RestAPIService.CreateGame(gameSessionConfig);
            await Shell.Current.Navigation.PushAsync(new WaitingRoomPage(gameSession));
        }
    }
    
    

    
}