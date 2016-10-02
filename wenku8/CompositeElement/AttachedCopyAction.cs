using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;

namespace wenku8.CompositeElement
{
    public sealed class AttachedCopyAction
    {
        public static readonly string ID = typeof( AttachedCopyAction ).Name;
        // Source
        public static string GetSource( DependencyObject d ) { return ( string ) d.GetValue( SourceProperty ); }
        public static void SetSource( DependencyObject d, string Source ) { d.SetValue( SourceProperty, Source ); }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached( "Source", typeof( string ),
            typeof( AttachedCopyAction ), new PropertyMetadata( null, InitiateContextMenu ) );

        private static void InitiateContextMenu( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            FrameworkElement Elem = d as FrameworkElement;
            if ( d == null ) return;

            object HasMenu = d.GetValue( FlyoutBase.AttachedFlyoutProperty );

            if ( HasMenu == null )
            {
                d.SetValue( FlyoutBase.AttachedFlyoutProperty, CreateMenuFlyout( d ) );

                Elem.RightTapped += ( s, e2 ) =>
                {
                    FlyoutBase.ShowAttachedFlyout( ( FrameworkElement ) s );
                };
            }
            else
            {
                Logger.Log( ID, "Menuflyout already attached", LogType.DEBUG );
            }
        }

        private static MenuFlyout CreateMenuFlyout( DependencyObject d )
        {
            MenuFlyout Menu = new MenuFlyout();
            MenuFlyoutItem CopyAction = new MenuFlyoutItem();
            StringResources stx = new StringResources( "ContextMenu" );
            CopyAction.Text = stx.Text( "Copy" );

            CopyAction.Click += ( s, e ) =>
            {
                DataPackage Data = new DataPackage();

                Data.SetText( GetSource( d ) );
                Clipboard.SetContent( Data );
            };

            Menu.Items.Add( CopyAction );

            return Menu;
        }
    }
}