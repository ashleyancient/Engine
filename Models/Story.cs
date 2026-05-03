using System;
using Engine.ViewModels;

namespace Engine.Models;

public class Story : BaseViewModel
{
    private bool _isViewed;
    private bool _isLiked;

    public int Id { get; set; }
    public User? Author { get; set; }
    public DateTime PublishedAt { get; set; }
    public string Caption { get; set; } = string.Empty;
    public string GradientStart { get; set; } = "#7B61FF";
    public string GradientEnd { get; set; } = "#00D4FF";
    public string PlaceholderText { get; set; } = string.Empty;

    public bool IsViewed { get => _isViewed; set => SetProperty(ref _isViewed, value); }
    public bool IsLiked { get => _isLiked; set => SetProperty(ref _isLiked, value); }

    public string TimeAgo
    {
        get
        {
            var diff = DateTime.Now - PublishedAt;
            if (diff.TotalMinutes < 1) return "только что";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} мин назад";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} ч назад";
            return $"{(int)diff.TotalDays} д назад";
        }
    }
}
