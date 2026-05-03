using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Engine.Commands;
using Engine.Models;
using Engine.Services;

namespace Engine.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly NavigationService _nav = NavigationService.Instance;
    private readonly MockDataService _data = MockDataService.Instance;

    private AppSection _currentSection = AppSection.Chats;
    private Story? _activeStory;
    private string _globalSearchText = string.Empty;
    private bool _isGlobalSearchOpen;

    public ChatViewModel ChatVM { get; } = new();
    public ContactsViewModel ContactsVM { get; } = new();
    public ProfileViewModel ProfileVM { get; } = new();
    public SettingsViewModel SettingsVM { get; } = new();

    public ObservableCollection<Channel> Channels => _data.Channels;
    public ObservableCollection<Story> Stories => _data.Stories;
    public ObservableCollection<Notification> Notifications => _data.Notifications;
    public User CurrentUser => _data.CurrentUser;

    public MainViewModel()
    {
        NavigateCommand = new RelayCommand(p =>
        {
            if (p is string s && Enum.TryParse<AppSection>(s, out var sec))
                CurrentSection = sec;
        });
        ToggleStoryCommand      = new RelayCommand(p => ActiveStory = p as Story);
        CloseStoryCommand       = new RelayCommand(_ => ActiveStory = null);
        LikeStoryCommand        = new RelayCommand(_ => { if (ActiveStory is not null) ActiveStory.IsLiked = !ActiveStory.IsLiked; });
        SubscribeChannelCommand = new RelayCommand(p => { if (p is Channel c) c.IsSubscribed = !c.IsSubscribed; });
        OpenGlobalSearchCommand = new RelayCommand(_ => IsGlobalSearchOpen = true);
        CloseGlobalSearchCommand = new RelayCommand(_ => IsGlobalSearchOpen = false);
        MarkAllNotificationsCommand = new RelayCommand(_ => { foreach (var n in Notifications) n.IsRead = true; });
        OpenChatFromContactCommand = new RelayCommand(p =>
        {
            if (p is User u)
            {
                var existing = _data.Chats.FirstOrDefault(c => c.Type == ChatType.Direct && c.Counterpart?.Id == u.Id);
                if (existing is null)
                {
                    existing = new Chat
                    {
                        Id = _data.Chats.Count + 1,
                        Type = ChatType.Direct,
                        Title = u.FullName,
                        Counterpart = u,
                        AvatarColorStart = u.AvatarColorStart,
                        AvatarColorEnd = u.AvatarColorEnd
                    };
                    existing.Members.Add(u);
                    existing.Members.Add(_data.CurrentUser);
                    _data.Chats.Add(existing);
                    ChatVM.ResortAndFilter();
                }
                ChatVM.SelectedChat = existing;
                CurrentSection = AppSection.Chats;
                ContactsVM.SelectedContact = null;
            }
        });
    }

    public ICommand NavigateCommand             { get; }
    public ICommand ToggleStoryCommand          { get; }
    public ICommand CloseStoryCommand           { get; }
    public ICommand LikeStoryCommand            { get; }
    public ICommand SubscribeChannelCommand     { get; }
    public ICommand OpenGlobalSearchCommand     { get; }
    public ICommand CloseGlobalSearchCommand    { get; }
    public ICommand MarkAllNotificationsCommand { get; }
    public ICommand OpenChatFromContactCommand  { get; }

    public AppSection CurrentSection
    {
        get => _currentSection;
        set
        {
            if (SetProperty(ref _currentSection, value))
            {
                _nav.Navigate(value);
                OnPropertyChanged(nameof(SectionTitle));
                OnPropertyChanged(nameof(IsChatsActive));
                OnPropertyChanged(nameof(IsContactsActive));
                OnPropertyChanged(nameof(IsStoriesActive));
                OnPropertyChanged(nameof(IsChannelsActive));
                OnPropertyChanged(nameof(IsFavoritesActive));
                OnPropertyChanged(nameof(IsNotificationsActive));
                OnPropertyChanged(nameof(IsSettingsActive));
            }
        }
    }

    public string SectionTitle => CurrentSection switch
    {
        AppSection.Chats         => "Чаты",
        AppSection.Contacts      => "Контакты",
        AppSection.Stories       => "Истории",
        AppSection.Channels      => "Каналы",
        AppSection.Favorites     => "Избранное",
        AppSection.Notifications => "Уведомления",
        AppSection.Settings      => "Настройки",
        _ => "Engine"
    };

    public bool IsChatsActive         => CurrentSection == AppSection.Chats;
    public bool IsContactsActive      => CurrentSection == AppSection.Contacts;
    public bool IsStoriesActive       => CurrentSection == AppSection.Stories;
    public bool IsChannelsActive      => CurrentSection == AppSection.Channels;
    public bool IsFavoritesActive     => CurrentSection == AppSection.Favorites;
    public bool IsNotificationsActive => CurrentSection == AppSection.Notifications;
    public bool IsSettingsActive      => CurrentSection == AppSection.Settings;

    public int UnreadNotificationsCount => Notifications.Count(n => !n.IsRead);

    public Story? ActiveStory
    {
        get => _activeStory;
        set { if (SetProperty(ref _activeStory, value)) { if (value is not null) value.IsViewed = true; OnPropertyChanged(nameof(IsStoryOverlayVisible)); } }
    }

    public bool IsStoryOverlayVisible => ActiveStory is not null;

    public string GlobalSearchText { get => _globalSearchText; set => SetProperty(ref _globalSearchText, value); }
    public bool IsGlobalSearchOpen { get => _isGlobalSearchOpen; set => SetProperty(ref _isGlobalSearchOpen, value); }
}
