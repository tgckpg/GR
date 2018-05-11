using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Config.Scopes
{
	class Conf_BookInfoView : ScopedConfig<Database.Models.BookInfoView>
	{
		public Conf_BgContext BgContext => new Conf_BgContext();

		public class Conf_BgContext : ScopedConfig<Database.Models.BookInfoView>, IConf_BgContext
		{
			protected override string ScopeId => "BgContext";

			public string BgType
			{
				get => GetValue<string>( "BgType", null );
				set => SetValue( "BgType", value );
			}

			public string BgValue
			{
				get
				{
					if ( BgType == "System" )
						return "ms-appx:///Assets/Samples/BgInfoView.jpg";
					return GetValue<string>( "BgValue", null );
				}
				set => SetValue( "BgValue", value );
			}
		}

	}
}