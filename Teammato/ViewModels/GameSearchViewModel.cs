using System.Collections.ObjectModel;
using System.Windows.Input;
using Teammato.Abstractions;
using Teammato.Pages;
using Teammato.Services;

namespace Teammato.ViewModels;

public class GameSearchViewModel : BaseViewModel
{
    public ObservableCollection<Game> SearchResults { get; set; } = new();
    
    public ICommand GameSelectedCommand => new Command<Game>(OnGameSelected);
    
    private void OnGameSelected(Game selectedGame)
    {
        if (selectedGame == null) return;

        if (Application.Current.MainPage.Navigation.NavigationStack.Last() is GameSearchPage page)
        {
            page.OnGameSelected(selectedGame);
        }
    }
    
    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            SearchGamesAsync(SearchText);
        }
    }
    
    private async void SearchGamesAsync(string name)
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            
            await App.Current.MainPage.DisplayAlert("No connection", "Your device is not connected to the internet ", "OK");
            
            return;
        }
        if (string.IsNullOrWhiteSpace(name))
            return;

        var games = await RestAPIService.SearchGames(name);
        SearchResults.Clear();
        if (games != null)
        {
            foreach (var game in games)
                SearchResults.Add(game);
        }
    }

    
}