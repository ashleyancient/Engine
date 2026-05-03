using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Engine.Converters;

public class MessageAlignmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isOutgoing = value is bool b && b;
        return isOutgoing ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class MessageBubbleBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isOutgoing = value is bool b && b;
        var key = isOutgoing ? "BubbleOutBrush" : "BubbleInBrush";
        if (Application.Current?.Resources[key] is Brush br) return br;
        return isOutgoing
            ? (Brush)new SolidColorBrush(Color.FromRgb(0x3D, 0x2E, 0x6B))
            : new SolidColorBrush(Color.FromRgb(0x1E, 0x22, 0x35));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class MessageCornerRadiusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isOutgoing = value is bool b && b;
        return isOutgoing
            ? new CornerRadius(14, 14, 4, 14)
            : new CornerRadius(14, 14, 14, 4);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
