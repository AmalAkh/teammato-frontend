using System.Text.RegularExpressions;
using System.Windows.Input;
using Teammato.Services;

namespace Teammato.ViewModels;

public class SignUpViewModel : BaseViewModel
{
    public ICommand SignUpCommand { get; private set; }
    public ICommand SignInCommand  { get; private set; }
    
    private string _nickname;
    public string Nickname
    {
        get => _nickname;
        set
        {
            _nickname = value;
            OnPropertyChanged(nameof(Nickname));
        }
    }
    
    private string _email;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }
    
    private string _password;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }
    
    private string _repeatPassword;
    public string RepeatPassword
    {
        get => _repeatPassword;
        set
        {
            _repeatPassword = value;
            OnPropertyChanged(nameof(RepeatPassword));
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
    
    private bool _noNickname;
    public bool NoNickname
    {
        get => _noNickname;
        set
        {
            _noNickname = value;
            OnPropertyChanged(nameof(NoNickname));
        }
    }
    private bool _noEmail;
    public bool NoEmail
    {
        get => _noEmail;
        set
        {
            _noEmail = value;
            OnPropertyChanged(nameof(NoEmail));
        }
    }
    private bool _wrongEmailFormat;
    public bool WrongEmailFormat
    {
        get => _wrongEmailFormat;
        set
        {
            _wrongEmailFormat = value;
            OnPropertyChanged(nameof(WrongEmailFormat));
        }
    }
    private bool _noPassword;
    public bool NoPassword
    {
        get => _noPassword;
        set
        {
            _noPassword = value;
            OnPropertyChanged(nameof(NoPassword));
        }
    }
    private bool _passwordsDoNotMatch;
    public bool PasswordsDoNotMatch
    {
        get => _passwordsDoNotMatch;
        set
        {
            _passwordsDoNotMatch = value;
            OnPropertyChanged(nameof(PasswordsDoNotMatch));
        }
    }

    public SignUpViewModel()
    {
        SignUpCommand = new Command(async () =>
        {
            NoNickname = false;
            NoEmail = false;
            WrongEmailFormat = false;
            NoPassword = false;
            PasswordsDoNotMatch = false;
            IsAuthFailed = false;
            
            if (_nickname != null && _email != null && _password != null && _repeatPassword == _password)
            {
                IsAuthFailed = await RestAPIService.SignUp(_email, _nickname, _password);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Nickname))
                {
                    NoNickname = true;
                    IsAuthFailed = true;
                }
                if (string.IsNullOrWhiteSpace(Email))
                {
                    NoEmail = true;
                    IsAuthFailed = true;
                }
                else
                {
                    if (!IsValidEmail(Email))
                    {
                        WrongEmailFormat = true;
                        IsAuthFailed = true;
                    }
                }
                if (string.IsNullOrWhiteSpace(Password))
                {
                    NoPassword = true;
                    IsAuthFailed = true;
                }
                if (RepeatPassword != Password)
                {
                    PasswordsDoNotMatch = true;
                    IsAuthFailed = true;
                }
                return;
            }

            IsAuthFailed = !IsAuthFailed;
            if (!IsAuthFailed)
            {
                App.Current.MainPage = new AppShell();    
            }
        });

        SignInCommand = new Command(async () =>
        {
            App.Current.MainPage = new Teammato.Pages.LoginPage();
        });
    }
    
    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}