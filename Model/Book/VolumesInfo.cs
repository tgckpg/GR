using System;
using System.Collections.Generic;
using System.Linq;

namespace GR.Model.Book
{
	using Database.Models;

	sealed class VolumesInfo
	{
		public static readonly string ID = typeof( VolumesInfo ).Name;
		public string[] VolTitles;
		public int[] vids;
		public int[][] cids;
		public string[][] EpTitles;

		public Volume[] Volumes { get; private set; }

		public VolumesInfo( BookItem b )
		{
			Volumes = b.GetVolumes();

			int l = Volumes.Length;
			vids = new int[ l ];
			cids = new int[ l ][];
			VolTitles = new string[ l ];
			EpTitles = new string[ l ][];

			for ( int i = 0; i < l; i++ )
			{
				Volume V = Volumes[ i ];
				VolTitles[ i ] = V.Title;
				vids[ i ] = V.Id;

				int k = V.Chapters.Count();

				cids[ i ] = new int[ k ];
				EpTitles[ i ] = new string[ k ];

				for ( int j = 0; j < k; j++ )
				{
					Chapter C = V.Chapters[ j ];
					cids[ i ][ j ] = C.Id;
					EpTitles[ i ][ j ] = C.Title;
				}
			}
		}

	}
}