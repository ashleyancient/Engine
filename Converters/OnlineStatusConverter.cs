using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Engine.Converters;

public class OnlineStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isOnline = value is bool b && b;
        if (isOnline)
        {
            if (Application.Current?.Resources["OnlineBrush"] is Brush br) return br;
            return new SolidColorBrush(Color.FromRgb(0x2E, 0xD5, 0x73));
        }
        if (Application.Current?.Resources["TextSecondaryBrush"] is Brush br2) return br2;
        return new SolidColorBrush(Color.FromRgb(0x8B, 0x8F, 0xA8));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class OnlineStatusVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isOnline = value is bool b && b;
        return isOnline ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class HexToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (value is string s && !string.IsNullOrEmpty(s))
            {
                var color = (Color)ColorConverter.ConvertFromString(s);
                if (targetType == typeof(Color)) return color;
                return new SolidColorBrush(color);
            }
        }
        catch { }
        if (targetType == typeof(Color)) return Colors.Transparent;
        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class TwoHexToGradientConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (values is { Length: >= 2 } && values[0] is string a && values[1] is string b)
            {
                var brush = new LinearGradientBrush(
                    (Color)ColorConverter.ConvertFromString(a),
                    (Color)ColorConverter.ConvertFromString(b),
                    45);
                brush.Freeze();
                return brush;
            }
        }
        catch { }
        return Brushes.Transparent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class StringEqualsToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var sv = value?.ToString() ?? string.Empty;
        var sp = parameter?.ToString() ?? string.Empty;
        return string.Equals(sv, sp, StringComparison.OrdinalIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class StringEqualsToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var sv = value?.ToString() ?? string.Empty;
        var sp = parameter?.ToString() ?? string.Empty;
        return string.Equals(sv, sp, StringComparison.OrdinalIgnoreCase);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b && b) return parameter?.ToString() ?? string.Empty;
        return Binding.DoNothing;
    }
}
