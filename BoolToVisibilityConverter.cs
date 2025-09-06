using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ETAG_ERP
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        // هذا التحويل من bool إلى Visibility
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isOwner = false;

            if (value is bool)
                isOwner = (bool)value;

            return isOwner ? Visibility.Visible : Visibility.Collapsed;
        }

        // هذا التحويل العكسي (غير مستخدم هنا)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
