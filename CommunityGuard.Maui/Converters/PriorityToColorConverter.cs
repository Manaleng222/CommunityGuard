using System.Globalization;
using CommunityGuard.Domain.Enums;
using Microsoft.Maui.Graphics;

namespace CommunityGuard.Maui.Converters;

public class PriorityToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IncidentPriority priority)
        {
            return priority switch
            {
                IncidentPriority.Low => Colors.Green,
                IncidentPriority.Medium => Colors.Orange,
                IncidentPriority.High => Colors.Red,
                IncidentPriority.Critical => Colors.DarkRed,
                _ => Colors.Gray
            };
        }
        return Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}