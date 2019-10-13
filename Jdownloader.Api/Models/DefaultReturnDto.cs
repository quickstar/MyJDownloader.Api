using System;

namespace Jdownloader.Api.Models
{
	public class DefaultReturnDto<T> : BaseDto
	{
		public T Data { get; set; }
	}
}