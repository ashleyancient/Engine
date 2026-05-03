using System;
using Engine.ViewModels;

namespace Engine.Models;

public enum NotificationKind { NewMessage, Mention, ContactRequest, GroupInvite, System }

public class Notification : BaseViewModel
{
    private bool _isRead;

    public int Id { get; set; }
    public NotificationKind Kind { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int? ChatId { get; set; }
    public User? FromUser { get; set; }

    public bool IsRead { get => _isRead; set => SetProperty(ref _isRead, value); }

    public string TimeAgo
    {
        get
        {
            var diff = DateTime.Now - Timestamp;
            if (diff.TotalMinutes < 1) return "только что";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} мин назад";
            if (Timestamp.Date == DateTime.Today) return Timestamp.ToString("HH:mm");
            if (Timestamp.Date == DateTime.Today.AddDays(-1)) return "вчера";
            return Timestamp.ToString("dd.MM HH:mm");
        }
    }

    public string KindText => Kind switch
    {
        NotificationKind.NewMessage => "Новое сообщение",
        NotificationKind.Mention => "Упоминание",
        NotificationKind.ContactRequest => "Запрос в контакты",
        NotificationKind.GroupInvite => "Приглашение в группу",
        _ => "Системное событие"
    };
}
