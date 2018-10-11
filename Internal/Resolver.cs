namespace CryoDI
{
	internal class Resolver<T> : IResolver<T>
	{
		private readonly string _name;
		private readonly CryoContainer _container;

		public Resolver(string name, CryoContainer container)
		{
			_name = name;
			_container = container;
		}

		public T Get(params object[] parameters)
		{
			return _container.ResolveByName<T>(_name, parameters);
		}
	}
}