using System;
using System.Collections.Generic;

namespace Jdownloader.Api.Models
{
	public class DevicesDto : BaseDto
	{
		public IEnumerable<DeviceItemDto> List { get; set; }
	}

	public class DeviceItemDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Status { get; set; }

		public string Type { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}