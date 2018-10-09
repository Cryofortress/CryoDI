namespace CryoDI
{
	internal class RuntimeResolver<T> : IRuntimeResolver<T>
	{
		private readonly string _name;
		private readonly CryoContainer _container;

		public RuntimeResolver(string name, CryoContainer container)
		{
			_name = name;
			_container = container;
		}

		public T Get()
		{
			return _container.ResolveByName<T>(_name);
		}
	}
}