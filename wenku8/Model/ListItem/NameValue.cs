using Net.Astropenguin.DataModel;
using System.ComponentModel;
using System;

namespace wenku8.Model.ListItem
{
    interface INameValue
    {
        string Name { get; }
        object Value { get; }
    }
    class NameValue<T> : ActiveData, INamable, INameValue
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

        object INameValue.Value { get { return _Value; } }

        public NameValue( string Name, T Value )
        {
            _Name = Name;
            _Value = Value;
        }
    }
}