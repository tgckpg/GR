using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Model.Book
{
	sealed class VirtualVolume : Volume
	{
		public VirtualVolume( Volume SrcVolume, int ChunkIndex, int StartIndex, int ChunkSize )
			: base()
		{
			vid = SrcVolume.vid;
			EndVisibility = SrcVolume.EndVisibility;

			ChapterList = SrcVolume.ChapterList.Skip( StartIndex ).Take( ChunkSize ).ToArray();

			VolumeTitle = string.Format(
				"({1}) {0}"
				, SrcVolume.VolumeTitle
				, ChunkIndex );
		}

		public static VirtualVolume[] Create( Volume SrcVolume, int ChunkSize = 30 )
		{
			int l = SrcVolume.ChapterList.Length;

			VirtualVolume[] VVols = new VirtualVolume[ ( int ) Math.Ceiling( l / ( double ) ChunkSize ) ];
			for ( int i = 0, j = 0; i < l; i += ChunkSize, j ++ )
			{
				VVols[ j ] = new VirtualVolume( SrcVolume, j + 1, i, ChunkSize );
			}

			return VVols;
		}
	}
}