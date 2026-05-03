using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Engine.Commands;
using Engine.Models;
using Engine.Services;

namespace Engine.ViewModels;

public class ChatViewModel : BaseViewModel
{
    private readonly MockDataService _data = MockDataService.Instance;
    private readonly DispatcherTimer _typingTimer;

    private Chat? _selectedChat;
    private string _searchText = string.Empty;
    private string _draftText = string.Empty;
    private string _filter = "all";
    private Message? _replyTo;

    public ObservableCollection<Chat> AllChats => _data.Chats;
    public ObservableCollection<Chat> FilteredChats { get; } = new();

    public ChatViewModel()
    {
        SelectChatCommand    = new RelayCommand(p => { if (p is Chat c) SelectedChat = c; });
        SendMessageCommand   = new RelayCommand(_ => SendMessage(), _ => CanSend());
        TogglePinCommand     = new RelayCommand(p => { if (p is Chat c) { c.IsPinned = !c.IsPinned; ResortAndFilter(); } });
        ToggleMuteCommand    = new RelayCommand(p => { if (p is Chat c) c.IsMuted = !c.IsMuted; });
        ArchiveCommand       = new RelayCommand(p => { if (p is Chat c) c.IsArchived = !c.IsArchived; });
        DeleteChatCommand    = new RelayCommand(p => { if (p is Chat c) { _data.Chats.Remove(c); ResortAndFilter(); } });
        StartReplyCommand    = new RelayCommand(p => ReplyTo = p as Message);
        CancelReplyCommand   = new RelayCommand(_ => ReplyTo = null);
        SetFilterCommand     = new RelayCommand(p => { Filter = p?.ToString() ?? "all"; });
        InsertEmojiCommand   = new RelayCommand(p => { if (p is string s) DraftText += s; });

        ApplyFilter();
        if (FilteredChats.Count > 0) SelectedChat = FilteredChats[0];

        _typingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        _typingTimer.Tick += (_, _) => { if (_selectedChat is not null) _selectedChat.IsTyping = false; _typingTimer.Stop(); };
    }

    public ICommand SelectChatCommand  { get; }
    public ICommand SendMessageCommand { get; }
    public ICommand TogglePinCommand   { get; }
    public ICommand ToggleMuteCommand  { get; }
    public ICommand ArchiveCommand     { get; }
    public ICommand DeleteChatCommand  { get; }
    public ICommand StartReplyCommand  { get; }
    public ICommand CancelReplyCommand { get; }
    public ICommand SetFilterCommand   { get; }
    public ICommand InsertEmojiCommand { get; }

    public Chat? SelectedChat
    {
        get => _selectedChat;
        set
        {
            if (SetProperty(ref _selectedChat, value))
            {
                if (value is not null) value.UnreadCount = 0;
                ReplyTo = null;
                OnPropertyChanged(nameof(HasSelectedChat));
                OnPropertyChanged(nameof(SelectedChatMessages));
            }
        }
    }

    public bool HasSelectedChat => SelectedChat is not null;
    public ObservableCollection<Message> SelectedChatMessages => SelectedChat?.Messages ?? new ObservableCollection<Message>();

    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }

    public string Filter
    {
        get => _filter;
        set
        {
            if (SetProperty(ref _filter, value))
            {
                ApplyFilter();
                OnPropertyChanged(nameof(IsFilterAll));
                OnPropertyChanged(nameof(IsFilterUnread));
                OnPropertyChanged(nameof(IsFilterGroups));
                OnPropertyChanged(nameof(IsFilterChannels));
            }
        }
    }

    public bool IsFilterAll      => Filter == "all";
    public bool IsFilterUnread   => Filter == "unread";
    public bool IsFilterGroups   => Filter == "groups";
    public bool IsFilterChannels => Filter == "channels";

    public string DraftText
    {
        get => _draftText;
        set { if (SetProperty(ref _draftText, value)) { OnPropertyChanged(nameof(HasDraft)); OnPropertyChanged(nameof(SendButtonGlyph)); } }
    }

    public bool HasDraft => !string.IsNullOrWhiteSpace(DraftText);
    public string SendButtonGlyph => HasDraft ? "send" : "mic";

    public Message? ReplyTo
    {
        get => _replyTo;
        set { if (SetProperty(ref _replyTo, value)) OnPropertyChanged(nameof(HasReply)); }
    }

    public bool HasReply => ReplyTo is not null;

    private bool CanSend() => HasDraft && SelectedChat is not null;

    public void SendMessage()
    {
        if (!CanSend() || SelectedChat is null) return;
        var msg = new Message
        {
            Id = SelectedChat.Messages.Count + 1,
            ChatId = SelectedChat.Id,
            Sender = _data.CurrentUser,
            IsOutgoing = true,
            Text = DraftText.Trim(),
            Timestamp = DateTime.Now,
            Status = MessageStatus.Sent,
            Type = MessageType.Text,
            ReplyTo = ReplyTo
        };
        SelectedChat.Messages.Add(msg);
        SelectedChat.RefreshPreview();
        DraftText = string.Empty;
        ReplyTo = null;
        SimulateReply(SelectedChat);
        ResortAndFilter();
    }

    private void SimulateReply(Chat chat)
    {
        if (chat.Type == ChatType.Channel || chat.Type == ChatType.Saved) return;
        chat.IsTyping = true;
        _typingTimer.Stop();
        _typingTimer.Start();

        var partner = chat.Counterpart ?? chat.Members.FirstOrDefault(u => u.Id != _data.CurrentUser.Id);
        if (partner is null) return;

        var responses = new[]
        {
            "Хорошо, понял.", "Спасибо за информацию.", "Согласен с тобой.",
            "Перезвоню чуть позже.", "Отлично, договорились.", "Принято.",
            "Сейчас посмотрю.", "Обсудим завтра."
        };
        var rng = new Random();
        var delay = TimeSpan.FromSeconds(rng.Next(2, 4));
        var response = responses[rng.Next(responses.Length)];
        var timer = new DispatcherTimer { Interval = delay };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            chat.IsTyping = false;
            chat.Messages.Add(new Message
            {
                Id = chat.Messages.Count + 1,
                ChatId = chat.Id,
                Sender = partner,
                IsOutgoing = false,
                Text = response,
                Timestamp = DateTime.Now,
                Status = MessageStatus.Read,
                Type = MessageType.Text
            });
            chat.RefreshPreview();
            ResortAndFilter();
        };
        timer.Start();
    }

    private void ApplyFilter()
    {
        FilteredChats.Clear();
        IEnumerable<Chat> q = _data.Chats;
        if (Filter == "unread")   q = q.Where(c => c.HasUnread);
        else if (Filter == "groups")   q = q.Where(c => c.Type == ChatType.Group);
        else if (Filter == "channels") q = q.Where(c => c.Type == ChatType.Channel);
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var s = SearchText.Trim().ToLower();
            q = q.Where(c => c.Title.ToLower().Contains(s) || c.LastMessagePreview.ToLower().Contains(s));
        }
        q = q.OrderByDescending(c => c.IsPinned).ThenByDescending(c => c.LastMessage?.Timestamp ?? DateTime.MinValue);
        foreach (var c in q) FilteredChats.Add(c);
    }

    public void ResortAndFilter() => ApplyFilter();
}
