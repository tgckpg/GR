using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Book
{
	using Database.Models;

	sealed class VirtualVolume : Volume
	{
		public VirtualVolume( Volume SrcVolume, int ChunkIndex, int StartIndex, int ChunkSize )
			: base()
		{
			Id = SrcVolume.Id;
			Title = string.Format( "({1}) {0}", SrcVolume.Title, ChunkIndex );

			Chapters = SrcVolume.Chapters.Skip( StartIndex ).Take( ChunkSize ).ToList();
		}

		public static VirtualVolume[] Create( Volume SrcVolume, int ChunkSize = 30 )
		{
			int l = SrcVolume.Chapters.Count();

			VirtualVolume[] VVols = new VirtualVolume[ ( int ) Math.Ceiling( l / ( double ) ChunkSize ) ];
			for ( int i = 0, j = 0; i < l; i += ChunkSize, j ++ )
			{
				VVols[ j ] = new VirtualVolume( SrcVolume, j + 1, i, ChunkSize );
			}

			return VVols;
		}
	}
}