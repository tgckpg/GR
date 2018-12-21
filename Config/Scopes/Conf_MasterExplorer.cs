using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Config.Scopes
{
	class Conf_MasterExplorer : ScopedConfig<Database.Models.GRSystem>
	{
		public bool DefaultWidgets
		{
			get => GetValue( "DefaultWidgets", true );
			set => SetValue( "DefaultWidgets", value );
		}
	}
}