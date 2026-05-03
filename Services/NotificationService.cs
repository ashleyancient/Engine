using System;
using System.Collections.ObjectModel;
using Engine.Models;

namespace Engine.Services;

public class NotificationService
{
    private static NotificationService? _instance;
    public static NotificationService Instance => _instance ??= new NotificationService();

    public ObservableCollection<Notification> Toasts { get; } = new();
    public event Action<Notification>? ToastRequested;

    public void Show(string title, string body, NotificationKind kind = NotificationKind.System)
    {
        var n = new Notification
        {
            Id = (int)(DateTime.Now.Ticks % int.MaxValue),
            Title = title, Body = body, Kind = kind, Timestamp = DateTime.Now
        };
        Toasts.Add(n);
        ToastRequested?.Invoke(n);
    }

    public void Dismiss(Notification n) => Toasts.Remove(n);
}
