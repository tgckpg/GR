using System;
using System.Collections.Generic;

namespace wenku8.Model.ListItem
{
	class Topic : ActiveItem
	{
		public Digests[] Collections { get; private set; }

		public string Index { get; private set; }

        public int TreeLevel { get { return 0; } }

		public string LatestSection { get; set; }

		// Topic that contains peroidic digests
		public Topic( string name, Digests[] digests, string desc, string index, string section )
			: base( name, desc, index )
		{
			Collections = digests;
			Index = index;
			LatestSection = section;
		}

		// Single Topic
		public Topic( string name, string path, string desc, string index, string section )
			: base( name, desc, index )
		{
			Index = index;
			LatestSection = section;
		}

		public Topic( string name, string desc, string guid )
			:base( name, desc, guid )
		{
			// -1 means only message available, used by announcement
			Index = "-1";
		}
    }
}