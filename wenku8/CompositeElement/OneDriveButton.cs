using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wenku8.Config;
using wenku8.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Net.Astropenguin.UI.Icons;

namespace wenku8.CompositeElement
{
    [TemplatePart( Name = ProgressRingName, Type = typeof( ProgressRing ) )]
    [TemplatePart( Name = IconName, Type = typeof( IconBase ) )]
    public class OneDriveButton : Button
    {
        private const string ProgressRingName = "OneDriveRing";
        private const string IconName = "OneDriveIcon";

        private ProgressRing OneDriveRing;
        private IconBase Icon;

        private Func<Task> SyncOp;
        private Action SyncComp;

        public static DependencyProperty InSyncProperty = DependencyProperty.Register( "InSync", typeof( bool ), typeof( OneDriveButton ), new PropertyMetadata( false, SyncStateUpdate ) );
        public bool InSync
        {
            get { return ( bool ) GetValue( InSyncProperty ); }
            set
            {
                SetValue( InSyncProperty, value );
                if( OneDriveRing != null )
                    OneDriveRing.IsActive = value;
            }
        }

        public void SetSync( Func<Task> SyncOp )
        {
            this.SyncOp = SyncOp;
            StartSync();
        }

        public void SetComplete( Action Complete )
        {
            SyncComp = Complete;
        }

        private async void StartSync()
        {
            if ( !Properties.ENABLE_ONEDRIVE || InSync || SyncOp == null ) return;

            InSync = true;
            await SyncOp();
            InSync = false;
            if ( SyncComp != null ) SyncComp();
        }

        private static void SyncStateUpdate( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
        }

        public OneDriveButton()
            :base()
        {
            DefaultStyleKey = typeof( OneDriveButton );
            Click += OneDriveButton_Click;
        }

        private void OneDriveButton_Click( object sender, RoutedEventArgs e )
        {
            StartSync();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            OneDriveRing = GetTemplateChild( ProgressRingName ) as ProgressRing;
            Icon = GetTemplateChild( IconName ) as IconBase;

            if ( !Properties.ENABLE_ONEDRIVE || OneDriveRing == null || Icon == null ) return;

            Icon.Visibility = Visibility.Visible;

            Icon.Width = 0.8 * Width;
            Icon.Height = 0.8 * Height;
            OneDriveRing.Width = Width;
            OneDriveRing.Height = Height;
            OneDriveRing.IsActive = InSync;

            StartSync();
        }
    }
}
