using System;
using Windows.Networking.Connectivity;
using Net.Astropenguin.Logging;

namespace GR.AdvDM
{
	using Config;

	static class WCacheMode
	{
		public static readonly string ID = typeof( WCacheMode ).Name;
		public delegate void OfflineTrigger();
		public static event OfflineTrigger OfflineEnabled;

		private static bool IsOffline = false;

		public static bool OfflineMode
		{
			set
			{
				if ( IsOffline = value )
				{
					if ( OfflineEnabled != null )
					{
						OfflineEnabled();
					}
				}
			}
			get
			{
				return IsOffline;
			}
		}

		public static void Initialize()
		{
			Logger.Log( ID, "Determinating Connection Mode ...", LogType.INFO );

			try
			{
				IsOffline = 
					( NetworkInformation.GetInternetConnectionProfile().GetNetworkConnectivityLevel()
					== NetworkConnectivityLevel.InternetAccess );

				OfflineMode = Properties.DATA_CONNECTION_WIFI_ONLY && !IsWiFiAvailable();
				NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
			}
			catch( Exception ex )
			{
				Logger.Log(
					ID
					, string.Format( "Cannot determinate the network profile: {0}", ex.Message )
					, LogType.WARNING
				);

				Logger.Log( ID, "Assuming Connected", LogType.INFO );
				IsOffline = false;
			}
		}

		static void NetworkInformation_NetworkStatusChanged( object sender )
		{
			if ( Properties.DATA_CONNECTION_WIFI_ONLY && !IsWiFiAvailable() )
			{
				OfflineMode = true;
			}
			OfflineMode = false;
		}

		public static bool IsWiFiAvailable()
		{
			return NetworkInformation.GetInternetConnectionProfile().IsWlanConnectionProfile;
		}
	}
}
