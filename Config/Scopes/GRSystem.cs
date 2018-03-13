using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Config.Scopes
{
	class GRSystem : ScopedConfig<Database.Models.GRSystem>
	{
		public bool EnableOneDrive
		{
			get => GetValue<bool>( "EnableOneDrive", false );
			set => SetValue( "EnableOneDrive", value );
		}

		public bool ChunkSingleVol
		{
			get => GetValue<bool>( "ChunkSingleVol", true );
			set => SetValue( "ChunkSingleVol", value );
		}

		public bool PatchSyntax
		{
			get => GetValue<bool>( "PatchSyntax", true );
			set => SetValue( "PatchSyntax", value );
		}

	}
}