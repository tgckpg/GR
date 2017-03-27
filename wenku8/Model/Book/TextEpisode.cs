using Net.Astropenguin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wenku8.Model.Loaders;
using wenku8.Settings;
using wenku8.Storage;
using wenku8.System;

namespace wenku8.Model.Book
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
			await new ContentParser().OrganizeBookContent( Content, new LocalChapter( Title, aid, vid, id ) );
		}

		public void Push( string p )
		{
			Content += p + "\n";
		}
	}
}
