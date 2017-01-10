using System;
using System.Globalization;

using Net.Astropenguin.DataModel;

namespace wenku8.Model.Comments
{
    class Comment : ActiveData
    {
        public DateTime TimeStamp { get; protected set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public int Index { get; set; }

        public string PostTime
        {
            get
            {
                return TimeStamp.ToString();
            }
            set
            {
                TimeStamp = DateTime.ParseExact( value + "+8", "yyyyMMddHHmmssz", CultureInfo.CurrentUICulture );
            }
        }
    }
}