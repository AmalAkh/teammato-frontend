using Teammato.Abstractions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Teammato.Abstractions;

namespace Teammato.ViewModels;

public class GameViewModel : INotifyPropertyChanged
{
    private Game _game { get; set; }

    public string Name
    {
        get
        {
            return _game.Name;
        }
        set
        {
            _game.Name = value;
        }
    }
    public string GameID
    {
        get
        {
            return _game.GameID;
        }
        set
        {
            _game.GameID = value;
        }
    }
    
    public bool isSelected = false;
    public bool IsSelected
    {
        get
        {
            return isSelected;
        }
        set
        {
            if (isSelected != value)
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public  GameViewModel(Game game)
    {
        _game = game;
    }
}