using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Engine.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }
    public bool UseHidden { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = value is bool b && b;
        if (Invert) flag = !flag;
        return flag ? Visibility.Visible : (UseHidden ? Visibility.Hidden : Visibility.Collapsed);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility v)
        {
            bool flag = v == Visibility.Visible;
            return Invert ? !flag : flag;
        }
        return false;
    }
}

public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = value is bool b && b;
        return flag ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility v && v == Visibility.Collapsed;
}

public class NullToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isNull = value is null || (value is string s && string.IsNullOrEmpty(s));
        if (Invert) isNull = !isNull;
        return isNull ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int n = 0;
        if (value is int i) n = i;
        else if (value is long l) n = (int)l;
        return n > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
