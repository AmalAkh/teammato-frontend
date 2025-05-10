using System.Collections.ObjectModel;
using System.Windows.Input;
using Teammato.Abstractions;

namespace Teammato.ViewModels;

public class GamePickerViewModel: BaseViewModel
{
    public ObservableCollection<GameSession> GameSessions
    {
        get; set;
    }
    private GameSession _selectedGameSession;
    public GameSession SelectedGameSession
    {
        get
        {
            return _selectedGameSession;
        }
        set
        {
            if (_selectedGameSession != value)
            {
                _selectedGameSession = value;
                OnPropertyChanged(nameof(SelectedGameSession));
            }
        }
    }
    public ICommand NextCommand { get; private set; }
    public ICommand BackCommand { get; private set; }
    public GamePickerViewModel(List<GameSession> gamesSessions)
    {
        GameSessions = new ObservableCollection<GameSession>();
        foreach (var game in gamesSessions)
        {
            GameSessions.Add(game);
        }

        SelectedGameSession = GameSessions[0];
        NextCommand = new Command(Next);
        BackCommand = new Command(Back);


    }
    private int _selectedGameSessionIndex = 0;
    public void Next()
    {
        if (_selectedGameSessionIndex+1 < GameSessions.Count)
        {
            
            _selectedGameSessionIndex++;
            SelectedGameSession = GameSessions[_selectedGameSessionIndex];
        }
    }
    public void Back()
    {
        if (_selectedGameSessionIndex-1 >= 0)
        {
            
            _selectedGameSessionIndex--;
            SelectedGameSession = GameSessions[_selectedGameSessionIndex];
        }
    }
}