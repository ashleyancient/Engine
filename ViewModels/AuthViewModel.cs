using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Engine.Commands;
using Engine.Models;
using Engine.Services;

namespace Engine.ViewModels;

public class AuthViewModel : BaseViewModel
{
    private readonly MockDataService _data = MockDataService.Instance;

    private bool _isRegisterMode;
    private string _loginEmail = "admin@nexus.com";
    private string _loginPassword = "password123";
    private bool _rememberMe = true;
    private bool _showLoginPassword;

    private string _regFirstName = string.Empty;
    private string _regLastName = string.Empty;
    private string _regUsername = string.Empty;
    private string _regEmail = string.Empty;
    private string _regPassword = string.Empty;
    private string _regConfirm = string.Empty;
    private bool _regAcceptTerms;
    private string _regAvatarStart = "#7B61FF";
    private string _regAvatarEnd = "#00D4FF";
    private string _statusMessage = string.Empty;
    private bool _isError;

    public AuthViewModel()
    {
        LoginCommand                  = new RelayCommand(_ => DoLogin(),    _ => CanLogin());
        RegisterCommand               = new RelayCommand(_ => DoRegister(), _ => CanRegister());
        SwitchToRegisterCommand       = new RelayCommand(_ => IsRegisterMode = true);
        SwitchToLoginCommand          = new RelayCommand(_ => IsRegisterMode = false);
        TogglePasswordVisibilityCommand = new RelayCommand(_ => ShowLoginPassword = !ShowLoginPassword);
        ChangeAvatarCommand           = new RelayCommand(_ => CycleAvatar());
    }

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }
    public ICommand SwitchToRegisterCommand { get; }
    public ICommand SwitchToLoginCommand { get; }
    public ICommand TogglePasswordVisibilityCommand { get; }
    public ICommand ChangeAvatarCommand { get; }

    public event Action<User>? AuthenticationSucceeded;

    public bool IsRegisterMode
    {
        get => _isRegisterMode;
        set { if (SetProperty(ref _isRegisterMode, value)) { StatusMessage = string.Empty; IsError = false; OnPropertyChanged(nameof(IsLoginMode)); } }
    }
    public bool IsLoginMode => !IsRegisterMode;

    public string LoginEmail    { get => _loginEmail;    set => SetProperty(ref _loginEmail, value); }
    public string LoginPassword { get => _loginPassword; set => SetProperty(ref _loginPassword, value); }
    public bool RememberMe      { get => _rememberMe;    set => SetProperty(ref _rememberMe, value); }
    public bool ShowLoginPassword
    {
        get => _showLoginPassword;
        set { if (SetProperty(ref _showLoginPassword, value)) OnPropertyChanged(nameof(ShowLoginPasswordToggleText)); }
    }
    public string ShowLoginPasswordToggleText => ShowLoginPassword ? "скрыть" : "показать";

    public string RegFirstName { get => _regFirstName; set => SetProperty(ref _regFirstName, value); }
    public string RegLastName  { get => _regLastName;  set => SetProperty(ref _regLastName,  value); }
    public string RegUsername  { get => _regUsername;  set { if (SetProperty(ref _regUsername,  value)) OnPropertyChanged(nameof(UsernameError)); } }
    public string RegEmail     { get => _regEmail;     set { if (SetProperty(ref _regEmail,     value)) OnPropertyChanged(nameof(EmailError)); } }
    public string RegPassword  { get => _regPassword;  set { if (SetProperty(ref _regPassword,  value)) { OnPropertyChanged(nameof(PasswordError)); OnPropertyChanged(nameof(ConfirmError)); } } }
    public string RegConfirm   { get => _regConfirm;   set { if (SetProperty(ref _regConfirm,   value)) OnPropertyChanged(nameof(ConfirmError)); } }
    public bool RegAcceptTerms { get => _regAcceptTerms; set => SetProperty(ref _regAcceptTerms, value); }
    public string RegAvatarStart { get => _regAvatarStart; set => SetProperty(ref _regAvatarStart, value); }
    public string RegAvatarEnd   { get => _regAvatarEnd;   set => SetProperty(ref _regAvatarEnd,   value); }
    public string StatusMessage  { get => _statusMessage;  set => SetProperty(ref _statusMessage, value); }
    public bool IsError { get => _isError; set => SetProperty(ref _isError, value); }

    public string EmailError    => string.IsNullOrEmpty(RegEmail)    || IsValidEmail(RegEmail)        ? string.Empty : "Некорректный email";
    public string UsernameError => IsValidUsername(RegUsername)                                        ? string.Empty : "Только латиница и цифры (3+ символа)";
    public string PasswordError => string.IsNullOrEmpty(RegPassword) || RegPassword.Length >= 6      ? string.Empty : "Минимум 6 символов";
    public string ConfirmError  => string.IsNullOrEmpty(RegConfirm)  || RegConfirm == RegPassword    ? string.Empty : "Пароли не совпадают";

    private static bool IsValidEmail(string s) => Regex.IsMatch(s ?? string.Empty, "^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$");
    private static bool IsValidUsername(string s) { if (string.IsNullOrEmpty(s)) return true; return Regex.IsMatch(s, "^[A-Za-z0-9_]{3,}$"); }

    public bool CanLogin()    => !string.IsNullOrWhiteSpace(LoginEmail) && !string.IsNullOrWhiteSpace(LoginPassword);
    public bool CanRegister() => !string.IsNullOrWhiteSpace(RegFirstName) && !string.IsNullOrWhiteSpace(RegLastName)
        && IsValidUsername(RegUsername) && RegUsername.Length >= 3 && IsValidEmail(RegEmail)
        && RegPassword.Length >= 6 && RegPassword == RegConfirm && RegAcceptTerms && !_data.IsUsernameTaken(RegUsername);

    private void DoLogin()
    {
        if (_data.TryAuthenticate(LoginEmail.Trim(), LoginPassword, out var user) && user is not null)
        { StatusMessage = "Добро пожаловать!"; IsError = false; AuthenticationSucceeded?.Invoke(user); }
        else
        { StatusMessage = "Неверный email или пароль"; IsError = true; }
    }

    private void DoRegister()
    {
        if (_data.IsUsernameTaken(RegUsername)) { StatusMessage = "Имя пользователя уже занято"; IsError = true; return; }
        var user = new User
        {
            Id = 1000 + _data.Users.Count,
            FirstName = RegFirstName.Trim(), LastName = RegLastName.Trim(),
            Username = RegUsername.Trim(), Email = RegEmail.Trim(), Password = RegPassword,
            Bio = "Новый пользователь Engine",
            AvatarColorStart = RegAvatarStart, AvatarColorEnd = RegAvatarEnd,
            IsOnline = true, LastSeen = DateTime.Now
        };
        _data.RegisterUser(user);
        StatusMessage = "Аккаунт создан"; IsError = false;
        AuthenticationSucceeded?.Invoke(user);
    }

    private static readonly (string, string)[] _avatarPalette =
    {
        ("#7B61FF", "#00D4FF"), ("#FF6B6B", "#FFD93D"), ("#43E97B", "#38F9D7"),
        ("#F857A6", "#FF5858"), ("#5B86E5", "#36D1DC"), ("#FFB347", "#FF4757")
    };
    private int _avatarIndex;
    private void CycleAvatar()
    {
        _avatarIndex = (_avatarIndex + 1) % _avatarPalette.Length;
        var (s, e) = _avatarPalette[_avatarIndex];
        RegAvatarStart = s; RegAvatarEnd = e;
    }
}
