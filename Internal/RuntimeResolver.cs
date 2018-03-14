namespace CryoDI
{
	internal class RuntimeResolver<T> : IRuntimeResolver<T>
	{
		private readonly string _name;
		private readonly Container _container;

		public RuntimeResolver(string name, Container container)
		{
			_name = name;
			_container = container;
		}

		public T Get()
		{
			return _container.Resolve<T>(_name);
		}
	}
}