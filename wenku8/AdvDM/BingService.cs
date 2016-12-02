using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;

namespace wenku8.AdvDM
{
    using Model.Book;
    using Model.REST;
    using Settings;

    sealed class BingService
    {
        private XRegistry ServiceReg;
        private XParameter ServParam;

        private BookItem Book;

        public BingService( BookItem Book )
        {
            ServiceReg = new XRegistry( "<bing />", FileLinks.ROOT_SETTING + FileLinks.BING_SERVICE );

            ServParam = ServiceReg.Parameter( Book.Id );
            if ( ServParam == null )
            {
                ServParam = new XParameter( Book.Id );
                ServParam.SetValue( new XKey( "keyword", Book.Title + " " + Book.AuthorRaw ) );
                ServParam.SetValue( new XKey( "offset", 0 ) );

                ServiceReg.SetParameter( ServParam );
                ServiceReg.Save();
            }

            this.Book = Book;
        }

        public async Task<string> GetImage()
        {
            BingImageSearch BReq = new BingImageSearch( ServParam.GetValue( "keyword" ) );

            string ImgUrl = await BReq.GetImage( ServParam.GetSaveInt( "offset" ) );
            SetResultUrl( BReq.ResultObj );

            return ImgUrl;
        }

        public async Task<string> GetNextImage()
        {
            XParameter ServParam = ServiceReg.Parameter( Book.Id );

            if ( ServParam == null ) return null;

            int offset = ServParam.GetSaveInt( "offset" ) + 1;
            ServParam.SetValue( new XKey( "offset", offset ) );

            BingImageSearch BReq = new BingImageSearch( ServParam.GetValue( "keyword" ) );

            return await BReq.GetImage( offset );
        }


        public string GetSearchQuery()
        {
            return ServParam.GetValue( "browser" );
        }

        private void SetResultUrl( JsonObject Obj )
        {
            ServParam.SetValue( new XKey( "browser", Obj.GetNamedString( "webSearchUrl" ) ) );
            ServiceReg.SetParameter( ServParam );
            ServiceReg.Save();
        }

    }

    sealed class BingHttpRequest : HttpRequest
	{
		public BingHttpRequest( Uri RequestUri ) :base( RequestUri ) { EN_UITHREAD = false; } 

		override protected void CreateRequest()
		{
            base.CreateRequest();
            WCRequest.Headers[ HttpRequestHeader.UserAgent ] = WHttpRequest.UA;
            WCRequest.Headers[ "Ocp-Apim-Subscription-Key" ] = "";
		}
	}

}