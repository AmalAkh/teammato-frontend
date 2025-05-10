using System.Collections.ObjectModel;

using System.Windows.Input;
using Teammato.Abstractions;
using Teammato.Pages;
using Teammato.Services;


namespace Teammato.ViewModels;

public class SearchGameSessionViewModel : BaseViewModel
{
    private ObservableCollection<Language> _languages {
        get;
        set;
    }
    private ObservableCollection<Game> _games {
        get;
        set;
    }

    private uint _temmatesCount = 0;

    public uint TeammatesCount
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

    private double durationFrom = 1;
    
    public double DurationFrom 
    {
        get
        {
            return durationFrom ;
        }
        set
        {
            if (durationFrom  != value)
            {
                durationFrom  = value;
                OnPropertyChanged("DurationFrom");
            }
        }
        
    }
    
    private double durationTo = 2;
    
    public double DurationTo
    {
        get
        {
            return durationTo ;
        }
        set
        {
            if (durationTo  != value)
            {
                durationTo  = value;
                OnPropertyChanged("DurationTo");
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
    
    
    public  ICommand AddLanguageCommand { get; protected set; }
    public  ICommand SearchCommand { get; protected set; }
    
    public ObservableCollection<LanguageViewModel> Languages { get; set; }
    public ObservableCollection<GameViewModel> Games { get; set; }
    
    public SearchGameSessionViewModel()
    {
        _languages = App.LocalProfileViewModel.PreferredLanguages;
        _games = App.LocalProfileViewModel.FavoriteGames;
        Languages = new ObservableCollection<LanguageViewModel>();
        Games = new ObservableCollection<GameViewModel>();
        SearchCommand = new Command(StartGameSearch);
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
    
    private async void StartGameSearch()
    {
        List<string> gameIds = new List<string>();
        foreach (var game in Games)
        {
            if (game.IsSelected)
            {
                gameIds.Add(game.GameID);
            }
        }
        List<string> langs = new List<string>();
        foreach (var lang in Languages)
        {
            if (lang.IsSelected)
            {
                langs.Add(lang.ISOName);
            }
        }
        var config = new GameSessionSearchConfig()
        {
            GameIds = gameIds,
            Languages = langs,
            DurationFrom = durationFrom,
            DurationTo = DurationTo,
            PlayersCount = TeammatesCount,
            Nearest = Nearest
        };
        if (Nearest)
        {
            var location = await Geolocation.GetLocationAsync();
            config.Latitude = location.Latitude;
            config.Longitude = location.Longitude;
            

        }
        

        
        var gameSessions = await RestAPIService.GetGameSessions(config);
        if (gameSessions.Count > 0)
        {
            await Shell.Current.Navigation.PushAsync(new GamePickerPage(gameSessions));
        }
        else
        {
            await Shell.Current.DisplayAlert("No session found", "Try with different filters, please", "OK");
        }

    }
    

    
}