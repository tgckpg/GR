using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.IO;
using Net.Astropenguin.Messaging;

namespace wenku8.Model.Book
{
	using Settings;
	using System;

	class TextVolume
	{
		public string id { get; private set; }
		public string Title { get; set; }
		public string MetaLocation
		{
			get { return FileLinks.ROOT_LOCAL_VOL + aid + "/" + id + ".vol"; }
		}

		private string aid;
		private XRegistry VolReg;
		private IEnumerable<TextEpisode> Episodes;

		// For write
		public TextVolume( string aid, string Title, IEnumerable<TextEpisode> VolGroup )
		{
			this.aid = aid;
			this.Title = Title;
			id = Utils.Md5( Title );

			Episodes = VolGroup;
			VolReg = new XRegistry( "<VolumeMeta />", MetaLocation );
		}

		// For read
		public TextVolume( string aid, string hash )
		{
			this.aid = aid;
			id = hash;

			VolReg = new XRegistry( "<VolumeMeta />", MetaLocation );

			XParameter Param = VolReg.Parameter( AppKeys.GLOBAL_META );
			Title = Param.GetValue( AppKeys.GLOBAL_NAME );
		}

		public async Task Save()
		{
			List<XKey> Keys = new List<XKey>();
			foreach ( TextEpisode Ep in Episodes )
			{
				VolReg.SetParameter( Ep.id, new XKey[] {
					new XKey( AppKeys.GLOBAL_CID, true )
					, new XKey( AppKeys.GLOBAL_NAME, Ep.Title.Substring( Title.Length ) )
				} );
				await Ep.Save( id );
			}

			VolReg.SetParameter( AppKeys.GLOBAL_META, new XKey( AppKeys.GLOBAL_NAME, Title.Trim() ) );

			MessageBus.SendUI( typeof( ListItem.LocalBook ), "Saving ... " + Title, aid );
			VolReg.Save();
		}

		public Chapter[] GetChapters()
		{
			XParameter[] Params = VolReg.Parameters( AppKeys.GLOBAL_CID );

			List<Chapter> Chapters = new List<Chapter>();
			foreach( XParameter Param in Params )
			{
				LocalChapter C = new LocalChapter( Param.GetValue( AppKeys.GLOBAL_NAME ), aid, id, Param.Id );
				Chapters.Add( C );
			}

			return Chapters.ToArray();
		}
	}
}
