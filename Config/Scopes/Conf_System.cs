using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Config.Scopes
{
	class Conf_System : ScopedConfig<Database.Models.GRSystem>
	{
		public bool EnableOneDrive
		{
			get => GetValue( "EnableOneDrive", false );
			set => SetValue( "EnableOneDrive", value );
		}

		public bool ChunkSingleVol
		{
			get => GetValue( "ChunkSingleVol", true );
			set => SetValue( "ChunkSingleVol", value );
		}

		public bool PatchSyntax
		{
			get => GetValue( "PatchSyntax", true );
			set => SetValue( "PatchSyntax", value );
		}

		public bool TwitterConfirmed
		{
			get => GetValue( "TwitterConfirmed", false );
			set => SetValue( "TwitterConfirmed", value );
		}

		public string GCustomSearchAPI
		{
			get => GetValue( "GCustomSearchAPI", ( string ) null );
			set => SetValue( "GoogleCustomSearch", value );
		}

		public string BingImageAPI
		{
			get => GetValue( "BingImageAPI", ( string ) null );
			set => SetValue( "BingImageAPI", value );
		}

	}
}