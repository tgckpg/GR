using Net.Astropenguin.Logging;

namespace wenku8.System
{
    class LogControl
    {
        public static readonly string ID = typeof( LogControl ).Name;

        public static void SetFilter( string Preset )
        {
            Logger.LogFilter.Clear();
            Logger.LogFilter.Add( LogType.SYSTEM );

            switch( Preset )
            {
                case "DEBUG":
                    Logger.LogFilter.Add( LogType.DEBUG );
                    Logger.LogFilter.Add( LogType.INFO );
                    Logger.LogFilter.Add( LogType.WARNING );
                    Logger.LogFilter.Add( LogType.ERROR );
                    break;
                case "INFO":
                    Logger.LogFilter.Add( LogType.INFO );
                    Logger.LogFilter.Add( LogType.WARNING );
                    Logger.LogFilter.Add( LogType.ERROR );
                    break;
                case "WARNING":
                    Logger.LogFilter.Add( LogType.WARNING );
                    Logger.LogFilter.Add( LogType.ERROR );
                    break;
                case "ERROR":
                    Logger.LogFilter.Add( LogType.ERROR );
                    break;
                case "NONE":
                default:
                    Preset = "NONE";
                    break;
            }

            Logger.Log( ID, string.Format( "LogLevel set to {0}", Preset ), LogType.SYSTEM );
        }
    }
}
