using System.Collections.ObjectModel;
using System.Windows.Input;
using Teammato.Abstractions;


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
    
    public ObservableCollection<LanguageViewModel> Languages { get; set; }
    public ObservableCollection<GameViewModel> Games { get; set; }
    
    public SearchGameSessionViewModel()
    {
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
    
    
    

    
}