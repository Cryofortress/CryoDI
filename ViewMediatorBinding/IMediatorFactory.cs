namespace CryoDI.ViewMediatorBinding
{
	public interface IMediatorFactory
	{
		T CreateAndBindMediator<T>(View<T> view) where T : class;

		void UnbindMediator<T>(T mediator, View<T> view) where T : class;

		IMediatorFactory RegisterMediator<V, M>()
			where V : View<M>
			where M : class, new();
	}
}
