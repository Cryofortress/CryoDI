namespace CryoDI.ViewMediatorBinding
{
	public interface IMediator
	{
		void AfterViewBinded<T>(View<T> view) where T : class;
		void BeforeViewDestroyed();
	}
}
