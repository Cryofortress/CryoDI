using System;

namespace CryoDI
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class DependencyAttribute : Attribute
	{
		public DependencyAttribute() {}

		public DependencyAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}
