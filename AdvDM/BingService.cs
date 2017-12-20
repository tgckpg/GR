using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;

namespace GR.AdvDM
{
	using Model.Book;
	using Model.REST;
	using Settings;

	sealed class BingService
	{
		public static string SYS_API_KEY { get; set; }
		public static string API_KEY { get; private set; }

		public string DefaultKeyword { get { return Book.Title + " " + Book.AuthorRaw; } }

		private XRegistry ServiceReg;
		private XParameter ServParam;

		private BookItem Book;

		public static void SetApiKey( string Key )
		{
			API_KEY = string.IsNullOrEmpty( Key ) ? SYS_API_KEY : Key;
		}

		public BingService( BookItem Book )
		{
			this.Book = Book;

			ServiceReg = new XRegistry( "<bing />", FileLinks.ROOT_SETTING + FileLinks.BING_SERVICE );

			ServParam = ServiceReg.Parameter( Book.Id );

			if ( ServParam == null )
			{
				ServParam = new XParameter( Book.Id );
				ServParam.SetValue( new XKey( "keyword", DefaultKeyword ) );
				ServParam.SetValue( new XKey( "offset", 0 ) );

				ServiceReg.SetParameter( ServParam );
				ServiceReg.Save();
			}
		}

		public async Task<string> GetImage( bool FullSize = false )
		{
			int offset = ServParam.GetSaveInt( "offset" );
			XParameter ImgParam = ServParam.Parameter( offset.ToString() );

			string ImgUrl = ImgParam?.GetValue( "url" );

			if ( string.IsNullOrEmpty( ImgUrl ) )
			{
				BingImageSearch BReq = new BingImageSearch( ServParam.GetValue( "keyword" ) );

				ImgUrl = await ( FullSize ? BReq.GetFullImage( offset ) : BReq.GetImage( offset ) );

				ImgParam = new XParameter( offset.ToString() );
				ImgParam.SetValue( new XKey( "url", ImgUrl ) );

				SetResultUrl( ImgParam, BReq.ResultObj );
			}

			return ImgUrl;
		}

		public void SetKeyword( string Keyword )
		{
			ServParam.SetValue( new XKey( "keyword", Keyword ) );
			ServParam.SetValue( new XKey( "offset", 0 ) );
			ServParam.ClearParams();

			ServiceReg.SetParameter( ServParam );
			ServiceReg.Save();
		}

		public void SetOffset( int Value )
		{
			int offset = ServParam.GetSaveInt( "offset" );

			if ( Value == 0 ) offset = 0;
			else offset += Value;

			ServParam.SetValue( new XKey( "offset", offset ) );
			ServiceReg.SetParameter( ServParam );
			ServiceReg.Save();
		}

		public string GetSearchQuery()
		{
			XParameter ImgParam = ServParam.Parameter( ServParam.GetValue( "offset" ) );
			return ImgParam?.GetValue( "browser" );
		}
		public string GetKeyword() { return ServParam.GetValue( "keyword" ); }
		public bool Exists() { return ServParam.GetParameters().FirstOrDefault() != null; }

		private void SetResultUrl( XParameter ImgParam, JsonObject Obj )
		{
			if ( Obj == null ) return;

			ImgParam.SetValue( new XKey( "browser", Obj.GetNamedString( "webSearchUrl" ) ) );

			ServParam.SetParameter( ImgParam );
			ServiceReg.SetParameter( ServParam );

			ServiceReg.Save();
		}

	}

	sealed class BingHttpRequest : HttpRequest
	{
		public BingHttpRequest( Uri RequestUri ) : base( RequestUri ) { EN_UITHREAD = false; }

		override protected void CreateRequest()
		{
			base.CreateRequest();
			UserAgent = WHttpRequest.UA;
			WCMessage.Headers.Add( "Ocp-Apim-Subscription-Key", BingService.API_KEY );
		}
	}

}