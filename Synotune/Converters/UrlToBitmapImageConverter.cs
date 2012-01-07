using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Synotune.Converters
{
    public class UrlToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, string typeName, object parameter, string language)
        {
            Uri source = new Uri((string)value);
            return new BitmapImage(source);

        }

        public object ConvertBack(object value, string typeName, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
