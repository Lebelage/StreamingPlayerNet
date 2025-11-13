using System.Globalization;
using System.Windows.Data;

namespace StreamingPlayer.Infrastructure.Converters
{
    class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float speed)
            {
                return FormatSpeed(speed);
            }

            if (value is double doubleSpeed)
            {
                return FormatSpeed((float)doubleSpeed);
            }

            return "0 B/s";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        private string FormatSpeed(float speedBytesPerSecond)
        {
            if (speedBytesPerSecond >= 1024 * 1024) 
            {
                return $"{(speedBytesPerSecond / (1024 * 1024)):F2} MB/s";
            }
            else if (speedBytesPerSecond >= 1024) 
            {
                return $"{(speedBytesPerSecond / 1024):F2} kB/s";
            }
            else
            {
                return $"{speedBytesPerSecond:F2} B/s";
            }
        }

    }
}
