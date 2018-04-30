using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Messaging;

namespace GR.Config.Scopes
{
	using Database.Contexts;
	using Database.Models;

	class ScopedConfig<TEntity>
		where TEntity : GenericSettings
	{
		virtual protected string ScopeId => "";

		protected string ConfigKey( string Key ) => ScopeId + "." + Key;

		protected T GetValue<T>( string Key, Func<object> DefaultValue )
		{
			object Val = GetValue<object>( Key, ( object ) null );
			if ( Val == null )
				return ( T ) DefaultValue?.Invoke();

			return ( T ) Val;
		}

		protected T GetValue<T>( string Key, object DefaultValue )
		{
			try
			{
				using ( var Context = new SettingsContext() )
				{
					TEntity Entity = Context.Find<TEntity>( ConfigKey( Key ) );
					if ( Entity != null )
					{
						return ( T ) Entity.GetValue();
					}
				}
			}
			catch ( Exception ) { }

			return ( T ) DefaultValue;
		}

		protected void SetValue<T>( string Key, T value )
		{
			using ( var Context = new SettingsContext() )
			{
				string _Key = ConfigKey( Key );
				TEntity Entity = Context.Find<TEntity>( _Key );
				if ( Entity == null )
				{
					Entity = ( TEntity ) Activator.CreateInstance( typeof( TEntity ) );
					Entity.Key = _Key;
					Entity.SetValue( value );
					Context.Add( Entity );
				}
				else
				{
					Entity.SetValue( value );
					Context.Update( Entity );
				}

				Context.SaveChanges();
			}

			GRConfig.ConfigChanged.Deliver( new Message( GetType(), Key, value ) );
		}

	}
}