namespace CryoDI
{
	public interface IResolver<out T>
	{
		T Get(params object[] parameters);
	}
}