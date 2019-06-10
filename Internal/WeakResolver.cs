namespace CryoDI
{
	public class WeakResolver <T> : IWeakResolver<T>
	{
		private readonly string _name;
		private readonly CryoContainer _container;

		public WeakResolver(string name, CryoContainer container)
		{
			_name = name;
			_container = container;
		}

		public T Resolve(params object[] parameters)
		{
			return _container.WeakResolveByName<T>(_name, parameters);
		}
		
		public T ResolveByName(string name, params object[] parameters)
		{
			return _container.WeakResolveByName<T>(name, parameters);
		}
	}
}