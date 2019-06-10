namespace CryoDI
{
	public interface IWeakResolver<out T>
	{
		T Resolve(params object[] parameters);
		T ResolveByName(string name, params object[] parameters);
	}
}