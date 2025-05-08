using System.Collections.ObjectModel;
using Teammato.Abstractions;

namespace Teammato.ViewModels;

public class SearchGameSessionViewModel
{
    public ObservableCollection<Language> Languages {
        get;
        set;
    }
    
    public SearchGameSessionViewModel()
    {
        
    }
}