using Mufasa.BackEnd.Designer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mufasa.Pages
{

    class VolumeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
                    object parameter, CultureInfo culture)
        {
            //if (values.Length != 2)
            //    return null;

            var item = values[0] as FragmentViewModel;
            var view = values[1] as ICollectionView;
            if (item == null || view == null)
                return null;

            var volume = item.Length * 0.1 / item.Concentration;
            return Math.Round(volume,1).ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
