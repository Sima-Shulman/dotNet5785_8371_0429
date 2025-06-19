using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PL
{


    public class RoleToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.Enums.Role role && parameter is string roleName)
            {
                return role.ToString().Equals(roleName, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter is string roleName)
            {
                return Enum.Parse(typeof(BO.Enums.Role), roleName);
            }
            return BO.Enums.Role.None;
        }
    }
    public class ConvertUpdateToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Example logic: Show the element if ButtonText is "Update"
            if (value is string buttonText && buttonText == "Update")
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConvertUpdateToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Example logic: Show the element if ButtonText is "Update"
            if (value is string buttonText && buttonText == "Update")
            {
                return true;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConvertUpdateToFalse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string buttonText && buttonText == "Update")
            {
                return false;
            }
            return true; ; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    //class ConvertRoleToBolean: IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        BO.Enums.Role year = (BO.Year)value;
    //        switch (year)
    //        {
    //            case BO.Year.FirstYear:
    //                return Brushes.Yellow;
    //            case BO.Year.SecondYear:
    //                return Brushes.Orange;
    //            case BO.Year.ThirdYear:
    //                return Brushes.Green;
    //            case BO.Year.ExtraYear:
    //                return Brushes.PaleVioletRed;
    //            case BO.Year.None:
    //                return Brushes.White;
    //            default:
    //                return Brushes.White;
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
