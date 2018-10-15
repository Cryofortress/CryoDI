namespace CryoDI
{
	public interface IResolver<out T>
	{
		T Resolve(params object[] parameters);
	}
}