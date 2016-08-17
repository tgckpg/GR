using Net.Astropenguin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.System
{
    sealed class ActionCenter
    {
        public const string General = "noti";
        public const string Warning = "impo";
        public const string Error = "Err";

        private readonly Type MySelf = typeof( ActionCenter );

        public ActionCenter()
        {
            MessageBus.OnDelivery += MessageBus_OnDelivery;
        }

        ~ActionCenter()
        {
            MessageBus.OnDelivery -= MessageBus_OnDelivery;
        }

        private void MessageBus_OnDelivery( Message Mesg )
        {
            if ( Mesg.TargetType != MySelf ) return;

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
