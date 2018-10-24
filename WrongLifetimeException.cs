using System;

namespace CryoDI
{
	public class WrongLifetimeException : ContainerException
	{
		public WrongLifetimeException()
		{
		}

		public WrongLifetimeException(string message) : base(message)
		{
		}

		public WrongLifetimeException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}