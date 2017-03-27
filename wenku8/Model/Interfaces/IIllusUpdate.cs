using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Model.Interfaces
{
	using ListItem;
	interface IIllusUpdate
	{
		ImageThumb ImgThumb { get; set; }
		string SrcUrl { get; }
		void Update();
	}
}