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
        var gameSessionConfig = new GameSessionConfig()
        {
            GameId = TargetGame.Game.GameID,
            PlayersCount = TeammatesCount,
            Duration = Duration,
            
            
        };
        foreach (var language in Languages)
        {
            if (language.IsSelected)
            {
                gameSessionConfig.Languages.Add(language.ISOName);
            }
        }

        
        if (WebSocketService.State == WebSocketState.Open)
        {
            var gameSession = await RestAPIService.CreateGame(gameSessionConfig);
            await Shell.Current.Navigation.PushAsync(new WaitingRoomPage(gameSession));
        }
    }
    
    

    
}