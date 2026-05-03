using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Engine.Views.Controls;

public partial class AvatarControl : UserControl
{
    public AvatarControl()
    {
        InitializeComponent();
        UpdateColors();
        UpdateFontSize();
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(double), typeof(AvatarControl),
            new PropertyMetadata(40.0, OnSizeChanged));

    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AvatarControl c) c.UpdateFontSize();
    }

    private void UpdateFontSize() => SetCurrentValue(InitialsFontSizeProperty, Size * 0.4);

    public static readonly DependencyProperty InitialsFontSizeProperty =
        DependencyProperty.Register(nameof(InitialsFontSize), typeof(double), typeof(AvatarControl),
            new PropertyMetadata(16.0));

    public double InitialsFontSize
    {
        get => (double)GetValue(InitialsFontSizeProperty);
        set => SetValue(InitialsFontSizeProperty, value);
    }

    public static readonly DependencyProperty InitialsProperty =
        DependencyProperty.Register(nameof(Initials), typeof(string), typeof(AvatarControl),
            new PropertyMetadata("?"));

    public string Initials
    {
        get => (string)GetValue(InitialsProperty);
        set => SetValue(InitialsProperty, value);
    }

    public static readonly DependencyProperty ColorStartProperty =
        DependencyProperty.Register(nameof(ColorStart), typeof(string), typeof(AvatarControl),
            new PropertyMetadata("#7B61FF", OnColorChanged));

    public string ColorStart
    {
        get => (string)GetValue(ColorStartProperty);
        set => SetValue(ColorStartProperty, value);
    }

    public static readonly DependencyProperty ColorEndProperty =
        DependencyProperty.Register(nameof(ColorEnd), typeof(string), typeof(AvatarControl),
            new PropertyMetadata("#00D4FF", OnColorChanged));

    public string ColorEnd
    {
        get => (string)GetValue(ColorEndProperty);
        set => SetValue(ColorEndProperty, value);
    }

    public static readonly DependencyProperty ColorStartValueProperty =
        DependencyProperty.Register(nameof(ColorStartValue), typeof(Color), typeof(AvatarControl),
            new PropertyMetadata(Color.FromRgb(0x7B, 0x61, 0xFF)));

    public Color ColorStartValue
    {
        get => (Color)GetValue(ColorStartValueProperty);
        set => SetValue(ColorStartValueProperty, value);
    }

    public static readonly DependencyProperty ColorEndValueProperty =
        DependencyProperty.Register(nameof(ColorEndValue), typeof(Color), typeof(AvatarControl),
            new PropertyMetadata(Color.FromRgb(0x00, 0xD4, 0xFF)));

    public Color ColorEndValue
    {
        get => (Color)GetValue(ColorEndValueProperty);
        set => SetValue(ColorEndValueProperty, value);
    }

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AvatarControl c) c.UpdateColors();
    }

    private void UpdateColors()
    {
        try
        {
            if (!string.IsNullOrEmpty(ColorStart))
                ColorStartValue = (Color)ColorConverter.ConvertFromString(ColorStart);
            if (!string.IsNullOrEmpty(ColorEnd))
                ColorEndValue = (Color)ColorConverter.ConvertFromString(ColorEnd);
        }
        catch { }
    }

    public static readonly DependencyProperty ShowOnlineProperty =
        DependencyProperty.Register(nameof(ShowOnline), typeof(bool), typeof(AvatarControl),
            new PropertyMetadata(false));

    public bool ShowOnline
    {
        get => (bool)GetValue(ShowOnlineProperty);
        set => SetValue(ShowOnlineProperty, value);
    }

    public static readonly DependencyProperty PulseProperty =
        DependencyProperty.Register(nameof(Pulse), typeof(bool), typeof(AvatarControl),
            new PropertyMetadata(false));

    public bool Pulse
    {
        get => (bool)GetValue(PulseProperty);
        set => SetValue(PulseProperty, value);
    }
}
