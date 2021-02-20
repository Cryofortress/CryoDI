namespace CryoDI
{
	public interface IBuilder<T>
	{
		T BuildUp(T obj, params object[] parameters);
	}
}