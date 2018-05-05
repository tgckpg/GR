using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Display;
using Windows.Devices.Sensors;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Helpers;
using Net.Astropenguin.Logging;

namespace GR.GSystem
{
	public class AccelerScroll : ActiveData
	{
		// Display Requests are accumulative, so we need static bool store the state
		private static bool StateActive = false;

		private Accelerometer Meter;
		private DisplayRequest DispRequest;

		private bool ReadingStarted = false;
		public Action<float> Delta;

		public float StartX = 0.2f;

		private float _X;
		public float X
		{
			get => _X;
			private set
			{
				_X = value;

				if ( Delta != null )
				{
					Worker.UIInvoke( () => Delta( 25.0f * _X ) );
					NotifyChanged( "X" );
				}
			}
		}

		public AccelerScroll()
		{
			Meter = Accelerometer.GetDefault( AccelerometerReadingType.Standard );
			Meter.ReportInterval = 20;

			DispRequest = new DisplayRequest();

			ReleaseActive();
		}

		public void StartReading()
		{
			if ( ReadingStarted )
				return;

			ReadingStarted = true;

			Meter.ReadingChanged += Meter_ReadingChanged;
		}

		public void StopReading()
		{
			ReleaseActive();

			Meter.ReadingChanged -= Meter_ReadingChanged;
			ReadingStarted = false;
		}

		private void Meter_ReadingChanged( Accelerometer sender, AccelerometerReadingChangedEventArgs args )
		{
			_X = ( float ) args.Reading.AccelerationX;
			if ( StartX < Math.Abs( _X ) )
			{
				X = _X;
				RequestActive();
			}
			else
			{
				X = 0;
				ReleaseActive();
			}
		}

		private void RequestActive()
		{
			if ( !StateActive )
			{
				StateActive = true;
				Logger.Log( "Accel", "Request Active", LogType.DEBUG );
				Worker.UIInvoke( DispRequest.RequestActive );
			}
		}

		private void ReleaseActive()
		{
			if ( StateActive )
			{
				StateActive = false;
				Logger.Log( "Accel", "Request Release", LogType.DEBUG );
				Worker.UIInvoke( DispRequest.RequestRelease );
			}
		}

	}
}