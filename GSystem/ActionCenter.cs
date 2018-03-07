using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Messaging;

namespace GR.GSystem
{
	using CompositeElement;

	sealed class ActionCenter
	{
		public const string General = "noti";
		public const string Warning = "impo";
		public const string Error = "Err";

		public static ActionCenter Instance { get; private set; }

		private readonly Type SelfType = typeof( ActionCenter );

		public ActionCenter()
		{
			MessageBus.Subscribe( this, MessageBus_OnDelivery );
		}

		public static void Init() { Instance = new ActionCenter(); }

		public void ShowError( string Key )
		{
			Worker.UIInvoke( () =>
			{
				StringResources stx = new StringResources( "Error" );
				var j = Popups.ShowDialog( UIAliases.CreateDialog( stx.Str( Key ) ) );
			} );
		}

		private void MessageBus_OnDelivery( Message Mesg )
		{
			if ( Mesg.TargetType != SelfType ) return;

			if( Mesg.Dispatcher != null )
			{
				ProcesssDispatcherMessage( Mesg );
				return;
			}

			if( Mesg.Payload != null )
			{
				ProcessPayloadMessage( Mesg );
				return;
			}

			ProcessGeneralMessage( Mesg );
		}

		private void ProcesssDispatcherMessage( Message Mesg )
		{

		}

		private void ProcessPayloadMessage( Message Mesg )
		{

		}

		private void ProcessGeneralMessage( Message Mesg )
		{

		}

	}
}