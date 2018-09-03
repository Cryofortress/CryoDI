
namespace CryoDI.ViewMediatorBinding
{
	public class View
	{
		public static IMediatorFactory MediatorFactory { protected get; set; }
	}

	public class View<T> : View where T : class
	{
		public T Mediator { get; private set; }

		public virtual void Awake()
		{
			Mediator = MediatorFactory.CreateAndBindMediator<T>(this);
		}

		public virtual void OnDestroy()
		{
			MediatorFactory.UnbindMediator(Mediator, this);
			Mediator = null;
		}
	}
}
