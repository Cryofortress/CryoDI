using System;

namespace CryoDI.ViewMediatorBinding
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class SubscribeAttribute : Attribute
	{
		public SubscribeAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}
