using System;

namespace Jdownloader.Api.Exceptions
{
	public class InvalidRequestIdException : Exception
	{
		public InvalidRequestIdException(string msg)
			: base(msg)
		{ }
	}
}