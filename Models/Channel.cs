using Engine.ViewModels;

namespace Engine.Models;

public class Channel : BaseViewModel
{
    private bool _isSubscribed;

    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Subscribers { get; set; }
    public string AvatarColorStart { get; set; } = "#7B61FF";
    public string AvatarColorEnd { get; set; } = "#00D4FF";

    public bool IsSubscribed
    {
        get => _isSubscribed;
        set
        {
            if (SetProperty(ref _isSubscribed, value))
                OnPropertyChanged(nameof(SubscribeButtonText));
        }
    }

    public string SubscribeButtonText => IsSubscribed ? "Отписаться" : "Подписаться";
    public string SubscribersText => Chat.FormatSubscribers(Subscribers) + " подписчиков";

    public string Initials
    {
        get
        {
            if (string.IsNullOrEmpty(Title)) return "?";
            var parts = Title.Split(' ');
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
            return (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)).ToUpper();
        }
    }
}
