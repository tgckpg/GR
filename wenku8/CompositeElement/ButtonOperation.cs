using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.CompositeElement
{
    sealed class ButtonOperation
    {
        private Func<Task> Operate;
        private Action OpComp;

        private AppBarButtonEx Btn;

        public void SetOp( Func<Task> Op, bool Run = true )
        {
            Operate = Op;
            if ( Run ) StartOp();
        }

        public void SetComplete( Action Complete )
        {
            OpComp = Complete;
        }

        public ButtonOperation( AppBarButtonEx Button )
        {
            Btn = Button;
            SetTemplate();
        }

        private void SetTemplate()
        {
            Btn.Click += ( s, e ) => StartOp();
        }

        private async void StartOp()
        {
            if ( Btn.IsLoading || Operate == null ) return;

            Btn.IsLoading = true;
            await Operate();
            Btn.IsLoading = false;
            OpComp?.Invoke();
        }

    }
}