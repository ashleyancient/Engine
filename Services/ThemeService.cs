using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Engine.Services;

public enum ThemeMode { Dark, Light }

public class ThemeService
{
    private static ThemeService? _instance;
    public static ThemeService Instance => _instance ??= new ThemeService();

    public event Action<ThemeMode>? ThemeChanged;

    private ThemeMode _mode = ThemeMode.Dark;
    private string _accentStart = "#7B61FF";
    private string _accentEnd = "#00D4FF";

    public ThemeMode Mode => _mode;
    public string AccentStart => _accentStart;
    public string AccentEnd => _accentEnd;

    private static readonly Dictionary<string, (Color Dark, Color Light)> _palette = new()
    {
        ["PrimaryBrush"]        = (Color.FromRgb(0x0F,0x11,0x17), Color.FromRgb(0xF2,0xF3,0xF7)),
        ["SurfaceBrush"]        = (Color.FromRgb(0x1A,0x1D,0x27), Color.FromRgb(0xFF,0xFF,0xFF)),
        ["CardBrush"]           = (Color.FromRgb(0x22,0x26,0x39), Color.FromRgb(0xEC,0xEE,0xF5)),
        ["BorderBrush"]         = (Color.FromRgb(0x2C,0x2F,0x45), Color.FromRgb(0xDC,0xDF,0xE8)),
        ["TextPrimaryBrush"]    = (Color.FromRgb(0xE8,0xEA,0xED), Color.FromRgb(0x12,0x14,0x1C)),
        ["TextSecondaryBrush"]  = (Color.FromRgb(0x8B,0x8F,0xA8), Color.FromRgb(0x5F,0x66,0x7E)),
        ["BubbleOutBrush"]      = (Color.FromRgb(0x3D,0x2E,0x6B), Color.FromRgb(0xD8,0xCC,0xFF)),
        ["BubbleInBrush"]       = (Color.FromRgb(0x1E,0x22,0x35), Color.FromRgb(0xFF,0xFF,0xFF)),
        ["DangerBrush"]         = (Color.FromRgb(0xFF,0x47,0x57), Color.FromRgb(0xE5,0x3E,0x4D)),
        ["OnlineBrush"]         = (Color.FromRgb(0x2E,0xD5,0x73), Color.FromRgb(0x1F,0xB0,0x55)),
        ["SecondaryBrush"]      = (Color.FromRgb(0x18,0x1B,0x24), Color.FromRgb(0xFA,0xFB,0xFF)),
        ["HoverBrush"]          = (Color.FromRgb(0x2A,0x2E,0x44), Color.FromRgb(0xE5,0xE8,0xF1)),
        ["ScrollThumbBrush"]    = (Color.FromRgb(0x3A,0x3D,0x55), Color.FromRgb(0xC8,0xCC,0xD9))
    };

    public void Apply(ThemeMode mode)
    {
        _mode = mode;
        var app = Application.Current;
        if (app is null) return;
        foreach (var (key, value) in _palette)
        {
            var color = mode == ThemeMode.Dark ? value.Dark : value.Light;
            app.Resources[key] = new SolidColorBrush(color);
        }
        ApplyAccent(_accentStart, _accentEnd);
        ThemeChanged?.Invoke(mode);
    }

    public void ApplyAccent(string start, string end)
    {
        _accentStart = start; _accentEnd = end;
        var app = Application.Current;
        if (app is null) return;
        try
        {
            var colorStart = (Color)ColorConverter.ConvertFromString(start);
            var colorEnd   = (Color)ColorConverter.ConvertFromString(end);
            app.Resources["AccentBrush"]          = new SolidColorBrush(colorStart);
            app.Resources["AccentSecondaryBrush"] = new SolidColorBrush(colorEnd);
            var grad = new LinearGradientBrush(colorStart, colorEnd, 45);
            grad.Freeze();
            app.Resources["AccentGradientBrush"] = grad;
        }
        catch { }
    }

    public void Toggle() => Apply(_mode == ThemeMode.Dark ? ThemeMode.Light : ThemeMode.Dark);
}
