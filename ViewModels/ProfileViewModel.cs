using System.Windows.Input;
using Engine.Commands;
using Engine.Models;
using Engine.Services;

namespace Engine.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    private readonly MockDataService _data = MockDataService.Instance;
    private string _firstName;
    private string _lastName;
    private string _username;
    private string _bio;
    private string _phoneNumber;
    private string _statusMessage = string.Empty;

    public ProfileViewModel()
    {
        var u = _data.CurrentUser;
        _firstName = u.FirstName; _lastName = u.LastName;
        _username = u.Username; _bio = u.Bio; _phoneNumber = u.PhoneNumber;
        SaveCommand = new RelayCommand(_ => Save());
        ChangeAvatarCommand = new RelayCommand(_ => CycleAvatar());
    }

    public ICommand SaveCommand { get; }
    public ICommand ChangeAvatarCommand { get; }
    public User CurrentUser => _data.CurrentUser;

    public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
    public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
    public string Username { get => _username; set => SetProperty(ref _username, value); }
    public string Bio { get => _bio; set { if (SetProperty(ref _bio, value)) OnPropertyChanged(nameof(BioCounter)); } }
    public string BioCounter => $"{(_bio?.Length ?? 0)} / 150";
    public string PhoneNumber { get => _phoneNumber; set => SetProperty(ref _phoneNumber, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

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
        CurrentUser.AvatarColorStart = s;
        CurrentUser.AvatarColorEnd = e;
        OnPropertyChanged(nameof(CurrentUser));
    }

    private void Save()
    {
        var u = _data.CurrentUser;
        u.FirstName = FirstName; u.LastName = LastName; u.Username = Username;
        u.Bio = Bio?.Length > 150 ? Bio.Substring(0, 150) : Bio ?? string.Empty;
        u.PhoneNumber = PhoneNumber;
        OnPropertyChanged(nameof(CurrentUser));
        StatusMessage = "Изменения сохранены";
    }
}
