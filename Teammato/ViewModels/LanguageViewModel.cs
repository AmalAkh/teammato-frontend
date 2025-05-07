using System.ComponentModel;
using System.Runtime.CompilerServices;
using Teammato.Abstractions;

namespace Teammato.ViewModels;

public class LanguageViewModel : INotifyPropertyChanged
{
    private Language _language;
    public string ISOName
    {
        get => _language.ISOName;
        set
        {
            if (_language.ISOName != value)
            {
                _language.ISOName = value;
                OnPropertyChanged();
            }
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
    public string Name
    {
        get => _language.Name;
        set
        {
            if (_language.Name != value)
            {
                _language.Name = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public LanguageViewModel(Language language)
    {
        _language = language;
    }
}