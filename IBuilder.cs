namespace CryoDI
{
	public interface IBuilder<in T>
	{
		void BuildUp(T obj, params object[] parameters);
	}
}