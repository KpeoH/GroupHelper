using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using WPFinal.Models;

namespace WPFinal.ViewModels.Converters
{
    public class SubjectsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var subjects = value as ObservableCollection<Subject>;
            if (subjects != null)
            {
                return string.Join(", ", subjects.Select(s => s.Name));
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
