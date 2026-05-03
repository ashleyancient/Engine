using System;
using Engine.ViewModels;

namespace Engine.Models;

public class User : BaseViewModel
{
    private bool _isOnline;
    private DateTime _lastSeen;
    private string _bio = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _username = string.Empty;
    private string _phoneNumber = string.Empty;
    private string _avatarColorStart = "#7B61FF";
    private string _avatarColorEnd = "#00D4FF";

    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string FirstName { get => _firstName; set { if (SetProperty(ref _firstName, value)) { OnPropertyChanged(nameof(FullName)); OnPropertyChanged(nameof(Initials)); } } }
    public string LastName { get => _lastName; set { if (SetProperty(ref _lastName, value)) { OnPropertyChanged(nameof(FullName)); OnPropertyChanged(nameof(Initials)); } } }
    public string Username { get => _username; set => SetProperty(ref _username, value); }
    public string PhoneNumber { get => _phoneNumber; set => SetProperty(ref _phoneNumber, value); }
    public string AvatarColorStart { get => _avatarColorStart; set => SetProperty(ref _avatarColorStart, value); }
    public string AvatarColorEnd { get => _avatarColorEnd; set => SetProperty(ref _avatarColorEnd, value); }
    public string Bio { get => _bio; set => SetProperty(ref _bio, value); }
    public bool IsOnline { get => _isOnline; set => SetProperty(ref _isOnline, value); }
    public DateTime LastSeen { get => _lastSeen; set => SetProperty(ref _lastSeen, value); }

    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Initials
    {
        get
        {
            var first = string.IsNullOrEmpty(FirstName) ? "?" : FirstName.Substring(0, 1).ToUpper();
            var second = string.IsNullOrEmpty(LastName) ? string.Empty : LastName.Substring(0, 1).ToUpper();
            return first + second;
        }
    }
}
