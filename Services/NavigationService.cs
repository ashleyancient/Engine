using System;
using System.Collections.Generic;

namespace Engine.Services;

public enum AppSection { Chats, Contacts, Stories, Channels, Favorites, Notifications, Settings }

public class NavigationService
{
    private static NavigationService? _instance;
    public static NavigationService Instance => _instance ??= new NavigationService();

    public event Action<AppSection>? SectionChanged;
    public event Action? RequestExit;

    private AppSection _current = AppSection.Chats;
    public AppSection CurrentSection => _current;

    public void Navigate(AppSection section) { _current = section; SectionChanged?.Invoke(section); }
    public void Exit() => RequestExit?.Invoke();

    public IReadOnlyList<AppSection> AllSections { get; } = new[]
    {
        AppSection.Chats, AppSection.Contacts, AppSection.Stories,
        AppSection.Channels, AppSection.Favorites, AppSection.Notifications, AppSection.Settings
    };
}
