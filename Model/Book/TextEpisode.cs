using Net.Astropenguin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR.Model.Loaders;
using GR.Settings;
using GR.Storage;
using GR.GSystem;

namespace GR.Model.Book
{
	class TextEpisode
	{
		string Content = "";
		public string Title { get; set; }
		public string id { get; private set; }

		string aid;

		public TextEpisode( string aid, string Title )
		{
			this.aid = aid;
			this.Title = Title;
			id = Utils.Md5( Title );
		}

		public TextEpisode( string id, string aid, bool b )
		{
			this.id = id;
			this.aid = aid;
		}

		public async Task Save( string vid )
		{
			MessageBus.SendUI( typeof( ListItem.LocalBook ), "Saving ... " + Title, aid );
			throw new NotImplementedException();

			// await new ContentParser().ParseAsync( Content, new Chapter( Title, aid, vid, id ) );
		}

		public void Push( string p )
		{
			Content += p + "\n";
		}
	}
}
