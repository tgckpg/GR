using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Devices.Sensors;
using Windows.System.Display;

using Net.Astropenguin.Helpers;

namespace GR.GSystem
{
	using Effects;
	using Windows.UI.ViewManagement;

	public class AccelerScroll
	{
		// Display Requests are accumulative, so we need static bool store the state
		public static bool StateActive { get; private set; } = false;

		private Accelerometer Meter;
		private DisplayRequest DispRequest;

		private bool ReadingStarted = false;
		public Action<float> Delta;

		public bool Available => Meter != null;

		public float AccelerMultiplier;
		public float TerminalVelocity;
		public float BrakeOffset;
		public float Brake;

		public bool ForceBrake = false;

		private float _X;
		private float HitBound;

		private Queue<float> SRSamples = new Queue<float>();

		public AccelerScroll()
		{
			Meter = Accelerometer.GetDefault( AccelerometerReadingType.Standard );
			if( Meter == null ) return;

			Meter.ReportInterval = 20;

			DispRequest = new DisplayRequest();
			ReleaseActive();
		}

		public void UpdateOrientation( DisplayOrientations Ori )
		{
			if ( Meter == null ) return;
			Meter.ReadingTransform = Ori;
		}

		public void StartCallibrate()
		{
			if ( Meter == null ) return;

			Meter.ReadingChanged -= Meter_ReadingChanged;
			Meter.ReadingChanged += Meter_CallibrateChanged;
		}

		public void EndCallibration()
		{
			UpdateHitBound();
			if ( Meter == null ) return;

			Meter.ReadingChanged -= Meter_CallibrateChanged;
			Meter.ReadingChanged += Meter_ReadingChanged;
		}

		public void StartReading()
		{
			UpdateHitBound();

			if ( Meter == null || ReadingStarted )
				return;

			ReadingStarted = true;

			Meter.ReadingChanged += Meter_ReadingChanged;
		}

		public void StopReading()
		{
			if ( Meter == null ) return;

			ReleaseActive();

			Meter.ReadingChanged -= Meter_ReadingChanged;
			ReadingStarted = false;
		}

		private void Meter_CallibrateChanged( Accelerometer sender, AccelerometerReadingChangedEventArgs args )
		{
			// Stabilize the value
			Easings.ParamTween( ref _X, ( float ) args.Reading.AccelerationX, 0.90f, 0.10f );
			Delta( _X );
		}

		private void Meter_ReadingChanged( Accelerometer sender, AccelerometerReadingChangedEventArgs args )
		{
			Easings.ParamTween( ref _X, ( float ) args.Reading.AccelerationX, 0.90f, 0.10f );

			float PosX = 0.5f * ( 1 + _X );

			bool InRange = ( PosX < HitBound || HitBound + Brake < PosX );

			if ( InRange )
			{
				Delta?.Invoke( _X - BrakeOffset );
				RequestActive();
			}
			else
			{
				Delta?.Invoke( 0 );
				ReleaseActive();
			}
		}

		private void RequestActive()
		{
			if ( !StateActive )
			{
				StateActive = true;
				Worker.UIInvoke( DispRequest.RequestActive );
			}
		}

		private void ReleaseActive()
		{
			if ( StateActive )
			{
				StateActive = false;
				Worker.UIInvoke( DispRequest.RequestRelease );
			}
		}

		private void UpdateHitBound()
		{
			HitBound = ( BrakeOffset + 1 ) * ( 0.5f * ( 1 - Brake ) );
		}

	}
}