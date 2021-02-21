using System;

namespace CryoDI
{
	public class CircularDependencyException : ContainerException
	{
		public CircularDependencyException()
		{
		}

		public CircularDependencyException(string message) : base(message)
		{
		}

		public CircularDependencyException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}