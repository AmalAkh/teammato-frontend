using System.Windows.Input;
using Microsoft.Maui.Controls;
using Teammato.Pages;
using Teammato.Services;

namespace Teammato.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    

    public ICommand LogOutCommand { get; private set; }

    public ProfileViewModel()
    {
        LogOutCommand = new Command(async () =>
        {

            await RestAPIService.LogOut();
            App.Current.MainPage = new LoginPage();
            
            
            
        });
    }
}