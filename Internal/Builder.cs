namespace CryoDI
{
	public class Builder<T> : IBuilder<T>
	{
		private readonly CryoContainer _container;

		public Builder(CryoContainer container)
		{
			_container = container;
		}
		
		public void BuildUp(T obj, params object[] parameters)
		{
			_container.BuildUp(obj, parameters);
		}
	}
}