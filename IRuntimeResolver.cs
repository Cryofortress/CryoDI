namespace CryoDI
{
	public interface IRuntimeResolver<out T>
	{
		T Get();
	}
}