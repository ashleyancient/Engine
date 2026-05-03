using System;
using System.Globalization;
using System.Windows.Data;

namespace Engine.Converters;

public class TimeAgoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DateTime dt) return string.Empty;
        var diff = DateTime.Now - dt;
        if (diff.TotalSeconds < 30) return "только что";
        if (diff.TotalMinutes < 1) return "меньше минуты назад";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} мин назад";
        if (dt.Date == DateTime.Today) return "сегодня в " + dt.ToString("HH:mm");
        if (dt.Date == DateTime.Today.AddDays(-1)) return "вчера в " + dt.ToString("HH:mm");
        if (diff.TotalDays < 7) return dt.ToString("ddd в HH:mm");
        return dt.ToString("dd.MM.yyyy");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
