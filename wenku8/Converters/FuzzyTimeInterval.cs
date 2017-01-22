using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Data;

using Net.Astropenguin.Loaders;

namespace wenku8.Converters
{
    sealed public class FuzzyTimeInterval : IValueConverter
    {
        private StringResources stx;

        private string LabelMin;
        private string LabelHour;
        private string LabelDay;

        public FuzzyTimeInterval()
        {
            stx = new StringResources( "DateTimeUnits" );
            LabelMin = stx.Str( "FT_Minute" );
            LabelHour = stx.Str( "FT_Hour" );
            LabelDay = stx.Str( "FT_Day" );
        }

        public object Convert( object value, Type targetType, object parameter, string language )
        {
            double Interval = ( double ) value;

            double LeftMinutes = Interval % 60;
            string TimeStamp = "";

            if ( 0 < LeftMinutes )
                TimeStamp = string.Format( LabelMin, LeftMinutes );

            double Hours = ( Interval - LeftMinutes ) / 60;
            double LeftHours = Hours % 24;

            if ( 0 < LeftHours )
                TimeStamp = string.Format( LabelHour, LeftHours ) + " " + TimeStamp;

            double Days = ( Hours - LeftHours ) / 24;
            if ( 0 < Days )
                TimeStamp = string.Format( LabelDay, Days ) + " " + TimeStamp;

            return TimeStamp;
        }

        public object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            return false;
        }
    }
}