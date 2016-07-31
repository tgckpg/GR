using Net.Astropenguin.DataModel;
using System.ComponentModel;

namespace wenku8.Model.ListItem
{
    class NameValue<T> : ActiveData, INamable
    {
        protected string _Name;
        virtual public string Name
        {
            get { return _Name; }
            set { _Name = value;  NotifyChanged( "Name" ); }
        }

        protected T _Value;
        virtual public T Value
        {
            get { return _Value; }
            set { _Value = value;  NotifyChanged( "Value" ); }
        }

        public NameValue( string Name, T Value )
        {
            _Name = Name;
            _Value = Value;
        }
    }
}