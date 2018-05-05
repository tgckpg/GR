using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Interfaces
{
	interface IImageService
	{
		string DefaultKeyword { get; }
		Task<string> GetImage( bool FullSize = false );

		void SetApiKey( string ApiKey );
		void SetKeyword( string Keyword );
		void SetOffset( int Value );

		string GetSearchQuery();
		string GetKeyword();

		bool Exists();
	}
}