using System.Windows.Input;
using Teammato.Services;


namespace Teammato.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private string _login;
    public string Login
    {
        get => _login;
        set
        {
            _login = value;
            OnPropertyChanged("Login");
        }
    }
    private string _password;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged("Password");
        }
    }
    
    private bool _isAuthFailed;
    public bool IsAuthFailed
    {
        get => _isAuthFailed;
        private set
        {
            _isAuthFailed= value;
            OnPropertyChanged("IsAuthFailed");
        }
    }
    

    public ICommand SignInCommand { get; private set; }
    public ICommand SignUpCommand { get; private set; }

    public LoginViewModel()
    {
        SignInCommand = new Command(async () =>
        {
            if (_login != null && _password != null)
            {
                IsAuthFailed = await RestAPIService.SignIn(_login, _password);
            }
            else
            {
                return;
            }

            IsAuthFailed = !IsAuthFailed;
            if (!IsAuthFailed)
            {
                App.Current.MainPage = new AppShell();    
            }
        });

        SignUpCommand = new Command(async () =>
        {
            App.Current.MainPage = new Teammato.Pages.SignUpPage();
        });
    }
}