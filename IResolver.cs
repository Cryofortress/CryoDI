namespace CryoDI
{
	public interface IResolver<out T>
	{
		T Resolve(params object[] parameters);
		T ResolveByName(string name, params object[] parameters);
	}
}