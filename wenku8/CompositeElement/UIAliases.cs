using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace wenku8.CompositeElement
{
    static class UIAliases
    {
        public static AppBarButton CreateAppBarBtn( Symbol Symbol, string Label )
        {
            AppBarButton Btn = new AppBarButton()
            {
                Icon = new SymbolIcon( Symbol ),
                Label = Label
            };

            return Btn;
        }

        public static AppBarButton CreateAppBarBtn( string Glyph, string Label )
        {
            AppBarButton Btn = new AppBarButton()
            {
                Icon = new FontIcon() { Glyph = Glyph },
                Label = Label
            };

            return Btn;
        }

        public static AppBarButton CreateAppBarBtn( PathIcon Icon, string Label )
        {
            AppBarButton Btn = new AppBarButton()
            {
                Icon = Icon
                , Label = Label
            };

            return Btn;
        }

        public static AppBarButtonEx CreateAppBarBtnEx( Symbol Symbol, string Label )
        {
            AppBarButtonEx Btn = new AppBarButtonEx()
            {
                Icon = new SymbolIcon( Symbol ),
                Label = Label
            };

            return Btn;
        }

        public static AppBarButtonEx CreateAppBarBtnEx( string Glyph, string Label )
        {
            AppBarButtonEx Btn = new AppBarButtonEx()
            {
                Icon = new FontIcon() { Glyph = Glyph },
                Label = Label
            };

            return Btn;
        }

        public static AppBarButtonEx CreateAppBarBtnEx( PathIcon Icon, string Label )
        {
            AppBarButtonEx Btn = new AppBarButtonEx()
            {
                Icon = Icon,
                Label = Label
            };

            return Btn;
        }

        public static SecondaryIconButton CreateSecondaryIconBtn( string Glyph, string Label )
        {
            return new SecondaryIconButton( Glyph ) { Label = Label };
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

    }
}