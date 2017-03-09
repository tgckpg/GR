using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Resources
{
	class LocaleDefaults : IDictionary<string, object>
	{
		private Dictionary<string, object> Settings = new Dictionary<string, object>();

		public object this[ string key ]
		{
			get
			{
				if ( !Settings.ContainsKey( key ) ) return null;
				return Settings[ key ];
			}

			set
			{
				Settings[ key ] = value;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return ( ( IDictionary<string, object> ) Settings ).IsReadOnly;
			}
		}

		public T Get<T>( string key )
		{
			object r = this[ key ];

			if ( r != null )
			{
				return ( T ) r;
			}

			return default( T );
		}

		public void Add( KeyValuePair<string, object> item )
		{
			( ( IDictionary<string, object> ) Settings ).Add( item );
		}

		public void CopyTo( KeyValuePair<string, object>[] array, int arrayIndex )
		{
			( ( IDictionary<string, object> ) Settings ).CopyTo( array, arrayIndex );
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator() { return Settings.GetEnumerator(); }

		public bool Remove( KeyValuePair<string, object> item )
		{
			return ( ( IDictionary<string, object> ) Settings ).Remove( item );
		}

		public int Count { get { return Settings.Count; } }
		public ICollection<string> Keys { get { return Settings.Keys; } }
		public ICollection<object> Values { get { return Settings.Values; } }
		IEnumerator IEnumerable.GetEnumerator() { return Settings.GetEnumerator(); }

		public void Add( string key, object value ) { Settings.Add( key, value ); }
		public void Clear() { Settings.Clear(); }
		public bool Contains( KeyValuePair<string, object> item ) { return Settings.Contains( item ); }
		public bool ContainsKey( string key ) { return Settings.ContainsKey( key ); }
		public bool Remove( string key ) { return Settings.Remove( key ); }
		public bool TryGetValue( string key, out object value ) { return Settings.TryGetValue( key, out value ); }
	}
}