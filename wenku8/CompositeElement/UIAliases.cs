using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace wenku8.CompositeElement
{
    using Resources;

    static class UIAliases
    {
        private static SolidColorBrush ContextColor = new SolidColorBrush( LayoutSettings.RelativeMajorBackgroundColor );

        public static AppBarButton CreateAppBarBtn( Symbol Symbol, string Label )
        {
            return new AppBarButton() { Icon = new SymbolIcon( Symbol ), Label = Label, Foreground = ContextColor };
        }

        public static AppBarButton CreateAppBarBtn( string Glyph, string Label )
        {
            return new AppBarButton() { Icon = new FontIcon() { Glyph = Glyph }, Label = Label, Foreground = ContextColor };
        }

        public static AppBarButton CreateAppBarBtn( PathIcon Icon, string Label )
        {
            return new AppBarButton() { Icon = Icon, Label = Label, Foreground = ContextColor };
        }

        public static AppBarButtonEx CreateAppBarBtnEx( Symbol Symbol, string Label )
        {
            return new AppBarButtonEx() { Icon = new SymbolIcon( Symbol ), Label = Label, Foreground = ContextColor };
        }

        public static AppBarButtonEx CreateAppBarBtnEx( string Glyph, string Label )
        {
            return new AppBarButtonEx() { Icon = new FontIcon() { Glyph = Glyph }, Label = Label, Foreground = ContextColor };
        }

        public static AppBarButtonEx CreateAppBarBtnEx( PathIcon Icon, string Label )
        {
            return new AppBarButtonEx() { Icon = Icon, Label = Label, Foreground = ContextColor };
        }

        public static SecondaryIconButton CreateSecondaryIconBtn( string Glyph, string Label )
        {
            return new SecondaryIconButton( Glyph ) { Label = Label };
        }

        public static AppBarToggleButton CreateToggleBtn( string Glyph, string Label )
        {
            return new AppBarToggleButton() { Icon = new FontIcon() { Glyph = Glyph }, Label = Label, Foreground = ContextColor };
        }

        public static MessageDialog CreateDialog( string Mesg, Action PrimaryAction, string PrimaryText, string SecondaryText )
        {
            MessageDialog MsgBox = new MessageDialog( Mesg );
            MsgBox.Commands.Add( new UICommand( PrimaryText, ( e ) => PrimaryAction() ) );
            MsgBox.Commands.Add( new UICommand( SecondaryText ) );

            return MsgBox;
        }

        public static MessageDialog CreateDialog( string Mesg, string Title, Action PrimaryAction, string PrimaryText, string SecondaryText )
        {
            MessageDialog MsgBox = new MessageDialog( Mesg, Title );
            MsgBox.Commands.Add( new UICommand( PrimaryText, ( e ) => PrimaryAction() ) );
            MsgBox.Commands.Add( new UICommand( SecondaryText ) );

            return MsgBox;
        }

        public static MessageDialog CreateDialog( string Mesg )
        {
            return new MessageDialog( Mesg );
        }

        public static MessageDialog CreateDialog( string Mesg, string Title )
        {
            return new MessageDialog( Mesg, Title );
        }

    }
}