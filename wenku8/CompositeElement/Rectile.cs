using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

using Net.Astropenguin.Logging;

namespace wenku8.CompositeElement
{
    public enum HoverStates
    {
        Idle, Hovered, Loading
    }

    public enum RectileSize
    {
        Large = 270, Medium = 135
    }

    [TemplateVisualState( GroupName = HoverStatesGroup, Name = "Loading" )]
    [TemplateVisualState( GroupName = HoverStatesGroup, Name = "Idle" )]
    [TemplateVisualState( GroupName = HoverStatesGroup, Name = "Hovered" )]
    [TemplatePart( Name = "Stage" )]
    [TemplatePart( Name = "ContentTC" )]
    [TemplatePart( Name = "ContentTCC" )]
    [TemplatePart( Name = "ContentTCT" )]
    [TemplatePart( Name = "ContentCT" )]
    [TemplatePart( Name = "ContentCTT" )]
    [TemplatePart( Name = "ContentCTC" )]
    public class Rectile : Control, INotifyPropertyChanged
    {
        public static readonly string ID = typeof( Rectile ).Name;

        private const string HoverStatesGroup = "HoverStates";

        #region depeendency properties
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register( "State", typeof( HoverStates ), typeof( Rectile ), new PropertyMetadata( HoverStates.Loading, OnStateChanged ) );
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register( "Size", typeof( RectileSize ), typeof( Rectile ), new PropertyMetadata( RectileSize.Medium, OnSizeChanged ) );
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register( "Title", typeof( string ), typeof( Rectile ), new PropertyMetadata( "{Title}", OnTitleChanged ) );
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register( "Image", typeof( ImageSource ), typeof( Rectile ), new PropertyMetadata( null, OnImageChanged ) );
        public static readonly DependencyProperty IsLoadedProperty = DependencyProperty.Register( "IsLoaded", typeof( bool ), typeof( Rectile ), new PropertyMetadata( false, OnIsLoadedChanged ) );
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register( "Description", typeof( string ), typeof( Rectile ), new PropertyMetadata( "{Description}", OnDescriptionChanged ) );

        public RectileSize Size
        {
            get { return ( RectileSize ) GetValue( SizeProperty ); }
            set { SetValue( SizeProperty, value ); }
        }

        public string Title
        {
            get { return ( string ) GetValue( TitleProperty ); }
            set { SetValue( TitleProperty, value ); }
        }

        public ImageSource Image
        {
            get { return ( ImageSource ) GetValue( ImageProperty ); }
            set { SetValue( ImageProperty, value ); }
        }

        public bool IsLoaded
        {
            get { return ( bool ) GetValue( IsLoadedProperty ); }
            set { SetValue( IsLoadedProperty, value ); }
        }

        public string Description
        {
            get { return ( string ) GetValue( DescriptionProperty ); }
            set { SetValue( DescriptionProperty, value ); }
        }
 
        public HoverStates State
        {
            get { return ( HoverStates ) GetValue( StateProperty ); }
            set { SetValue( StateProperty, value ); }
        }
        #endregion

        private Border Stage;

        private Grid ContentTC;
        private Grid ContentCT;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Property Changed Callbacks
        private static void OnSizeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( Rectile ) d ).UpdateSize();
        }

        private static void OnTitleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( Rectile ) d ).NotifyChanged( "Title" );
        }

        private static void OnImageChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( Rectile ) d ).NotifyChanged( "Image" );
        }

        private static void OnIsLoadedChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            Rectile r = ( ( Rectile ) d );
            r.State = HoverStates.Idle;
            OnStateChanged( d, e );
            r.NotifyChanged( "IsLoaded" );
        }

        private static void OnDescriptionChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( Rectile ) d ).NotifyChanged( "Description" );
        }

        private static void OnStateChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( Rectile ) d ).UpdateVisualState( true );
        }

        private void NotifyChanged( string Name )
        {
            if ( PropertyChanged != null ) PropertyChanged( this, new PropertyChangedEventArgs( Name ) );
        }
        #endregion

        private void UpdateContentVisibility()
        {
            if ( ContentTC == null || ContentCT == null ) return;
            ContentTC.Visibility
                = ContentCT.Visibility
                = Visibility.Collapsed;
            switch( Size )
            {
                case RectileSize.Large:
                    ContentCT.Visibility = Visibility.Visible;
                    break;

                case RectileSize.Medium:
                default:
                    ContentTC.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void UpdateSize()
        {
            if ( Stage == null ) return;

            switch ( Size )
            {
                case RectileSize.Large:
                    Stage.Width = Stage.Height = ( int ) RectileSize.Large;
                    break;
                case RectileSize.Medium:
                    Stage.Width = ( int ) RectileSize.Large;
                    Stage.Height = ( int ) RectileSize.Medium;
                    break;
                default:
                    throw new ArgumentException( string.Format( "No such Size {0}", Size ) );
            }

        }

        private void UpdateVisualState( bool useTransitions )
        {
            switch ( State )
            {
                case HoverStates.Loading:
                    VisualStateManager.GoToState( this, "Loading", useTransitions );
                    break;
                case HoverStates.Hovered:
                    VisualStateManager.GoToState( this, "Hovered", useTransitions );
                    break;
                case HoverStates.Idle:
                default:
                    VisualStateManager.GoToState( this, "Idle", useTransitions );
                    break;
            }
            NotifyChanged( "State" );
        }

        public Rectile()
            :base()
        {
            DefaultStyleKey = typeof( Rectile );
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Stage = ( Border ) GetTemplateChild( "Stage" );
            ContentCT = ( Grid ) GetTemplateChild( "ContentCT" );
            ContentTC = ( Grid ) GetTemplateChild( "ContentTC" );

            UpdateVisualState( false );
            UpdateSize();
        }

        protected override void OnPointerEntered( PointerRoutedEventArgs e )
        {
            base.OnPointerEntered( e );
            if ( State != HoverStates.Loading )
            {
                State = HoverStates.Hovered;
            }
        }

        protected override void OnPointerExited( PointerRoutedEventArgs e )
        {
            base.OnPointerExited( e );
            if ( State != HoverStates.Loading )
            {
                State = HoverStates.Idle;
            }
        }
    }
}
