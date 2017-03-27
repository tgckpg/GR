using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace wenku8.Model.Text
{
	using Interfaces;
	using Resources;
	using ListItem;
	using Config;

	sealed class IllusPara : Paragraph, IIllusUpdate
	{
		public bool Valid { get; private set; }

		public ImageThumb ImgThumb { get; set; }
		public string SrcUrl { get; private set; }

		private Uri _Illus;
		public Uri Illus
		{
			get
			{
				if( EmbedIllus ) return _Illus;
				return null;
			}

			private set { _Illus = value; }
		}

		private static bool _EmbedIllus = Properties.APPEARANCE_CONTENTREADER_EMBED_ILLUS;
		public bool EmbedIllus { get { return _EmbedIllus; } }

		public IllusPara( string Illus )
			: base( SegoeMDL2.Photo2 )
		{
			SrcUrl = Illus;
		}

		public void Update()
		{
			Illus = new Uri( "ms-appdata:///local/" + ImgThumb.Location );
			NotifyChanged( "Illus" );
		}

		public override void Dispose()
		{
			base.Dispose();
			ImgThumb = null;
			Illus = null;
			NotifyChanged( "Illus" );
		}
	}
}