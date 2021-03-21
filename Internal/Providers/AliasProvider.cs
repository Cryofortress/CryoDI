namespace CryoDI.Providers
{
	public class AliasProvider<T> : IObjectProvider
	{
		private readonly string _name;

		public AliasProvider(string name = null)
		{
			_name = name;
		}

		public void Dispose()
		{
		}

		public LifeTime LifeTime
		{
			get { return LifeTime.External; }
		}

		public object GetObject(object owner, CryoContainer container, params object[] parameters)
		{
			return container.ResolveByName<T>(_name, parameters);
		}

		public object WeakGetObject(CryoContainer container, params object[] parameters)
		{
			return container.TryResolveByName<T>(_name, parameters);
		}
	}
}