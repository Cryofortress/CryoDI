using System;

namespace CryoDI
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ParamAttribute : Attribute
	{
		public ParamAttribute() {}

		public ParamAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}