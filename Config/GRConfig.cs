using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

using Net.Astropenguin.Messaging;

namespace GR.Config
{
	class GRConfig
	{
		public static Messenger ConfigChanged = new Messenger();

		public static Scopes.Conf_Theme Theme => new Scopes.Conf_Theme();
		public static Scopes.Conf_System System => new Scopes.Conf_System();
		public static Scopes.Conf_BookInfoView BookInfoView => new Scopes.Conf_BookInfoView();
		public static Scopes.Conf_ContentReader ContentReader => new Scopes.Conf_ContentReader();
		public static Scopes.Conf_MasterExplorer MasterExplorer => new Scopes.Conf_MasterExplorer();
	}
}