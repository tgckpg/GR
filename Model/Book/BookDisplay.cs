using GR.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Linq;

namespace GR.Model.Book
{
	using Data;
	using Database.Models;

	public class BookDisplay : ActiveData
	{
		public Book Entry { get; private set; }

		public BookDisplay( Book Entry )
		{
			this.Entry = Entry;
			_Zone = Entry.ZoneId;
		}

		private string _Zone;
		public string Zone
		{
			get { return _Zone; }
			set { _Zone = value; NotifyChanged( "Zone" ); }
		}

		public string Intro => Entry.Description ?? Entry.Info.LongDescription;
		public string LastAccess => Entry.LastAccess.ToString();

		public override bool Equals( object obj ) => Entry.Equals( ( obj as BookDisplay )?.Entry );
		public override int GetHashCode() => Entry.GetHashCode();

		public object Payload { get; set; }

		public static Func<IQueryable<Book>, IQueryable<Book>> QuerySort( PropertyInfo Prop , int Order )
		{
			ParameterExpression _x = Expression.Parameter( typeof( Book ), "x" );

			Expression OrderExp;
			if ( Prop.DeclaringType == typeof( Book ) )
			{
				OrderExp = Expression.PropertyOrField( _x, Prop.Name );
			}
			else if ( Prop.DeclaringType == typeof( BookInfo ) )
			{
				OrderExp = Expression.PropertyOrField( _x, "Info" );
				OrderExp = Expression.PropertyOrField( OrderExp, Prop.Name );
			}
			else if ( Prop.DeclaringType == typeof( BookDisplay ) )
			{
				// Special fields
				switch ( Prop.Name )
				{
					case "LastAccess":
						OrderExp = Expression.PropertyOrField( _x, Prop.Name );
						break;
					case "Zone":
						OrderExp = Expression.PropertyOrField( _x, "ZoneId" );
						break;
					default:
						return null;
				}
			}
			else
			{
				return null;
			}

			string OrderMethod = Order == 1 ? "OrderBy" : "OrderByDescending";

			return ( x ) =>
			{
				Expression _Exp = Expression.Call(
							typeof( Queryable ), OrderMethod,
							new Type[] { x.ElementType, OrderExp.Type },
							x.Expression, Expression.Quote( Expression.Lambda( OrderExp, _x ) ) );
				return x.Provider.CreateQuery<Book>( _Exp );
			};
		}

		public static List<IGRCell> GetHeaders()
		{
			List<IGRCell> BkHeaders = new List<IGRCell>();

			string[] BkExclude = new string[] { "ZoneId", "ZItemId", "Description" };
			string[] InfoExclude = new string[] { "LongDescription" };

			Type StringType = typeof( string );

			BkHeaders.AddRange(
				typeof( Book ).GetProperties()
					.Where(
						x => x.PropertyType == StringType
						&& !( x.Name.StartsWith( "Json_" ) || BkExclude.Contains( x.Name ) ) )
					.Remap( p => new GRCell<BookDisplay>( p ) { Path = x => x.Entry } )
			);

			BkHeaders.AddRange(
				typeof( BookDisplay ).GetProperties()
					.Where( x => x.PropertyType == StringType )
					.Remap( p => new GRCell<BookDisplay>( p ) )
			);

			BkHeaders.AddRange(
				typeof( BookInfo ).GetProperties()
					.Where( x => x.PropertyType == StringType
						&& !( x.Name.StartsWith( "Json_" ) || InfoExclude.Contains( x.Name ) ) )
					.Remap( p => new GRCell<BookDisplay>( p ) { Path = x => x.Entry.Info } )
			);

			return BkHeaders;
		}
	}
}