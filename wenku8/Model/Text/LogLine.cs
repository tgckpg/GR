using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wenku8.Model.Text
{
    class LogLine
    {
        public string Level { get; private set; }
        public string Time { get; private set; }
        public string Tag { get; private set; }
        public string Message { get; private set; }

        public LogLine( string Line )
        {
            string Flag = "\\[([^\\]]+)\\]";
            Regex LineMatch = new Regex(
                string.Format( "{0}{1}{2} {3}", Flag, Flag, Flag, "(.+)" )
            );

            Match m = LineMatch.Match( Line );
            if( m.Success )
            {
                Time = m.Groups[ 1 ].Value;
                Level = m.Groups[ 2 ].Value;
                Tag = m.Groups[ 3 ].Value;
                Message = m.Groups[ 4 ].Value;
            }
            else
            {
                Time = "";
                Level = "";
                Tag = "";
                Message = Line;
            }
        }
    }
}
