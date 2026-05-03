using System;
using System.Collections.ObjectModel;
using System.Linq;
using Engine.ViewModels;

namespace Engine.Models;

public enum ChatType { Direct, Group, Channel, Bot, Saved }

public class Chat : BaseViewModel
{
    private bool _isPinned, _isMuted, _isArchived, _isTyping;
    private int _unreadCount;
    private string _title = string.Empty;

    public int Id { get; set; }
    public ChatType Type { get; set; } = ChatType.Direct;
    public string Title { get => _title; set => SetProperty(ref _title, value); }
    public string Username { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AvatarColorStart { get; set; } = "#7B61FF";
    public string AvatarColorEnd { get; set; } = "#00D4FF";
    public ObservableCollection<User> Members { get; set; } = new();
    public ObservableCollection<Message> Messages { get; set; } = new();
    public User? Counterpart { get; set; }
    public int Subscribers { get; set; }

    public bool IsPinned { get => _isPinned; set => SetProperty(ref _isPinned, value); }
    public bool IsMuted { get => _isMuted; set => SetProperty(ref _isMuted, value); }
    public bool IsArchived { get => _isArchived; set => SetProperty(ref _isArchived, value); }
    public int UnreadCount { get => _unreadCount; set { if (SetProperty(ref _unreadCount, value)) OnPropertyChanged(nameof(HasUnread)); } }
    public bool HasUnread => UnreadCount > 0;
    public bool IsTyping { get => _isTyping; set => SetProperty(ref _isTyping, value); }

    public Message? LastMessage => Messages.LastOrDefault();

    public string LastMessagePreview
    {
        get
        {
            var m = LastMessage;
            if (m is null) return string.Empty;
            return m.Type switch
            {
                MessageType.Image => "Изображение",
                MessageType.File => "Файл" + (string.IsNullOrEmpty(m.FileName) ? string.Empty : ": " + m.FileName),
                MessageType.Voice => "Голосовое сообщение",
                MessageType.Sticker => "Стикер",
                _ => m.Text
            };
        }
    }

    public string LastMessageTime
    {
        get
        {
            var m = LastMessage;
            if (m is null) return string.Empty;
            var diff = DateTime.Now - m.Timestamp;
            if (diff.TotalMinutes < 1) return "сейчас";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} мин";
            if (m.Timestamp.Date == DateTime.Today) return m.Timestamp.ToString("HH:mm");
            if (m.Timestamp.Date == DateTime.Today.AddDays(-1)) return "вчера";
            if (diff.TotalDays < 7) return m.Timestamp.ToString("ddd");
            return m.Timestamp.ToString("dd.MM");
        }
    }

    public string Initials
    {
        get
        {
            if (string.IsNullOrEmpty(Title)) return "?";
            var parts = Title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
            return (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)).ToUpper();
        }
    }

    public string SubtitleText
    {
        get
        {
            if (Type == ChatType.Direct && Counterpart is not null)
            {
                if (Counterpart.IsOnline) return "в сети";
                var diff = DateTime.Now - Counterpart.LastSeen;
                if (diff.TotalMinutes < 60) return $"был(а) {(int)diff.TotalMinutes} мин назад";
                if (Counterpart.LastSeen.Date == DateTime.Today) return $"был(а) сегодня в {Counterpart.LastSeen:HH:mm}";
                return $"был(а) {Counterpart.LastSeen:dd.MM HH:mm}";
            }
            if (Type == ChatType.Group) return $"{Members.Count} участников";
            if (Type == ChatType.Channel) return $"{FormatSubscribers(Subscribers)} подписчиков";
            if (Type == ChatType.Bot) return "бот";
            if (Type == ChatType.Saved) return "сохранённые сообщения";
            return string.Empty;
        }
    }

    public static string FormatSubscribers(int n)
    {
        if (n >= 1_000_000) return (n / 1_000_000.0).ToString("0.#") + "M";
        if (n >= 1_000) return (n / 1_000.0).ToString("0.#") + "K";
        return n.ToString();
    }

    public void RefreshPreview()
    {
        OnPropertyChanged(nameof(LastMessagePreview));
        OnPropertyChanged(nameof(LastMessageTime));
        OnPropertyChanged(nameof(LastMessage));
    }
}
