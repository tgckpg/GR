using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GR.Database.Models
{
	using Schema;

	public class ZCache
	{
		[Key]
		public string Key { get; set; }

		[NotMapped]
		public ZData Data { get; set; } = new ZData();

		public byte[] RawData
		{
			get => Data.RawBytes;
			set => Data.RawBytes = value;
		}
	}
}